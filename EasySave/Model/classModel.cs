using System.Collections.Generic;

namespace Model
{
    public class classModel
    {
        private static BackupService backupService = new BackupService();

        // D�marrer une sauvegarde ; renvoie un objet r�sultat � l�interface
        public static BackupResult runBackup(string source, string target, string backupName, string type)
        {
            return backupService.StartBackup(source, target, backupName, type);
        }

        // Restaurer une sauvegarde ; differential indique le type de restauration
        public static BackupResult runRestore(int backupId, string destination, bool differential)
        {
            return backupService.RestoreBackup(backupId, destination, differential);
        }

        public static List<BackupData> listBackups()
        {
            return backupService.ListBackups();
        }

        public static bool deleteBackup(int backupId)
        {
            return backupService.DeleteBackup(backupId);
        }

        // Vous pourrez ajouter d�autres m�thodes (modifyBackup, addBackup, etc.) en suivant ce mod�le.
    }
}