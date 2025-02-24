using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using Log;

namespace Model;

// Conteneur de données pour une sauvegarde
public class BackupData
{
    public required string Source { get; set; }
    public required string Target { get; set; }
    public required string Name { get; set; }
    public IBackupStrategy Strategy { get; set; }
    public int BackupId { get; set; }
}

public interface IBackupStrategy
{
    Task<(long totalSize, long totalFiles)> ExecuteBackup(string source, string target);
}


public class CompleteBackup : IBackupStrategy
{
    public async Task<(long totalSize, long totalFiles)> ExecuteBackup(string source, string target)
    {
        // Appel de la méthode asynchrone CopyDirectoryAsync
        return await Copie.Instance.CopyDirectoryAsync(source, target, false);
    }
}


public class DifferentialBackup : IBackupStrategy
{
    public async Task<(long totalSize, long totalFiles)> ExecuteBackup(string source, string target)
    {
        // Appel de la méthode asynchrone CopyDirectoryAsync avec différentiel
        return await Copie.Instance.CopyDirectoryAsync(source, target, true);
    }
}



// Fabrique de stratégies de sauvegarde
public class BackupFactory
{
    public IBackupStrategy CreateBackupStrategy(string type)
    {
        MessageBox.Show($"DEBUG: TypeSauvegarde après conversion 123 = {type}");
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

    public BackupData CreateBackupData(string source, string target, string name, IBackupStrategy strategy)
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

    public BackupData? GetBackup(int backupId)
    {
        return backupList.FirstOrDefault(b => b.BackupId == backupId);
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

    // Démarre une sauvegarde et renvoie un résultat (aucun affichage dans le modèle)
    public async Task StartBackup(string source, string target, string name, string type)
    {
        var strategy = backupFactory.CreateBackupStrategy(type);
        var backupData = backupManager.CreateBackupData(source, target, name, strategy);
        string sauvegarde = Path.Combine("Sauvegardes", name);

        var stopwatch = Stopwatch.StartNew();

        // Attente asynchrone pour récupérer les résultats de la méthode ExecuteBackup
        var (totalSize, totalFiles) = await strategy.ExecuteBackup(source, sauvegarde);

        stopwatch.Stop();

        Historic.Backup(name, source, target, stopwatch.ElapsedMilliseconds.ToString(), totalSize.ToString());
    }

    // Restaure une sauvegarde et renvoie le résultat
    public async Task RestoreBackup(int backupId, string restoreDestination, bool differential)
    {
        var backupData = backupManager.GetBackup(backupId);

        // Vérification si le backup existe
        if (backupData == null)
        {
            throw new ArgumentException("Backup non trouvé");
        }

        string sauvegarde = Path.Combine("Sauvegarde", backupData.Name);

        // Choix de la stratégie pour la restauration
        IBackupStrategy strategy = differential ? new DifferentialBackup() : new CompleteBackup();

        var stopwatch = Stopwatch.StartNew();

        // Appel asynchrone de la méthode ExecuteBackup pour effectuer la restauration
        var (totalSize, totalFiles) = await strategy.ExecuteBackup(sauvegarde, restoreDestination);

        stopwatch.Stop();

        // Enregistrement de l'historique après la restauration
        Historic.Backup(backupData.Name, backupData.Source, backupData.Target, stopwatch.ElapsedMilliseconds.ToString(), totalSize.ToString());
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
//    public long TotalSize { get; set; }
//    public long TotalFiles { get; set; }
//    public long ElapsedTimeMs { get; set; }
//}