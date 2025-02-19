using System;
using System.Diagnostics;
using Logs;
//using ViewModel;
using Main;
using System.IO;

namespace Model;

public class ClassBackupService
{
    private static ClassBackupService? _instance;
    private static readonly object _lock = new object();

    // Constructeur privé pour empêcher l'instanciation externe
    private ClassBackupService() { }

    // Propriété pour récupérer l'instance unique
    public static ClassBackupService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new ClassBackupService();
                }
            }
            return _instance;
        }
    }

    private LanguageManager languagemanager = LanguageManager.Instance;
    private Copie copie = Copie.Instance;

    internal void DeleteDirectory(string path)
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

    public void DemarrerSauvegarde()
    {
        long totalSize = 0;
        long totalFiles = 0;
        Console.WriteLine("\n" + languagemanager.ObtenirMessage("MenuSeparator"));
        Console.WriteLine(languagemanager.ObtenirMessage("StartBackup"));
        Console.WriteLine(languagemanager.ObtenirMessage("MenuSeparator"));

        Console.Write(languagemanager.ObtenirMessage("EnterBackupName") + " ");
        string? nomSauvegarde = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(nomSauvegarde))
        {
            Console.WriteLine(languagemanager.ObtenirMessage("InvalidBackupName"));
            return;
        }

        string cheminDestination = Path.Combine("Sauvegardes", nomSauvegarde);
        if (Directory.Exists(cheminDestination))
        {
            Console.WriteLine("Cette sauvegarde existe déjà, veuillez choisir un nom différent");
            return;
        }

        Console.Write(languagemanager.ObtenirMessage("EnterFolderPath") + " ");
        string? cheminDossier = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(cheminDossier) || !Directory.Exists(cheminDossier))
        {
            Console.WriteLine(languagemanager.ObtenirMessage("FolderNotExist"));
            return;
        }

        Directory.CreateDirectory(cheminDestination);
        Console.WriteLine(string.Format(languagemanager.ObtenirMessage("StartingBackup"), nomSauvegarde));

        var stopwatch = Stopwatch.StartNew();

        copie.CopierDossier(cheminDossier, cheminDestination, ref totalSize, ref totalFiles);
        Console.WriteLine($"Total Size: {totalSize} bytes");
        Console.WriteLine($"Total Files: {totalFiles}");

        stopwatch.Stop();
        string elapsedTime = $"{stopwatch.ElapsedMilliseconds} ms";

        EasySaveApp.MainWindow.historic.Backup(nomSauvegarde, cheminDossier, cheminDestination, elapsedTime, $"{totalSize} bytes");

        Console.WriteLine(languagemanager.ObtenirMessage("BackupCompleted"));
    }

    public (string, string[]) ListerSauvegardesNoms()
    {
        string backupsRoot = "Sauvegardes";
        if (!Directory.Exists(backupsRoot))
        {
            Console.WriteLine(languagemanager.ObtenirMessage("NoBackups"));
        }

        string[] backupDirs = Directory.GetDirectories(backupsRoot);
        if (backupDirs.Length == 0)
        {
            Console.WriteLine(languagemanager.ObtenirMessage("NoBackups"));
        }

        Array.Sort(backupDirs);
        Console.WriteLine("Sauvegardes disponibles :");
        for (int i = 0; i < backupDirs.Length; i++)
        {
            string backupName = Path.GetFileName(backupDirs[i]);
            Console.WriteLine($"{i + 1}. {backupName}");
        }

        return (backupsRoot, backupDirs);
    }

    
    public void Restaurer()
    {
        Console.WriteLine("\n" + languagemanager.ObtenirMessage("MenuSeparator"));
        Console.WriteLine(languagemanager.ObtenirMessage("RestoreBackup"));
        Console.WriteLine(languagemanager.ObtenirMessage("MenuSeparator"));

        // Choix du type de restauration
        Console.WriteLine(languagemanager.ObtenirMessage("SelectRestoreType"));
        Console.WriteLine("1. " + languagemanager.ObtenirMessage("CompleteRestore"));
        Console.WriteLine("2. " + languagemanager.ObtenirMessage("DifferentialRestore"));
        Console.Write(languagemanager.ObtenirMessage("ChooseOption"));
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
            Console.WriteLine(languagemanager.ObtenirMessage("InvalidRestoreType"));
            return;
        }

        (string backupsRoot, string[] backupDirs) = ListerSauvegardesNoms();

        Console.Write(languagemanager.ObtenirMessage("EnterBackupToRestore") + " ");
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(input))
        {
            Console.WriteLine(languagemanager.ObtenirMessage("InvalidBackupName"));
            return;
        }

        string backupPath = Path.Combine(backupsRoot, input);
        if (!Directory.Exists(backupPath))
        {
            Console.WriteLine(languagemanager.ObtenirMessage("BackupNotExist"));
            return;
        }

        Console.Write(languagemanager.ObtenirMessage("EnterRestoreDestination") + " ");
        string? destination = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(destination))
        {
            Console.WriteLine(languagemanager.ObtenirMessage("InvalidDestination"));
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

        Console.WriteLine(languagemanager.ObtenirMessage("RestoreCompleted"));
    }

    internal void RestoreBackup(string backupPath, string destination, string nomSauvegarde)
    {
        long totalSize = 0;
        long totalFiles = 0;

        if (Directory.Exists(destination))
        {
            try
            {
                Console.WriteLine(languagemanager.ObtenirMessage("CleaningDestination"));
                DeleteDirectory(destination);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(languagemanager.ObtenirMessage("FileCopyError"), destination, ex.Message));
                return;
            }
        }
        else
        {
            Directory.CreateDirectory(destination);
        }

        Console.WriteLine($"Restauration de la sauvegarde '{Path.GetFileName(backupPath)}' vers '{destination}'...");

        var stopwatch = Stopwatch.StartNew();
        copie.CopierDossier(backupPath, destination, ref totalSize, ref totalFiles);

        Console.WriteLine($"Total Size: {totalSize} bytes");
        Console.WriteLine($"Total Files: {totalFiles}");

        stopwatch.Stop();
        string elapsedTime = $"{stopwatch.ElapsedMilliseconds} ms";

        EasySaveApp.MainWindow.historic.Restore(nomSauvegarde, backupPath, destination, elapsedTime, $"{totalSize} bytes");
        Console.WriteLine(languagemanager.ObtenirMessage("RestoreCompleted"));
    }

    //Model
    private void RestoreBackupDifferential(string backupPath, string destination, string nomSauvegarde)
    {
        long totalSize = 0;
        long totalFiles = 0;
        if (!Directory.Exists(destination))
        {
            Directory.CreateDirectory(destination);
        }
        Console.WriteLine($"Restauration diffÃ©rentielle de la sauvegarde '{Path.GetFileName(backupPath)}' vers '{destination}'...");
        var stopwatch = Stopwatch.StartNew();

        copie.CopierDossierDifferential(backupPath, destination, ref totalSize, ref totalFiles);

        stopwatch.Stop();
        string elapsedTime = $"{stopwatch.ElapsedMilliseconds} ms";
        Console.WriteLine($"Total Size: {totalSize} bytes");
        Console.WriteLine($"Total Files: {totalFiles}");

        EasySaveApp.MainWindow.historic.Restore(nomSauvegarde, backupPath, destination, elapsedTime, $"{totalSize} bytes");
    }
}

