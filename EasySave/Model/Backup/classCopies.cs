using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Model
{
    public class Copie
    {
        private static Copie? _instance;
        private static readonly object _lock = new object();

        // Indique si une interruption est demandée
        public volatile bool Interruption = false;
        // Indique si un arrêt est demandé
        public volatile static bool Arret = false;

        // Sémaphore pour empêcher le transfert parallèle de plusieurs fichiers "gros"
        private SemaphoreSlim _largeFileSemaphore = new SemaphoreSlim(1, 1);
        // Seuil configurable pour considérer un fichier comme "gros" (en Ko)
        public long LargeFileThresholdInKB { get; set; } = 100; // Valeur par défaut modifiable
        public long LargeFileThresholdInBytes => LargeFileThresholdInKB * 1024;

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
        public void Stop() => Arret = true;

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
                Arret = false;
                if (IsBusinessSoftwareRunning())
                {
                    MessageBox.Show("Erreur : Le logiciel métier est en cours d'exécution. Veuillez le fermer pour continuer.",
                                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new Exception("Logiciel métier en cours d'exécution.");
                }

                // On attend tant que le logiciel métier est actif ou qu'une interruption est demandée
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
            var priorityExtensions = GetPrioritizedExtensions();

            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);

            var allFiles = Directory.GetFiles(sourceDir);
            var priorityFiles = allFiles.Where(f => priorityExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();
            var normalFiles = allFiles.Where(f => !priorityExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();

            // Traitement des fichiers prioritaires
            foreach (var file in priorityFiles)
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                try
                {
                    if (Arret)
                    {
                        MessageBox.Show("okokok");
                        throw new Exception("Sauvegarde arrétée prématurément");
                    }
                    while (Interruption)
                    {
                        Task.Delay(200).Wait(); // Attend 200 ms avant de vérifier à nouveau
                    }
                    Task.Delay(1500).Wait();

                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.Length > LargeFileThresholdInBytes)
                    {
                        _largeFileSemaphore.Wait();
                        try
                        {
                            File.Copy(file, destFile, true);
                        }
                        finally
                        {
                            _largeFileSemaphore.Release();
                        }
                    }
                    else
                    {
                        File.Copy(file, destFile, true);
                    }

                    transferredSize += fileInfo.Length;
                    transferredFiles++;
                    updateProgress(transferredSize, transferredFiles);
                }
                catch (Exception)
                {
                    throw; // Laissez l'exception remonter
                }
            }

            // Traitement des fichiers non prioritaires
            foreach (var file in normalFiles)
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                try
                {
                    if (Arret)
                    {
                        MessageBox.Show("okokok");
                        throw new Exception("Sauvegarde arrétée prématurément");
                    }
                    while (Interruption)
                    {
                        Task.Delay(200).Wait();
                    }
                    Task.Delay(1500).Wait();

                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.Length > LargeFileThresholdInBytes)
                    {
                        _largeFileSemaphore.Wait();
                        try
                        {
                            File.Copy(file, destFile, true);
                        }
                        finally
                        {
                            _largeFileSemaphore.Release();
                        }
                    }
                    else
                    {
                        File.Copy(file, destFile, true);
                    }

                    transferredSize += fileInfo.Length;
                    transferredFiles++;
                    updateProgress(transferredSize, transferredFiles);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            // Traitement récursif des sous-répertoires
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
            var priorityExtensions = GetPrioritizedExtensions();

            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);

            var allFiles = Directory.GetFiles(sourceDir);
            var priorityFiles = allFiles.Where(f => priorityExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();
            var normalFiles = allFiles.Where(f => !priorityExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();

            foreach (var file in priorityFiles)
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
                        FileInfo fileInfo = new FileInfo(file);
                        if (fileInfo.Length > LargeFileThresholdInBytes)
                        {
                            _largeFileSemaphore.Wait();
                            try
                            {
                                File.Copy(file, destFile, true);
                            }
                            finally
                            {
                                _largeFileSemaphore.Release();
                            }
                        }
                        else
                        {
                            File.Copy(file, destFile, true);
                        }
                        transferredSize += fileInfo.Length;
                        transferredFiles++;
                        updateProgress(transferredSize, transferredFiles);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            foreach (var file in normalFiles)
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
                        FileInfo fileInfo = new FileInfo(file);
                        if (fileInfo.Length > LargeFileThresholdInBytes)
                        {
                            _largeFileSemaphore.Wait();
                            try
                            {
                                File.Copy(file, destFile, true);
                            }
                            finally
                            {
                                _largeFileSemaphore.Release();
                            }
                        }
                        else
                        {
                            File.Copy(file, destFile, true);
                        }
                        transferredSize += fileInfo.Length;
                        transferredFiles++;
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

        private System.Collections.Generic.HashSet<string> GetPrioritizedExtensions()
        {
            var priorityExtensions = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string filePath = "extensionsPrioritaires.txt";
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        priorityExtensions.Add(line.Trim().ToLower());
                    }
                }
            }
            return priorityExtensions;
        }
    }
}
