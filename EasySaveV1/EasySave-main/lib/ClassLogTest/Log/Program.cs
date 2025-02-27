using System;
using System.IO;

namespace Log
{
    class Program
    {
        static void Main(string[] args)
        {
            // Créer une instance de la classe Historic
            Historic historic = new Historic();

            // Créer le fichier JSON si nécessaire
            historic.CreateFile();

            // Ajouter des logs de sauvegarde, restauration et remplacement
            historic.Backup("Save1", "C:\\Source\\file1.txt", "D:\\Target\\file1.txt", "100MB", "100MB");
            historic.Restore("Save2", "D:\\Source\\file2.txt", "C:\\Target\\file2.txt", "200MB", "200MB");
            historic.Replace("Save3", "Save4");

            // Afficher les logs actuels
            historic.DisplayLog();

            // Supprimer le fichier JSON
            // historic.DeleteFile();
        }
    }
}
