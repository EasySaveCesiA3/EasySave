using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;
using System.Xml.Linq;
using Log;

namespace Model;

// Conteneur de données pour une sauvegarde
public class BackupData
{
    public required string Source { get; set; }
    public required string Target { get; set; }
    public required string Name { get; set; }
    public string Strategy { get; set; }
    public int BackupId { get; set; }
}

public interface IBackupStrategy
{
    Task<(long transferredSize, long transferredFiles)> ExecuteBackup(string source, string target, string name, string action);
}


public class CompleteBackup : IBackupStrategy
{
    public async Task<(long transferredSize, long transferredFiles)> ExecuteBackup(string source, string target, string name, string action)
    {
        string path = System.IO.Path.Combine("Etat", String.Concat(name,".txt"));

        // Créer le fichier initial
        Tools.CreateFile(path);
        long sourceSize = Tools.GetSize(source);
        long targetSize = 0;

        Tools.WriteBackupState(path, name, targetSize, sourceSize, action, "En cours");

        Action<long, long> updateProgress = (transferredSize, transferredFiles) =>
        {
            targetSize = transferredSize;
            Tools.WriteBackupState(path, name, targetSize, sourceSize, action, "En cours");
        };

        var result = await Copie.Instance.CopyDirectoryAsync(source, target, false, updateProgress);

        File.Delete(path);

        return result;
    }

}


public class DifferentialBackup : IBackupStrategy
{
    public async Task<(long transferredSize, long transferredFiles)> ExecuteBackup(string source, string target, string name, string action)
    {
        string path = System.IO.Path.Combine("Etat", String.Concat(name, ".txt"));

        // Créer le fichier initial
        Tools.CreateFile(path);
        long sourceSize = Tools.GetSize(source);
        long targetSize = 0;

        Tools.WriteBackupState(path, name, targetSize, sourceSize, action, "En cours");

        Action<long, long> updateProgress = (transferredSize, transferredFiles) =>
        {
            targetSize = transferredSize;
            Tools.WriteBackupState(path, name, targetSize, sourceSize, action, "En cours");
        };

        var result = await Copie.Instance.CopyDirectoryAsync(source, target, true, updateProgress);

        File.Delete(path);

        return result;
    }
}



// Fabrique de stratégies de sauvegarde
public class BackupFactory
{
    public IBackupStrategy CreateBackupStrategy(string type)
    {
        //MessageBox.Show($"DEBUG: TypeSauvegarde après conversion 123 = {type}");
        return type switch
        {
            "Complete" => new CompleteBackup(),
            "Differential" => new DifferentialBackup(),
            _ => throw new ArgumentException("Invalid backup type")
        };
    }
}

// Gestionnaire des sauvegardes en mémoire (ou éventuellement persistant)
public class BackupManager
{
    private List<BackupData> backupList = new List<BackupData>();
    private int nextBackupId = 1;

    public BackupData CreateBackupData(string source, string target, string name, string strategy)
    {
        var backup = new BackupData
        {
            Source = source,
            Target = target,
            Name = name,
            Strategy = strategy,
            BackupId = nextBackupId++
        };
        backupList.Add(backup);
        return backup;
    }

    public bool DeleteBackup(int backupId)
    {
        var backup = backupList.FirstOrDefault(b => b.BackupId == backupId);
        if (backup != null)
        {
            backupList.Remove(backup);
            return true;
        }
        return false;
    }

    public BackupData? GetBackup(string backupName)
    {
        var (logDictionaries, errorMessage) = Historic.LogsData();

        if (!string.IsNullOrEmpty(errorMessage) || logDictionaries == null || logDictionaries.Count == 0)
        {
            return null;
        }

        var backups = logDictionaries.Select(b => new BackupData
        {
            Name = b.TryGetValue("BackupName", out string? name) ? name : string.Empty,
            Source = b.TryGetValue("Source", out string? source) ? source : string.Empty,
            Target = b.TryGetValue("RestorationTarget", out string? target) ? target : string.Empty,
            Strategy = b.TryGetValue("StrategyType", out string? strategy) ? strategy : string.Empty,
        }).ToList();

        return backups.FirstOrDefault(b => b.Name == backupName);
    }


    public List<BackupData> ListBackups()
    {
        return new List<BackupData>(backupList);
    }
}

// Service métier orchestrant la création et la restauration de sauvegardes
public class BackupService
{
    private BackupManager backupManager = new BackupManager();
    private BackupFactory backupFactory = new BackupFactory();

    // Méthode de vérification du logiciel métier
    private bool IsBusinessSoftwareRunning()
    {
        return Model.Copie.Instance.IsBusinessSoftwareRunning();
    }

    private bool CheckForBusinessSoftware()
    {
        if (IsBusinessSoftwareRunning())
        {
            MessageBox.Show("Erreur : Le logiciel métier est en cours d'exécution. Veuillez le fermer avant de lancer l'opération.",
                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            return true;
        }
        return false;
    }


    // Démarre une sauvegarde et renvoie un résultat (aucun affichage dans le modèle)
    public async Task StartBackup(string source, string target, string name, string strategyType)
    {
        if (CheckForBusinessSoftware())
            return;

        var strategy = backupFactory.CreateBackupStrategy(strategyType);
        var backupData = backupManager.CreateBackupData(source, target, name, strategyType);
        string sauvegarde = System.IO.Path.Combine("Sauvegardes", name);


        var stopwatch = Stopwatch.StartNew();

        // Attente asynchrone pour récupérer les résultats de la méthode ExecuteBackup
        var (transferredSize, transferredFiles) = await strategy.ExecuteBackup(source, sauvegarde, name, "Sauvegarde");

        stopwatch.Stop();

        MessageBox.Show($"Sauvegarde '{name}' enregistrée avec succès !", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);

        Historic.Backup(name, source, target, stopwatch.ElapsedMilliseconds.ToString(), transferredSize.ToString(), strategyType);
    }

    // Restaure une sauvegarde et renvoie le résultat
    public async Task RestoreBackup(string backupName, string restoreDestination)
    {
        if (CheckForBusinessSoftware())
            return;

        var backupData = backupManager.GetBackup(backupName);
        bool differential = false;

        if (backupData == null)
        {
            throw new ArgumentException("Backup non trouvé");
        }

        string sauvegarde = System.IO.Path.Combine("Sauvegardes", backupData.Name);

        // Choix de la stratégie pour la restauration
        IBackupStrategy strategy = differential ? new DifferentialBackup() : new CompleteBackup();

        var stopwatch = Stopwatch.StartNew();


        var (transferredSize, transferredFiles) = await strategy.ExecuteBackup(sauvegarde, restoreDestination, backupData.Name, "Restauration");

        stopwatch.Stop();

        Historic.Backup(backupData.Name, backupData.Source, backupData.Target, stopwatch.ElapsedMilliseconds.ToString(), transferredSize.ToString(), backupData.Strategy);
    }


    public bool DeleteBackup(int backupId)
    {
        return backupManager.DeleteBackup(backupId);
    }

    public List<BackupData> ListBackups()
    {
        return backupManager.ListBackups();
    }
}

// Objet résultat pour encapsuler le résultat d’une sauvegarde ou restauration
//public class BackupResult
//{
//    public int BackupId { get; set; }
//    public string BackupName { get; set; }
//    public long transferredSize { get; set; }
//    public long transferredFiles { get; set; }
//    public long ElapsedTimeMs { get; set; }
//}