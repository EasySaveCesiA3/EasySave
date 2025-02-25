using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Model
{
    public class classModel
    {
        private static BackupService backupService = new BackupService();

        public static void runBackup(string source, string target, string backupName, string type)
        {
            string sauvegardesPath = Path.Combine(Directory.GetCurrentDirectory(), "Sauvegardes");
            string backupPath = Path.Combine(sauvegardesPath, backupName);

            if (File.Exists(backupPath) || Directory.Exists(backupPath))
            {
                MessageBox.Show($"Un fichier ou un dossier nommé '{backupName}' existe déjà dans 'Sauvegardes'.","Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            backupService.StartBackup(source, target, backupName, type);
            MessageBox.Show($"Sauvegarde '{backupName}' enregistrée avec succès !", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        public static void runRestore(string backupName, string destination)
        {
            backupService.RestoreBackup(backupName, destination);
        }

        public static List<BackupData> listBackups()
        {
            return backupService.ListBackups();
        }

        public static bool DeleteBackup(int backupId)
        {
            return backupService.DeleteBackup(backupId);
        }

        // Vous pourrez ajouter d’autres méthodes (modifyBackup, addBackup, etc.) en suivant ce modèle.
    }
}