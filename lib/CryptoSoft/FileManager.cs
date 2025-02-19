using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CryptoSoft
{
    public class FileManager
    {
        private readonly string FilePath;
        private readonly byte[] Key;

        public FileManager(string path, string key)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Fichier introuvable : {path}");

            if (string.IsNullOrEmpty(key) || Encoding.UTF8.GetByteCount(key) < 8)
                throw new ArgumentException("La clé doit faire au moins 8 caractères.");

            FilePath = path;
            Key = Encoding.UTF8.GetBytes(key);
        }

        public int TransformFile()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                byte[] fileBytes = File.ReadAllBytes(FilePath);
                byte[] transformedBytes = XorMethod(fileBytes, Key);
                File.WriteAllBytes(FilePath, transformedBytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur : {ex.Message}");
            }

            stopwatch.Stop();
            return (int)stopwatch.ElapsedMilliseconds;
        }

        private static byte[] XorMethod(byte[] fileBytes, byte[] keyBytes)
        {
            byte[] result = new byte[fileBytes.Length];

            for (int i = 0; i < fileBytes.Length; i++)
            {
                result[i] = (byte)(fileBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return result;
        }
    }
}
