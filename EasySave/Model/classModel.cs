using System.Collections.Generic;

namespace Model
{
    public class classModel
    {
        private static BackupService backupService = new BackupService();

        public static void runBackup(string source, string target, string backupName, string type)
        {
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

        // Vous pourrez ajouter d’autres méthodes (modifyBackup, addBackup, etc.) en suivant ce modèle.
    }
}