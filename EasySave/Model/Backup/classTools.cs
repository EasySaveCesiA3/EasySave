using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}