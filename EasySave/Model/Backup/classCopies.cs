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

        public async Task<(long totalSize, long totalFiles)> CopyDirectoryAsync(string sourceDir, string destinationDir, bool useDifferential)
        {
            return await Task.Run(() =>
            {
                // Vérification initiale
                CheckForInterruption();

                // Lancement de la copie
                return useDifferential
                    ? CopyDirectoryDifferential(sourceDir, destinationDir)
                    : CopyDirectory(sourceDir, destinationDir);
            });
        }


        public bool IsBusinessSoftwareRunning()
        {
            if (string.IsNullOrWhiteSpace(BusinessSoftwarePath))
                return false;

            string exeName = Path.GetFileNameWithoutExtension(BusinessSoftwarePath);
            
            return Process.GetProcessesByName(exeName).Any();
        }
        public (long totalSize, long totalFiles) CopyDirectory(string sourceDir, string destinationDir)
        {
            long totalSize = 0;
            long totalFiles = 0;

            // Liste d'extensions prioritaires
            var priorityExtensions = new System.Collections.Generic.HashSet<string> { ".pdf", ".docx", ".xlsx" };

            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);

            var allFiles = Directory.GetFiles(sourceDir);
            var priorityFiles = allFiles.Where(f => priorityExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();
            var normalFiles = allFiles.Where(f => !priorityExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();

            foreach (var file in priorityFiles)
            {
                CheckForInterruption();
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
                    throw;
                }
            }

            foreach (var file in normalFiles)
            {
                CheckForInterruption();
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
                    throw;
                }
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                CheckForInterruption();
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

            // Liste d'extensions prioritaires
            var priorityExtensions = new System.Collections.Generic.HashSet<string> { ".pdf", ".docx", ".xlsx" };

            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);

            var allFiles = Directory.GetFiles(sourceDir);
            var priorityFiles = allFiles.Where(f => priorityExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();
            var normalFiles = allFiles.Where(f => !priorityExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();

            // Copier d'abord les fichiers prioritaires
            foreach (var file in priorityFiles)
            {
                CheckForInterruption();

                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                bool copyFile = true;
                if (File.Exists(destFile))
                {
                    FileInfo srcInfo = new FileInfo(file);
                    FileInfo destInfo = new FileInfo(destFile);
                    if (srcInfo.Length == destInfo.Length)
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

            // Puis copier les fichiers non prioritaires
            foreach (var file in normalFiles)
            {
                CheckForInterruption();

                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                bool copyFile = true;
                if (File.Exists(destFile))
                {
                    FileInfo srcInfo = new FileInfo(file);
                    FileInfo destInfo = new FileInfo(destFile);
                    if (srcInfo.Length == destInfo.Length)
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

            // Copier récursivement les sous-dossiers
            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                CheckForInterruption();
                string destDirPath = Path.Combine(destinationDir, Path.GetFileName(dir));
                var result = CopyDirectoryDifferential(dir, destDirPath);
                totalSize += result.totalSize;
                totalFiles += result.totalFiles;
            }

            return (totalSize, totalFiles);
        }


        private void CheckForInterruption()
        {
            // Vérifie si le logiciel métier est en cours d’exécution
            if (IsBusinessSoftwareRunning())
            {
                MessageBox.Show("Erreur : Le logiciel métier est en cours d'exécution. Copie interrompue.",
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Vérifie si l’utilisateur a activé l’interruption
            if (Interruption)
            {
                MessageBox.Show("Copie interrompue par l’utilisateur.",
                                "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

   

}
