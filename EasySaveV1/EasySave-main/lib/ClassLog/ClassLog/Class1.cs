using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;

namespace Log
{
    public class Historic
    {
        public static string jsonfile => $"historique/historique.json{DateTime.Now:yyyy-MM-dd}";

        public class LogEntry
        {
            public required string Action { get; set; }
            public required DateTime Time { get; set; }
            public required string Logname { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string? Logname2 { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string? Transfert { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string? Source { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string? Target { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string? Size { get; set; }
        }

        public void Backup(string logname, string source, string target, string transfert, string size)
        {
            LogEntry logEntry = new LogEntry
            {
                Logname = logname,
                Time = DateTime.Now,
                Source = source,
                Target = target,
                Transfert = transfert,
                Size = size,
                Action = "Sauvegarde"
            };
            Add(logEntry);
        }

        public void Restore(string logname, string source, string target, string transfert, string size)
        {
            LogEntry logEntry = new LogEntry
            {
                Logname = logname,
                Time = DateTime.Now,
                Source = source,
                Target = target,
                Transfert = transfert,
                Size = size,
                Action = "Restauration"
            };
            Add(logEntry);
        }

        public void Replace(string logname, string logname2)
        {
            LogEntry logEntry = new LogEntry
            {
                Time = DateTime.Now,
                Logname = logname,
                Logname2 = logname2,
                Action = "Remplacement"
            };
            Add(logEntry);
        }

        public void Add(LogEntry logEntry)
        {
            List<LogEntry> logEntries = new List<LogEntry>();

            try
            {
                if (File.Exists(jsonfile))
                {
                    string json = File.ReadAllText(jsonfile);
                    logEntries = JsonSerializer.Deserialize<List<LogEntry>>(json) ?? new List<LogEntry>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la lecture du fichier JSON : {ex.Message}");
            }

            logEntries.Add(logEntry);

            try
            {
                string newJson = JsonSerializer.Serialize(logEntries, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(jsonfile, newJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'écriture dans le fichier JSON : {ex.Message}");
            }
        }

        public void DisplayLog()
        {
            if (File.Exists(jsonfile))
            {
                try
                {
                    string json = File.ReadAllText(jsonfile);
                    var logEntries = JsonSerializer.Deserialize<List<LogEntry>>(json);

                    if (logEntries == null || logEntries.Count == 0)
                    {
                        Console.WriteLine("Le fichier est vide.");
                        return;
                    }

                    foreach (var entry in logEntries)
                    {
                        var displayParts = new List<string>();

                        foreach (var prop in entry.GetType().GetProperties())
                        {
                            object? value = prop.GetValue(entry);
                            if (value != null)
                            {
                                displayParts.Add($"{prop.Name}: {value}");
                            }
                        }

                        Console.WriteLine(string.Join(", ", displayParts));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la lecture du fichier JSON : {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Le fichier JSON n'existe pas.");
            }
        }

        public void CreateFile()
        {
            // Récupère le chemin complet du dossier historique
            string directory = Path.GetDirectoryName(jsonfile);

            // Créer le dossier 'historique' s'il n'existe pas
            if (!Directory.Exists(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                    Console.WriteLine("Dossier 'historique' créé avec succès !");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la création du dossier : {ex.Message}");
                    return;
                }
            }

            // Vérifier si le fichier existe déjà, sinon créer un fichier vide
            if (!File.Exists(jsonfile))
            {
                try
                {
                    File.WriteAllText(jsonfile, "[]");
                    Console.WriteLine("Fichier JSON créé avec succès !");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la création du fichier JSON : {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Le fichier JSON existe déjà, il n'a pas été écrasé.");
            }
        }

        public void DeleteFile()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C rmdir /s /q \"historique\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();
                    Console.WriteLine(process.StandardOutput.ReadToEnd());
                }

                Console.WriteLine("Le répertoire 'historique' a été supprimé avec succès.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Une erreur est survenue lors de la suppression forcée du répertoire : " + e.Message);
            }
        }

    }
}

