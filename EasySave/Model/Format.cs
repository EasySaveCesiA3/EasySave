using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Log;

namespace Model
{
    internal class Format
    {
        private static Format? _instance;
        private static readonly object _lock = new object();

        // Constructeur privé pour empêcher l'instanciation externe
        private Format() { }

        // Propriété pour récupérer l'instance unique
        public static Format Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new Format();
                    }
                }
                return _instance;
            }
        }

        internal void ChoixFormat()
        {
            Console.WriteLine("======================================");
            Console.WriteLine("=          EasySave - Format         =");
            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("Veuillez choisir le format de sauvegarde : \n 1 : Json\n 2 : Xaml");
            string choix = Console.ReadLine();
            switch (choix)
            {
                case "1":
                    Historic.choix = "1";
                    Historic.CreateFile();
                    Console.WriteLine("Sauvegarde sous format Json");
                    break;
                case "2":
                    Historic.choix = "2";
                    Historic.CreateFile();
                    Console.WriteLine("Sauvegarde sous format Xaml");
                    break;
                default:
                    Console.WriteLine("Cette valeur n'est pas autorisée.");
                    ChoixFormat();
                    break;
            }
        }
    }
}