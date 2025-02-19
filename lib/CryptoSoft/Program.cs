using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CryptoSoft
{
    public static class CryptoSoftManager
    {
        public static void StartCrypto(string folderPath, List<string> selectedExtensions)
        {
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
                throw new DirectoryNotFoundException("Erreur : Dossier introuvable.");

            var extensions = GetExtensionsFromFolder(folderPath);
            if (extensions.Count == 0)
                throw new Exception("Aucun fichier détecté dans ce dossier.");

            if (selectedExtensions.Count == 0)
                throw new Exception("Aucune extension sélectionnée pour le chiffrement.");

            string key = ReadCryptageKey();
            ProcessFiles(folderPath, key, selectedExtensions);
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
                    throw new Exception($"Erreur avec {file} : {ex.Message}");
                }
            }
        }

        public static string ReadCryptageKey()
        {
            return "MaSuperClé123"; // ⚠️ À sécuriser CHanger de procces pour demander de rentre la clé et pas de la mettre en statique
        }
    }
}
