using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}