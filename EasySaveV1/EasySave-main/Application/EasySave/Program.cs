﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Log;

namespace EasySave
{
    class Program
    {
        public static Historic historic = new();

        static void Main(string[] args)
        {
            // Pour cet exemple, on supprime puis recrée le fichier JSON d'historique
            historic.DeleteFile();
            historic.CreateFile();

            Console.Clear();

            var gestionnaireSauvegarde = new GestionnaireSauvegarde();
            gestionnaireSauvegarde.SelectionnerLangue();

            bool continuer = true;
            while (continuer)
            {
                Console.WriteLine(gestionnaireSauvegarde.ObtenirMessage("MenuSeparator"));
                Console.WriteLine(gestionnaireSauvegarde.ObtenirMessage("MainMenuTitle"));
                Console.WriteLine(gestionnaireSauvegarde.ObtenirMessage("MenuSeparator"));
                Console.WriteLine("1. " + gestionnaireSauvegarde.ObtenirMessage("StartBackup"));
                Console.WriteLine("2. " + gestionnaireSauvegarde.ObtenirMessage("ListBackups"));
                Console.WriteLine("3. " + gestionnaireSauvegarde.ObtenirMessage("RestoreBackup"));
                Console.WriteLine("4. " + gestionnaireSauvegarde.ObtenirMessage("ChangeLanguage"));
                Console.WriteLine("5. " + gestionnaireSauvegarde.ObtenirMessage("Exit"));
                Console.Write(gestionnaireSauvegarde.ObtenirMessage("ChooseOption"));

                string? choix = Console.ReadLine();
                if (choix == null)
                {
                    Console.WriteLine(gestionnaireSauvegarde.ObtenirMessage("InvalidOption"));
                    continue;
                }

                switch (choix)
                {
                    case "1":
                        gestionnaireSauvegarde.DemarrerSauvegarde();
                        break;
                    case "2":
                        gestionnaireSauvegarde.ListerSauvegardes();
                        break;
                    case "3":
                        gestionnaireSauvegarde.RestaurerSauvegarde();
                        break;
                    case "4":
                        gestionnaireSauvegarde.SelectionnerLangue();
                        break;
                    case "5":
                        continuer = false;
                        Console.WriteLine(gestionnaireSauvegarde.ObtenirMessage("ThankYou"));
                        break;
                    default:
                        Console.WriteLine(gestionnaireSauvegarde.ObtenirMessage("InvalidOption"));
                        break;
                }
            }
        }
    }

    class GestionnaireSauvegarde
    {
        private string langue = "fr";

        // Dictionnaire des messages avec les clés pour la restauration différentielle
        private Dictionary<string, Dictionary<string, string>> messages = new Dictionary<string, Dictionary<string, string>>()
        {
            { "fr", new Dictionary<string, string>()
                {
                    {"MenuSeparator", "--------------------------------------"},
                    {"MainMenuTitle", "        Menu principal EasySave       "},
                    {"StartBackup", "Démarrer une sauvegarde"},
                    {"ListBackups", "Lister les sauvegardes"},
                    {"RestoreBackup", "Restaurer une sauvegarde"},
                    {"ChangeLanguage", "Changer la langue"},
                    {"Exit", "Quitter"},
                    {"ChooseOption", "Choisissez une option: "},
                    {"InvalidOption", "Option invalide, veuillez réessayer."},
                    {"EnterBackupName", "Entrez le nom de la sauvegarde:"},
                    {"InvalidBackupName", "Nom de sauvegarde invalide."},
                    {"EnterFolderPath", "Entrez le chemin du dossier à sauvegarder:"},
                    {"FolderNotExist", "Le dossier spécifié n'existe pas. Sauvegarde annulée."},
                    {"StartingBackup", "Démarrage de la sauvegarde '{0}'..."},
                    {"BackupCompleted", "Sauvegarde terminée."},
                    {"FileCopied", "Fichier copié: {0} -> {1}"},
                    {"FileCopyError", "Erreur lors de la copie du fichier '{0}': {1}"},
                    {"NoBackups", "Aucune sauvegarde n'a été effectuée."},
                    {"BackupName", "Nom : {0}, Date : {1}"},
                    {"EnterBackupToRestore", "Entrez le nom de la sauvegarde à restaurer (ex 1-n):"},
                    {"BackupNotExist", "La sauvegarde spécifiée n'existe pas."},
                    {"EnterRestoreDestination", "Entrez le chemin de destination pour la restauration:"},
                    {"InvalidDestination", "Chemin de destination invalide. Restauration annulée."},
                    {"CleaningDestination", "Nettoyage du dossier de destination..."},
                    {"RestoreCompleted", "Restauration terminée."},
                    {"ThankYou", "Merci d'avoir utilisé EasySave. Au revoir!"},
                    {"SelectLanguage", "Sélectionnez la langue / Select language:"},
                    {"LanguageChangedFr", "La langue a été changée en français."},
                    {"LanguageChangedEn", "La langue a été changée en anglais."},
                    // Messages pour la restauration différentielle
                    {"SelectRestoreType", "Sélectionnez le type de restauration:"},
                    {"CompleteRestore", "Restauration complète"},
                    {"DifferentialRestore", "Restauration différentielle"},
                    {"InvalidRestoreType", "Type de restauration invalide."},
                }
            },
            { "en", new Dictionary<string, string>()
                {
                    {"MenuSeparator", "--------------------------------------"},
                    {"MainMenuTitle", "          EasySave Main Menu          "},
                    {"StartBackup", "Start a backup"},
                    {"ListBackups", "List backups"},
                    {"RestoreBackup", "Restore a backup"},
                    {"ChangeLanguage", "Change language"},
                    {"Exit", "Exit"},
                    {"ChooseOption", "Choose an option: "},
                    {"InvalidOption", "Invalid option, please try again."},
                    {"EnterBackupName", "Enter the backup name:"},
                    {"InvalidBackupName", "Invalid backup name."},
                    {"EnterFolderPath", "Enter the path of the folder to backup:"},
                    {"FolderNotExist", "The specified folder does not exist. Backup cancelled."},
                    {"StartingBackup", "Starting backup '{0}'..."},
                    {"BackupCompleted", "Backup completed."},
                    {"FileCopied", "File copied: {0} -> {1}"},
                    {"FileCopyError", "Error copying file '{0}': {1}"},
                    {"NoBackups", "No backups have been made."},
                    {"BackupName", "Name: {0}, Date: {1}"},
                    {"EnterBackupToRestore", "Enter the name of the backup to restore:"},
                    {"BackupNotExist", "The specified backup does not exist."},
                    {"EnterRestoreDestination", "Enter the destination path for restoration (ex 1-n):"},
                    {"InvalidDestination", "Invalid destination path. Restoration cancelled."},
                    {"CleaningDestination", "Cleaning destination folder..."},
                    {"RestoreCompleted", "Restoration completed."},
                    {"ThankYou", "Thank you for using EasySave. Goodbye!"},
                    {"SelectLanguage", "Sélectionnez la langue / Select language:"},
                    {"LanguageChangedFr", "Language has been changed to French."},
                    {"LanguageChangedEn", "Language has been changed to English."},
                    // Messages for differential restore
                    {"SelectRestoreType", "Select restore type:"},
                    {"CompleteRestore", "Complete restore"},
                    {"DifferentialRestore", "Differential restore"},
                    {"InvalidRestoreType", "Invalid restore type."},
                }
            },
                        { "es", new Dictionary<string, string>()
                {
                    {"MenuSeparator", "--------------------------------------"},
                    {"MainMenuTitle", "          Menú Principal de EasySave          "},
                    {"StartBackup", "Iniciar una copia de seguridad"},
                    {"ListBackups", "Listar copias de seguridad"},
                    {"RestoreBackup", "Restaurar una copia de seguridad"},
                    {"ChangeLanguage", "Cambiar idioma"},
                    {"Exit", "Salir"},
                    {"ChooseOption", "Elija una opción: "},
                    {"InvalidOption", "Opción no válida, inténtelo de nuevo."},
                    {"EnterBackupName", "Introduzca el nombre de la copia de seguridad:"},
                    {"InvalidBackupName", "Nombre de copia de seguridad no válido."},
                    {"EnterFolderPath", "Introduzca la ruta de la carpeta a copiar:"},
                    {"FolderNotExist", "La carpeta especificada no existe. Copia de seguridad cancelada."},
                    {"StartingBackup", "Iniciando copia de seguridad '{0}'..."},
                    {"BackupCompleted", "Copia de seguridad completada."},
                    {"FileCopied", "Archivo copiado: {0} -> {1}"},
                    {"FileCopyError", "Error al copiar el archivo '{0}': {1}"},
                    {"NoBackups", "No se han realizado copias de seguridad."},
                    {"BackupName", "Nombre: {0}, Fecha: {1}"},
                    {"EnterBackupToRestore", "Introduzca el nombre de la copia de seguridad a restaurar:"},
                    {"BackupNotExist", "La copia de seguridad especificada no existe."},
                    {"EnterRestoreDestination", "Introduzca la ruta de destino para la restauración (ex 1-n):"},
                    {"InvalidDestination", "Ruta de destino no válida. Restauración cancelada."},
                    {"CleaningDestination", "Limpiando la carpeta de destino..."},
                    {"RestoreCompleted", "Restauración completada."},
                    {"ThankYou", "Gracias por usar EasySave. ¡Adiós!"},
                    {"SelectLanguage", "Seleccione el idioma / Select language:"},
                    {"LanguageChangedFr", "El idioma ha sido cambiado a francés."},
                    {"LanguageChangedEn", "El idioma ha sido cambiado a inglés."},
                    {"LanguageChangedEs", "El idioma ha sido cambiado a español."},
                    {"LanguageChangedIt", "El idioma ha sido cambiado a italiano."},
                    {"SelectRestoreType", "Seleccione el tipo de restauración:"},
                    {"CompleteRestore", "Restauración completa"},
                    {"DifferentialRestore", "Restauración diferencial"},
                    {"InvalidRestoreType", "Tipo de restauración no válido."},
                }
            },
            { "it", new Dictionary<string, string>()
                {
                    {"MenuSeparator", "--------------------------------------"},
                    {"MainMenuTitle", "          Menu Principale di EasySave          "},
                    {"StartBackup", "Avvia un backup"},
                    {"ListBackups", "Elenca i backup"},
                    {"RestoreBackup", "Ripristina un backup"},
                    {"ChangeLanguage", "Cambia lingua"},
                    {"Exit", "Esci"},
                    {"ChooseOption", "Scegli un'opzione: "},
                    {"InvalidOption", "Opzione non valida, riprova."},
                    {"EnterBackupName", "Inserisci il nome del backup:"},
                    {"InvalidBackupName", "Nome del backup non valido."},
                    {"EnterFolderPath", "Inserisci il percorso della cartella da eseguire il backup:"},
                    {"FolderNotExist", "La cartella specificata non esiste. Backup annullato."},
                    {"StartingBackup", "Avvio del backup '{0}'..."},
                    {"BackupCompleted", "Backup completato."},
                    {"FileCopied", "File copiato: {0} -> {1}"},
                    {"FileCopyError", "Errore nella copia del file '{0}': {1}"},
                    {"NoBackups", "Nessun backup è stato effettuato."},
                    {"BackupName", "Nome: {0}, Data: {1}"},
                    {"EnterBackupToRestore", "Inserisci il nome del backup da ripristinare:"},
                    {"BackupNotExist", "Il backup specificato non esiste."},
                    {"EnterRestoreDestination", "Inserisci il percorso di destinazione per il ripristino (es 1-n):"},
                    {"InvalidDestination", "Percorso di destinazione non valido. Ripristino annullato."},
                    {"CleaningDestination", "Pulizia della cartella di destinazione..."},
                    {"RestoreCompleted", "Ripristino completato."},
                    {"ThankYou", "Grazie per aver utilizzato EasySave. Arrivederci!"},
                    {"SelectLanguage", "Seleziona la lingua / Select language:"},
                    {"LanguageChangedFr", "La lingua è stata cambiata in francese."},
                    {"LanguageChangedEn", "La lingua è stata cambiata in inglese."},
                    {"LanguageChangedEs", "La lingua è stata cambiata in spagnolo."},
                    {"LanguageChangedIt", "La lingua è stata cambiata in italiano."},
                    {"SelectRestoreType", "Seleziona il tipo di ripristino:"},
                    {"CompleteRestore", "Ripristino completo"},
                    {"DifferentialRestore", "Ripristino differenziale"},
                    {"InvalidRestoreType", "Tipo di ripristino non valido."},
                }
            }
        };

        public void SelectionnerLangue()
        {
            Console.WriteLine("======================================");
            Console.WriteLine("=          EasySave - Langue         =");
            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("Sélectionnez la langue / Select language :");
            Console.WriteLine("fr -> Français");
            Console.WriteLine("en -> English");
            Console.WriteLine("es -> Espagnol");
            Console.WriteLine("it -> Italien");
            Console.Write("Votre choix / Your choice: ");
            string? choixLangue = Console.ReadLine()?.Trim().ToLower();

            if (choixLangue == "fr")
            {
                langue = "fr";
                Console.WriteLine(ObtenirMessage("LanguageChangedFr"));
            }
            else if (choixLangue == "en")
            {
                langue = "en";
                Console.WriteLine(ObtenirMessage("LanguageChangedEn"));
            }
            else if (choixLangue == "es")
            {
                langue = "es";
                Console.WriteLine(ObtenirMessage("LanguageChangedEs"));
            }
            else if (choixLangue == "it")
            {
                langue = "it";
                Console.WriteLine(ObtenirMessage("LanguageChangedIt"));
            }
            else
            {
                Console.WriteLine(ObtenirMessage("InvalidOption"));
                SelectionnerLangue();
            }
        }

        public string ObtenirMessage(string cle)
        {
            if (messages[langue].ContainsKey(cle))
            {
                return messages[langue][cle];
            }
            else
            {
                return "Message non trouvé";
            }
        }

        public void DemarrerSauvegarde()
        {
            long totalSize = 0;
            long totalFiles = 0;
            Console.WriteLine("\n" + ObtenirMessage("MenuSeparator"));
            Console.WriteLine(ObtenirMessage("StartBackup"));
            Console.WriteLine(ObtenirMessage("MenuSeparator"));

            Console.Write(ObtenirMessage("EnterBackupName") + " ");
            string? nomSauvegarde = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(nomSauvegarde))
            {
                Console.WriteLine(ObtenirMessage("InvalidBackupName"));
                return;
            }

            Console.Write(ObtenirMessage("EnterFolderPath") + " ");
            string? cheminDossier = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(cheminDossier) || !Directory.Exists(cheminDossier))
            {
                Console.WriteLine(ObtenirMessage("FolderNotExist"));
                return;
            }

            string cheminDestination = Path.Combine("Sauvegardes", nomSauvegarde);
            if (!Directory.Exists(cheminDestination))
            {
                Directory.CreateDirectory(cheminDestination);
            }

            Console.WriteLine(string.Format(ObtenirMessage("StartingBackup"), nomSauvegarde));

            var stopwatch = Stopwatch.StartNew();

            CopierDossier(cheminDossier, cheminDestination, ref totalSize, ref totalFiles);
            Console.WriteLine($"Total Size: {totalSize} bytes");
            Console.WriteLine($"Total Files: {totalFiles}");

            stopwatch.Stop();
            string elapsedTime = $"{stopwatch.ElapsedMilliseconds} ms";

            Program.historic.Backup(nomSauvegarde, cheminDossier, cheminDestination, elapsedTime, $"{totalSize} bytes");

            Console.WriteLine(ObtenirMessage("BackupCompleted"));
        }

        private void CopierDossier(string sourceDir, string destinationDir, ref long totalSize, ref long totalFiles)
        {
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                try
                {
                    File.Copy(file, destFile, true);
                    Console.WriteLine(string.Format(ObtenirMessage("FileCopied"), file, destFile));
                    FileInfo fileInfo = new FileInfo(file);
                    totalSize += fileInfo.Length;
                    totalFiles++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format(ObtenirMessage("FileCopyError"), file, ex.Message));
                }
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string destDirPath = Path.Combine(destinationDir, Path.GetFileName(dir));
                CopierDossier(dir, destDirPath, ref totalSize, ref totalFiles);
            }
        }

        public void ListerSauvegardes()
        {
            Console.WriteLine("\n" + ObtenirMessage("MenuSeparator"));
            Console.WriteLine(ObtenirMessage("ListBackups"));
            Console.WriteLine(ObtenirMessage("MenuSeparator"));

            if (File.Exists(Historic.jsonfile))
            {
                Program.historic.DisplayLog();
            }
            else
            {
                Console.WriteLine(ObtenirMessage("NoBackups"));
            }

            Console.WriteLine(ObtenirMessage("MenuSeparator"));
        }

        public void RestaurerSauvegarde()
        {
            Console.WriteLine("\n" + ObtenirMessage("MenuSeparator"));
            Console.WriteLine(ObtenirMessage("RestoreBackup"));
            Console.WriteLine(ObtenirMessage("MenuSeparator"));

            // Choix du type de restauration
            Console.WriteLine(ObtenirMessage("SelectRestoreType"));
            Console.WriteLine("1. " + ObtenirMessage("CompleteRestore"));
            Console.WriteLine("2. " + ObtenirMessage("DifferentialRestore"));
            Console.Write(ObtenirMessage("ChooseOption"));
            string? restoreTypeChoice = Console.ReadLine()?.Trim();
            bool isDifferential;
            if (restoreTypeChoice == "1")
            {
                isDifferential = false;
            }
            else if (restoreTypeChoice == "2")
            {
                isDifferential = true;
            }
            else
            {
                Console.WriteLine(ObtenirMessage("InvalidRestoreType"));
                return;
            }

            string backupsRoot = "Sauvegardes";
            if (!Directory.Exists(backupsRoot))
            {
                Console.WriteLine(ObtenirMessage("NoBackups"));
                return;
            }
            string[] backupDirs = Directory.GetDirectories(backupsRoot);
            if (backupDirs.Length == 0)
            {
                Console.WriteLine(ObtenirMessage("NoBackups"));
                return;
            }
            Array.Sort(backupDirs);
            Console.WriteLine("Sauvegardes disponibles :");
            for (int i = 0; i < backupDirs.Length; i++)
            {
                string backupName = Path.GetFileName(backupDirs[i]);
                Console.WriteLine($"{i + 1}. {backupName}");
            }

            Console.Write(ObtenirMessage("EnterBackupToRestore") + " ");
            string? input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine(ObtenirMessage("InvalidBackupName"));
                return;
            }

            // Si plusieurs sauvegardes sont sélectionnées (ex: "1-3" ou "1;2")
            if (input.Contains("-") || input.Contains(";"))
            {
                List<int> indices = ParseIndices(input);
                if (indices.Count == 0)
                {
                    Console.WriteLine(ObtenirMessage("InvalidBackupName"));
                    return;
                }
                foreach (int index in indices)
                {
                    if (index < 1 || index > backupDirs.Length)
                    {
                        Console.WriteLine($"Indice {index} invalide.");
                        continue;
                    }
                    string backupPath = backupDirs[index - 1];
                    string backupName = Path.GetFileName(backupPath);
                    Console.WriteLine($"Pour la sauvegarde '{backupName}', veuillez saisir le chemin de destination pour la restauration :");
                    string? destination = Console.ReadLine()?.Trim();
                    if (string.IsNullOrEmpty(destination))
                    {
                        Console.WriteLine(ObtenirMessage("InvalidDestination"));
                        continue;
                    }
                    if (isDifferential)
                    {
                        RestoreBackupDifferential(backupPath, destination, backupName);
                    }
                    else
                    {
                        RestoreBackup(backupPath, destination, backupName);
                    }
                }
                Console.WriteLine(ObtenirMessage("RestoreCompleted"));
                return;
            }
            else
            {
                string backupPath = Path.Combine(backupsRoot, input);
                if (!Directory.Exists(backupPath))
                {
                    Console.WriteLine(ObtenirMessage("BackupNotExist"));
                    return;
                }
                Console.Write(ObtenirMessage("EnterRestoreDestination") + " ");
                string? destination = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(destination))
                {
                    Console.WriteLine(ObtenirMessage("InvalidDestination"));
                    return;
                }
                if (isDifferential)
                {
                    RestoreBackupDifferential(backupPath, destination, Path.GetFileName(backupPath));
                }
                else
                {
                    RestoreBackup(backupPath, destination, Path.GetFileName(backupPath));
                }
                Console.WriteLine(ObtenirMessage("RestoreCompleted"));
            }
        }

        private void RestoreBackup(string backupPath, string destination, string nomSauvegarde)
        {
            long totalSize = 0;
            long totalFiles = 0;
            if (Directory.Exists(destination))
            {
                try
                {
                    Console.WriteLine(ObtenirMessage("CleaningDestination"));
                    DirectoryInfo directory = new DirectoryInfo(destination);
                    foreach (FileInfo file in directory.GetFiles())
                        file.Delete();
                    foreach (DirectoryInfo dir in directory.GetDirectories())
                        ForceDeleteDirectory(dir.FullName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format(ObtenirMessage("FileCopyError"), destination, ex.Message));
                    return;
                }
            }
            else
            {
                Directory.CreateDirectory(destination);
            }
            Console.WriteLine($"Restauration de la sauvegarde '{Path.GetFileName(backupPath)}' vers '{destination}'...");

            var stopwatch = Stopwatch.StartNew();

            CopierDossier(backupPath, destination, ref totalSize, ref totalFiles);
            Console.WriteLine($"Total Size: {totalSize} bytes");
            Console.WriteLine($"Total Files: {totalFiles}");

            stopwatch.Stop();
            string elapsedTime = $"{stopwatch.ElapsedMilliseconds} ms";

            Program.historic.Restore(nomSauvegarde, backupPath, destination, elapsedTime, $"{totalSize} bytes");
            Console.WriteLine(ObtenirMessage("RestoreCompleted"));
        }

        private void RestoreBackupDifferential(string backupPath, string destination, string nomSauvegarde)
        {
            long totalSize = 0;
            long totalFiles = 0;
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }
            Console.WriteLine($"Restauration différentielle de la sauvegarde '{Path.GetFileName(backupPath)}' vers '{destination}'...");
            var stopwatch = Stopwatch.StartNew();

            CopierDossierDifferential(backupPath, destination, ref totalSize, ref totalFiles);

            stopwatch.Stop();
            string elapsedTime = $"{stopwatch.ElapsedMilliseconds} ms";
            Console.WriteLine($"Total Size: {totalSize} bytes");
            Console.WriteLine($"Total Files: {totalFiles}");

            Program.historic.Restore(nomSauvegarde, backupPath, destination, elapsedTime, $"{totalSize} bytes");
        }

        private void CopierDossierDifferential(string sourceDir, string destinationDir, ref long totalSize, ref long totalFiles)
        {
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                bool copyFile = true;
                if (File.Exists(destFile))
                {
                    FileInfo sourceInfo = new FileInfo(file);
                    FileInfo destInfo = new FileInfo(destFile);
                    if (sourceInfo.Length == destInfo.Length)
                    {
                        copyFile = false;
                    }
                }
                if (copyFile)
                {
                    try
                    {
                        File.Copy(file, destFile, true);
                        Console.WriteLine(string.Format(ObtenirMessage("FileCopied"), file, destFile));
                        FileInfo fileInfo = new FileInfo(file);
                        totalSize += fileInfo.Length;
                        totalFiles++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format(ObtenirMessage("FileCopyError"), file, ex.Message));
                    }
                }
            }
            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string destDirPath = Path.Combine(destinationDir, Path.GetFileName(dir));
                CopierDossierDifferential(dir, destDirPath, ref totalSize, ref totalFiles);
            }
        }

        public static List<int> ParseIndices(string input)
        {
            var indices = new List<int>();
            input = input.Replace(" ", "");
            string[] parts = input.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (part.Contains("-"))
                {
                    string[] rangeParts = part.Split('-');
                    if (rangeParts.Length == 2 &&
                        int.TryParse(rangeParts[0], out int start) &&
                        int.TryParse(rangeParts[1], out int end))
                    {
                        for (int i = start; i <= end; i++)
                        {
                            if (!indices.Contains(i))
                                indices.Add(i);
                        }
                    }
                }
                else
                {
                    if (int.TryParse(part, out int num))
                    {
                        if (!indices.Contains(num))
                            indices.Add(num);
                    }
                }
            }
            return indices;
        }

        static void ForceDeleteDirectory(string path)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C rmdir /s /q \"{path}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();
                    Console.WriteLine(process.StandardOutput.ReadToEnd());
                }

                Console.WriteLine("Le répertoire a été supprimé avec succès.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Une erreur est survenue lors de la suppression forcée du répertoire : " + e.Message);
            }
        }
    }
}
