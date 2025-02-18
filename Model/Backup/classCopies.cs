using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Model; 

internal class Copie
{
    private static Copie? _instance;
    private LanguageManager languagemanager = LanguageManager.Instance;
    private static readonly object _lock = new object();


    // Constructeur privé pour empêcher l'instanciation externe
    private Copie() { }

    // Propriété pour récupérer l'instance unique
    internal static Copie Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new Copie();
                }
            }
            return _instance;
        }
    }


    //Model -> Classbackup
    internal void CopierDossier(string sourceDir, string destinationDir, ref long totalSize, ref long totalFiles)
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
                Console.WriteLine(string.Format(languagemanager.ObtenirMessage("FileCopied"), file, destFile));
                FileInfo fileInfo = new FileInfo(file);
                totalSize += fileInfo.Length;
                totalFiles++;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(languagemanager.ObtenirMessage("FileCopyError"), file, ex.Message));
            }
        }

        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            string destDirPath = Path.Combine(destinationDir, Path.GetFileName(dir));
            CopierDossier(dir, destDirPath, ref totalSize, ref totalFiles);
        }
    }

    //Model -> class 
    internal void CopierDossierDifferential(string sourceDir, string destinationDir, ref long totalSize, ref long totalFiles)
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
                    Console.WriteLine(string.Format(languagemanager.ObtenirMessage("FileCopied"), file, destFile));
                    FileInfo fileInfo = new FileInfo(file);
                    totalSize += fileInfo.Length;
                    totalFiles++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format(languagemanager.ObtenirMessage("FileCopyError"), file, ex.Message));
                }
            }
        }
        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            string destDirPath = Path.Combine(destinationDir, Path.GetFileName(dir));
            CopierDossierDifferential(dir, destDirPath, ref totalSize, ref totalFiles);
        }
    }
}

