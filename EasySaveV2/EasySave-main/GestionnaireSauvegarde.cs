using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
//using Log;  // Référence à la gestion des logs

namespace EasySaveApp
{
    public class GestionnaireSauvegarde
    {
        private string langue = "fr"; // Langue par défaut

        private Dictionary<string, Dictionary<string, string>> messages = new()
        {
            { "fr", new Dictionary<string, string>
                {
                    {"MenuTitle", "EasySave - Menu"},
                    {"StartBackup", "Démarrer une sauvegarde"},
                    {"ListBackups", "Lister les sauvegardes"},
                    {"RestoreBackup", "Restaurer une sauvegarde"},
                    {"ChangeLanguage", "Changer la langue"},
                    {"Exit", "Quitter"},
                    {"ChooseOption", "Choisissez une option: "},
                    {"InvalidOption", "Option invalide."},
                    {"BackupCompleted", "Sauvegarde terminée !"},
                    {"RestoreCompleted", "Restauration terminée !"},
                    {"NoBackups", "Aucune sauvegarde disponible."},
                    {"LanguageChanged", "Langue changée avec succès !"},
                }
            },
            { "en", new Dictionary<string, string>
                {
                    {"MenuTitle", "EasySave - Menu"},
                    {"StartBackup", "Start a backup"},
                    {"ListBackups", "List backups"},
                    {"RestoreBackup", "Restore a backup"},
                    {"ChangeLanguage", "Change language"},
                    {"Exit", "Exit"},
                    {"ChooseOption", "Choose an option: "},
                    {"InvalidOption", "Invalid option."},
                    {"BackupCompleted", "Backup completed !"},
                    {"RestoreCompleted", "Restore completed !"},
                    {"NoBackups", "No backups available."},
                    {"LanguageChanged", "Language changed successfully !"},
                }
            }
        };

        public void AfficherMessage(string message)
        {
            MessageBox.Show(message);
        }

        public void DemarrerSauvegarde(string nomSauvegarde, string cheminDossier)
        {
            if (string.IsNullOrEmpty(nomSauvegarde))
            {
                AfficherMessage("Nom de sauvegarde invalide.");
                return;
            }

            string cheminDestination = Path.Combine("Sauvegardes", nomSauvegarde);
            if (Directory.Exists(cheminDestination))
            {
                AfficherMessage("Cette sauvegarde existe déjà.");
                return;
            }

            if (string.IsNullOrEmpty(cheminDossier) || !Directory.Exists(cheminDossier))
            {
                AfficherMessage("Le dossier spécifié n'existe pas.");
                return;
            }

            Directory.CreateDirectory(cheminDestination);
            CopierDossier(cheminDossier, cheminDestination);
            AfficherMessage(ObtenirMessage("BackupCompleted"));
        }

        public void RestoreBackup(string backupPath, string destination, string nomSauvegarde)
        {
            long totalSize = 0;
            long totalFiles = 0;

            if (Directory.Exists(destination))
            {
                try
                {
                    Directory.Delete(destination, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur de suppression du dossier destination : {ex.Message}");
                    return;
                }
            }

            Directory.CreateDirectory(destination);
            CopierDossier(backupPath, destination);
            MessageBox.Show($"Restauration complète de {nomSauvegarde} terminée !");
        }

        public void RestoreBackupDifferential(string backupPath, string destination, string nomSauvegarde)
        {
            long totalSize = 0;
            long totalFiles = 0;

            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            CopierDossierDifferential(backupPath, destination);
            MessageBox.Show($"Restauration différentielle de {nomSauvegarde} terminée !");
        }

        private void CopierDossier(string sourceDir, string destinationDir)
        {
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string destDirPath = Path.Combine(destinationDir, Path.GetFileName(dir));
                CopierDossier(dir, destDirPath);
            }
        }

        private void CopierDossierDifferential(string sourceDir, string destinationDir)
        {
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));

                if (!File.Exists(destFile) || new FileInfo(file).Length != new FileInfo(destFile).Length)
                {
                    File.Copy(file, destFile, true);
                }
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string destDirPath = Path.Combine(destinationDir, Path.GetFileName(dir));
                CopierDossierDifferential(dir, destDirPath);
            }
        }


        public void SelectionnerLangue()
        {
            string? choixLangue = Microsoft.VisualBasic.Interaction.InputBox(
                "Sélectionnez la langue :\nfr -> Français\nen -> English",
                "Changer la langue",
                "fr"
            )?.Trim().ToLower();

            if (choixLangue == "fr" || choixLangue == "en")
            {
                langue = choixLangue;
                AfficherMessage(ObtenirMessage("LanguageChanged"));
            }
            else
            {
                AfficherMessage(ObtenirMessage("InvalidOption"));
                SelectionnerLangue();
            }
        }

        public string ObtenirMessage(string cle)
        {
            return messages.ContainsKey(langue) && messages[langue].ContainsKey(cle)
                ? messages[langue][cle]
                : "Message non trouvé";
        }
    }
}
