using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Model
{
    public class Copie
    {
        private static Copie? _instance;
        private static readonly object _lock = new object();
        private volatile bool Interruption = false;

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

        public void Pause() => Interruption = true;
        public void Resume() => Interruption = false;

        public async Task<(long totalSize, long totalFiles)> CopyDirectoryAsync(string sourceDir, string destinationDir, bool useDifferential)
        {
            return await Task.Run(async () =>
            {
                while (IsDiscordRunning() || Interruption) // Pause si Discord est actif ou si Interruption est activée
                {
                    await Task.Delay(200); // Attend 200 ms avant de vérifier à nouveau
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
                Directory.CreateDirectory(destinationDir);

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
                    throw; // Laissez l'exception remonter
                }
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string destDirPath = Path.Combine(destinationDir, Path.GetFileName(dir));
                var result = CopyDirectory(dir, destDirPath);
                totalSize += result.totalSize;
                totalFiles += result.totalFiles;
            }

            return (totalSize, totalFiles);
        }

        public (long totalSize, long totalFiles) CopyDirectoryDifferential(string sourceDir, string destinationDir)
        {
            long totalSize = 0;
            long totalFiles = 0;

            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                bool copyFile = true;

                if (File.Exists(destFile))
                {
                    FileInfo sourceInfo = new FileInfo(file);
                    FileInfo destInfo = new FileInfo(destFile);
                    if (sourceInfo.Length == destInfo.Length)
                        copyFile = false;
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
                var result = CopyDirectoryDifferential(dir, destDirPath);
                totalSize += result.totalSize;
                totalFiles += result.totalFiles;
            }

            return (totalSize, totalFiles);
        }

        private bool IsDiscordRunning() => Process.GetProcessesByName("discord").Length > 0;
    }
}
