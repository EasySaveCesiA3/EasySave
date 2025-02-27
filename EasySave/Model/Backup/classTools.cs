using CryptoSoft;
using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using static Model.StateManager;

namespace Model
{
    public class Tools
    {
        //private static Tools? _instance;
        //private static readonly object _lock = new object();
        //private volatile bool Interruption = false;

        //private Tools() { }

        //public static Tools Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            lock (_lock)
        //            {
        //                if (_instance == null)
        //                    _instance = new Tools();
        //            }
        //        }
        //        return _instance;
        //    }
        //}

        public static string DeleteFiles(string path)
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
                }

                return $"Le répertoire '{path}' a été supprimé avec succès.";
            }
            catch (Exception e)
            {
                return $"Une erreur est survenue lors de la suppression forcée du répertoire '{path}' : {e.Message}";
            }
        }

        public static void CreateFile(string relativePath)
        {
            try
            {
                string fullPath = Path.GetFullPath(relativePath);
                string? directory = Path.GetDirectoryName(fullPath);

                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!File.Exists(fullPath))
                {
                    File.Create(fullPath).Close(); // Fermer immédiatement pour libérer le fichier
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Erreur lors de la création du fichier '{relativePath}' : {e.Message}");
            }
        }

        public static void WriteBackupState(string path, string name, long transferredSize, long sourceSize, string action, string state)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path, false))  // false pour écraser le fichier à chaque fois
                {
                    writer.WriteLine(name);
                    writer.WriteLine(transferredSize);
                    writer.WriteLine(sourceSize);
                    writer.WriteLine(action);
                    writer.WriteLine(state);
                }
            }
            catch (Exception e)
            {
            }
        }


        public static BackupProgress ReadBackupState(string path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string name = reader.ReadLine();
                    long progress = long.Parse(reader.ReadLine());
                    long total = long.Parse(reader.ReadLine());
                    string action = reader.ReadLine();
                    string state = reader.ReadLine();

                    return new BackupProgress
                    {
                        Name = name,
                        Progress = progress,
                        Total = total,
                        Action = action,
                        State = state
                    };
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }



        public static long GetSize(string path)
        {
            string fullPath = Path.GetFullPath(path);

            if (File.Exists(fullPath))
            {
                return new FileInfo(fullPath).Length;
            }
            else if (Directory.Exists(fullPath))
            {
                return new DirectoryInfo(fullPath)
                    .EnumerateFiles("*", SearchOption.AllDirectories)
                    .Sum(f => f.Length);
            }

            return 0;
        }

        internal static void CrypterFichiers(string cheminSauvegarde)
        {
            try
            {
                if (string.IsNullOrEmpty(cheminSauvegarde) || !Directory.Exists(cheminSauvegarde))
                {
                    MessageBox.Show("Le dossier de sauvegarde n'existe pas.",
                                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Charger les extensions à crypter depuis "extensions.txt"
                var extensionsACrypter = File.Exists("extensions.txt")
                    ? File.ReadAllLines("extensions.txt")
                           .Select(e => e.Trim().ToLower())
                           .Where(e => !string.IsNullOrWhiteSpace(e))
                           .ToList()
                    : new List<string>();

                if (extensionsACrypter.Count == 0)
                {
                    MessageBox.Show("Aucune extension définie pour le cryptage.",
                                    "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Récupérer la liste des fichiers dans le dossier cible correspondant aux extensions
                var fichiersACrypter = Directory.GetFiles(cheminSauvegarde, "*", SearchOption.AllDirectories)
                                .Where(f => extensionsACrypter.Contains(Path.GetExtension(f).ToLower()))
                                .ToList();


                if (!fichiersACrypter.Any())
                {
                    MessageBox.Show("Aucun fichier à crypter trouvé dans la sauvegarde.",
                                    "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Demander la clé de cryptage (8 caractères minimum)
                string key = Microsoft.VisualBasic.Interaction.InputBox(
                    "Entrez votre clé de cryptage (min. 8 caractères) :",
                    "Clé de Cryptage",
                    "");
                while (string.IsNullOrEmpty(key) || key.Length < 8)
                {
                    key = Microsoft.VisualBasic.Interaction.InputBox(
                        "La clé doit faire au moins 8 caractères. Réessayez :",
                        "Clé de Cryptage",
                        "");
                }

                // Appel de la DLL CryptoSoft (qui ne dépend pas de WPF)
                CryptoSoftManager.StartCrypto(cheminSauvegarde, extensionsACrypter, key);

                MessageBox.Show("Cryptage terminé. Les fichiers ont été cryptés dans la sauvegarde.",
                                "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du cryptage : {ex.Message}",
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal static void CreerMetadata(string cheminSauvegarde)
        {
            try
            {
                // On récupère éventuellement la liste des extensions cryptees
                string[] extensions = new string[0];
                if (File.Exists("extensions.txt"))
                {
                    extensions = File.ReadAllLines("extensions.txt")
                                     .Select(e => e.Trim())
                                     .Where(e => !string.IsNullOrWhiteSpace(e))
                                     .ToArray();
                }

                var metadata = new
                {
                    Crypte = true, // indique que la sauvegarde a été filtrée (cryptée)
                    Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    ExtensionsCryptees = extensions
                };

                string jsonMetadata = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
                string metadataFilePath = Path.Combine(cheminSauvegarde, "metadata.json");
                File.WriteAllText(metadataFilePath, jsonMetadata);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la création du fichier de métadonnées : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal static void DecrypterSauvegarde(string backupName, string targetFolder)
        {
            try
            {
                string backupFolder = Path.Combine("Sauvegardes", backupName);
                string backupMetadataPath = Path.Combine(Path.GetFullPath(backupFolder), "metadata.json");

                if (!File.Exists(backupMetadataPath))
                {
                    MessageBox.Show("Aucun fichier de métadonnées trouvé dans le dossier de sauvegarde.",
                                    "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                string metadataJson = File.ReadAllText(backupMetadataPath);
                var metadata = JsonSerializer.Deserialize<StateManager.BackupMetadata>(metadataJson);
                if (metadata == null || !metadata.Crypte)
                {
                    MessageBox.Show("La sauvegarde n'est pas cryptée.",
                                    "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                string key = Microsoft.VisualBasic.Interaction.InputBox("Cette sauvegarde est cryptée. Entrez la clé pour décrypter :",
                                                                         "Décryptage", "");
                while (string.IsNullOrEmpty(key) || key.Length < 8)
                {
                    key = Microsoft.VisualBasic.Interaction.InputBox("La clé doit faire au moins 8 caractères. Réessayez :",
                                                                     "Décryptage", "");
                }

                var extensionsToDecrypt = metadata.ExtensionsCryptees.ToList();

                CryptoSoftManager.StartCrypto(targetFolder, extensionsToDecrypt, key);

                string targetMetadataPath = Path.Combine(Path.GetFullPath(targetFolder), "metadata.json");

                if (!Path.GetFullPath(backupFolder).Equals(Path.GetFullPath(targetFolder), StringComparison.OrdinalIgnoreCase))
                {
                    if (File.Exists(targetMetadataPath))
                    {
                        File.Delete(targetMetadataPath);
                    }
                }

                MessageBox.Show("La sauvegarde a été décrytée avec succès.",
                                "Décryptage", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du décryptage : {ex.Message}",
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}