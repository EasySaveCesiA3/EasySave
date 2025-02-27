using Log;
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
                MessageBox.Show($"Un fichier ou un dossier nommé '{backupName}' existe déjà dans 'Sauvegardes'.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            backupService.StartBackup(source, target, backupName, type);
        }

        public static void runRestore(string backupName, string destination)
        {
            backupService.RestoreBackup(backupName, destination);
        }

        public static List<BackupData> listBackups()
        {
            return backupService.ListBackups();
        }

        public static bool deleteBackup(int backupId)
        {
            return backupService.DeleteBackup(backupId);
        }
        public static void openLog()
        {
            MessageBox.Show("Ouverture du journal d'événements");
            Historic.OpenLog();
        }

        public static bool logtype()
        {
            Historic.choix = !Historic.choix;
            return Historic.choix;
        }
    }


}