using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Model
{
    public class Copie
    {
        private static Copie? _instance;
        private static readonly object _lock = new object();
        private volatile bool Interruption = false;

        private Copie() { }

        public string BusinessSoftwarePath { get; set; } = string.Empty;

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

        public bool IsBusinessSoftwareRunning()
        {
            if (string.IsNullOrWhiteSpace(BusinessSoftwarePath))
                return false;

            string exeName = Path.GetFileNameWithoutExtension(BusinessSoftwarePath);

            return Process.GetProcessesByName(exeName).Any();
        }

        public async Task<(long transferredSize, long transferredFiles)> CopyDirectoryAsync(string sourceDir, string destinationDir, bool useDifferential, Action<long, long> updateProgress)
        {
            return await Task.Run(async () =>
            {
                if (IsBusinessSoftwareRunning())
                {
                    MessageBox.Show("Erreur : Le logiciel métier est en cours d'exécution. Veuillez le fermer pour continuer.",
                                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

                    throw new Exception("Logiciel métier en cours d'exécution.");
                }

                // On attend tant que le logiciel métier est actif ou que l'interruption est activée
                while (IsBusinessSoftwareRunning() || Interruption)
                {
                    await Task.Delay(200); // Attend 200 ms avant de vérifier à nouveau
                }

                return useDifferential
                    ? CopyDirectoryDifferential(sourceDir, destinationDir, updateProgress)
                    : CopyDirectory(sourceDir, destinationDir, updateProgress);
            });
        }

        public (long transferredSize, long transferredFiles) CopyDirectory(string sourceDir, string destinationDir, Action<long, long> updateProgress)
        {
            long transferredSize = 0;
            long transferredFiles = 0;

            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                try
                {
                    File.Copy(file, destFile, true);
                    FileInfo fileInfo = new FileInfo(file);
                    transferredSize += fileInfo.Length;
                    transferredFiles++;

                    // Mise à jour du progrès
                    updateProgress(transferredSize, transferredFiles);

                    // Pause de 2 secondes après chaque copie
                    //Thread.Sleep(20000); // Attend 2 secondes
                }
                catch (Exception)
                {
                    throw; // Laissez l'exception remonter
                }
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string destDirPath = Path.Combine(destinationDir, Path.GetFileName(dir));
                var result = CopyDirectory(dir, destDirPath, updateProgress);
                transferredSize += result.transferredSize;
                transferredFiles += result.transferredFiles;
            }

            return (transferredSize, transferredFiles);
        }


        public (long transferredSize, long transferredFiles) CopyDirectoryDifferential(string sourceDir, string destinationDir, Action<long, long> updateProgress)
        {
            long transferredSize = 0;
            long transferredFiles = 0;

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
                        transferredSize += fileInfo.Length;
                        transferredFiles++;

                        // Mise à jour du progrès
                        updateProgress(transferredSize, transferredFiles);
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
                var result = CopyDirectoryDifferential(dir, destDirPath, updateProgress);
                transferredSize += result.transferredSize;
                transferredFiles += result.transferredFiles;
            }

            return (transferredSize, transferredFiles);
        }

        //private bool IsDiscordRunning() => Process.GetProcessesByName("discord").Length > 0;
    }
}   
