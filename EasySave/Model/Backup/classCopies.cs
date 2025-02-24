using System;
using System.Diagnostics;
using System.IO;

namespace Model
{
    public class Copie
    {
        private static Copie? _instance;
        private static readonly object _lock = new object();

        private Copie() { }

        public static Copie Instance
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

        public async Task<(long totalSize, long totalFiles)> CopyDirectoryAsync(string sourceDir, string destinationDir, bool useDifferential)
        {
            return await Task.Run(async () =>
            {
                while (IsDiscordRunning()) // Tant que Discord est actif, on attend
                {
                    await Task.Delay(2000);
                }

                return useDifferential
                    ? CopyDirectoryDifferential(sourceDir, destinationDir)
                    : CopyDirectory(sourceDir, destinationDir);
            });
        }


        public (long totalSize, long totalFiles) CopyDirectory(string sourceDir, string destinationDir)
        {
            long totalSize = 0;
            long totalFiles = 0;

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
                    FileInfo fileInfo = new FileInfo(file);
                    totalSize += fileInfo.Length;
                    totalFiles++;
                }
                catch (Exception)
                {
                    throw; // Laissez l’exception remonter pour être gérée par l’interface
                }
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string destDirPath = Path.Combine(destinationDir, Path.GetFileName(dir));
                var result = CopyDirectory(dir, destDirPath); // Appel récursif
                totalSize += result.totalSize;
                totalFiles += result.totalFiles;
            }

            return (totalSize, totalFiles); // Retourne le tuple
        }


        public (long totalSize, long totalFiles) CopyDirectoryDifferential(string sourceDir, string destinationDir)
        {
            long totalSize = 0;
            long totalFiles = 0;

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
                        FileInfo fileInfo = new FileInfo(file);
                        totalSize += fileInfo.Length;
                        totalFiles++;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string destDirPath = Path.Combine(destinationDir, Path.GetFileName(dir));
                var result = CopyDirectoryDifferential(dir, destDirPath); // Appel récursif
                totalSize += result.totalSize;
                totalFiles += result.totalFiles;
            }

            return (totalSize, totalFiles); // Retourne le tuple
        }


        private bool IsDiscordRunning()
        {
            //return Process.GetProcessesByName("discord").Length > 0;
            return false;
        }
    }
}