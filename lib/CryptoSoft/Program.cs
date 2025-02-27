using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CryptoSoft
{
    public static class CryptoSoftManager
    {
        private static string _cryptageKey;


        public static void StartCrypto(string folderPath, List<string> selectedExtensions, string key)
        {
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
                throw new DirectoryNotFoundException("Erreur : Dossier introuvable.");

            if (selectedExtensions == null || selectedExtensions.Count == 0)
                throw new Exception("Aucune extension sélectionnée pour le chiffrement.");

            if (string.IsNullOrEmpty(key) || key.Length < 8)
                throw new ArgumentException("La clé doit faire au moins 8 caractères.", nameof(key));

            _cryptageKey = key;

            ProcessFiles(folderPath, _cryptageKey, selectedExtensions);
        }

        public static List<string> GetExtensionsFromFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException("Le dossier spécifié n'existe pas.");

            List<string> extensions = Directory.GetFiles(folderPath)
                .Select(file => Path.GetExtension(file).ToLower())
                .Distinct()
                .Where(ext => !string.IsNullOrEmpty(ext))
                .OrderBy(ext => ext)
                .ToList();

            Console.WriteLine("Extensions trouvées : " + string.Join(", ", extensions));

            return extensions;
        }

        public static void ProcessFiles(string folderPath, string key, List<string> selectedExtensions)
        {
            foreach (var file in Directory.GetFiles(folderPath))
            {
                if (!selectedExtensions.Contains(Path.GetExtension(file).ToLower()))
                    continue;

                try
                {
                    var fileManager = new FileManager(file, key);
                    fileManager.TransformFile();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur avec {file} : {ex.Message}");
                }
            }
        }
    }
}