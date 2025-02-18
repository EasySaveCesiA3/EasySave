namespace Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LanguageManager
{
    private static LanguageManager? _instance;
    private static readonly object _lock = new object();

    // Constructeur privé pour empêcher l'instanciation externe
    private LanguageManager() { }

    // Propriété pour récupérer l'instance unique
    public static LanguageManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new LanguageManager();
                }
            }
            return _instance;
        }
    }

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


    private string currentLanguage { get;set; }

    //supp ou use in refacto
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
            currentLanguage = "fr";
            Console.WriteLine(ObtenirMessage("LanguageChangedFr"));
        }
        else if (choixLangue == "en")
        {
            currentLanguage = "en";
            Console.WriteLine(ObtenirMessage("LanguageChangedEn"));
        }
        else if (choixLangue == "es")
        {
            currentLanguage = "es";
            Console.WriteLine(ObtenirMessage("LanguageChangedEs"));
        }
        else if (choixLangue == "it")
        {
            currentLanguage = "it";
            Console.WriteLine(ObtenirMessage("LanguageChangedIt"));
        }
        else
        {
            Console.WriteLine(ObtenirMessage("InvalidOption"));
            SelectionnerLangue();
        }
    }

    internal string ObtenirMessage(string cle)
    {
        if (messages[currentLanguage].ContainsKey(cle))
        {
            return messages[currentLanguage][cle];
        }
        else
        {
            return "Message non trouvé";
        }
    }
}

public enum LanguageData
{
    FR,
    EN,
    ES,
    DE
}