namespace Model;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public class BackupManager
{
    private List<BackupData> backupList { get; set; }

    public void createBackup(string source, string target)
    {
        Console.WriteLine("ouiIIIIII");
    }

    public void deleteBackup(int ID_Backup)
    {
        Console.WriteLine("ouiIIIIII");
    }

    public BackupData getBackup(int ID_Backup)
    {
        Console.WriteLine("ouiIIIIII");
        return new BackupData();
    }
}

public class BackupService
{
    private LanguageManager languagemanager = LanguageManager.Instance;
    private BackupManager backupManager { get; set; }


    public BackupService()
    {
        backupManager = new BackupManager();
    }

    public void createBackup(string source, string target, string type)
    {
        BackupFactory factory = new BackupFactory();
        IBackupStrategy strategy = factory.createBackup(type);
        backupManager.createBackup(source, target);
    }

    public void execute(int ID_Backup)
    {
        backupManager.deleteBackup(ID_Backup);
    }
}

public class BackupData
{
    private string source { get; set; }
    private string target { get; set; }
    private string name { get; set; }
    private IBackupStrategy strategy { get; set; }
    private int ID_Backup { get; set; }
}

public interface IBackupStrategy
{
    void ExecuteBackup(string source, string target);
}

public class CompleteBackup : IBackupStrategy
{
    public void ExecuteBackup(string source, string target)
    {
        Console.WriteLine("Complete Backup");
    }
}

public class DifferentialBackup : IBackupStrategy
{
    public void ExecuteBackup(string source, string target)
    {
        Console.WriteLine("Differential Backup");
    }
}

public class BackupFactory
{
    public IBackupStrategy createBackup(string type)
    {
        switch (type)
        {
            case "Complete":
                return new CompleteBackup();
            case "Differential":
                return new DifferentialBackup();
            default:
                return null;
        }
    }
}
