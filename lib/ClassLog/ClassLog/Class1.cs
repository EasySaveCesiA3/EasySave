using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Portable.Xaml;
using System.Diagnostics;
using System.Xml;
using static Log.Historic;

namespace Log
{
    public class LogEntry
    {
        public required string Logname { get; set; }
        public required string Action { get; set; }
        public required DateTime Time { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TransfertTime { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Source { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RestorationTarget { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Size { get; set; }
    }

    public class Historic
    {
        public static string jsonpath => "Historiques/historiqueJSON/";
        public static string xamlpath => "Historiques/historiqueXAML/";
        public static string jsonfile => $"Historiques/historiqueJSON/historique{DateTime.Now:yyyy-MM-dd}.json";
        public static string xamlfile => $"Historiques/historiqueXAML/historique{DateTime.Now:yyyy-MM-dd}.xaml";

        public static string choix = "1";

        public static void Backup(string logname, string source, string target, string transfert, string size)
        {
            LogEntry logEntry = new LogEntry
            {
                Logname = logname,
                Time = DateTime.Now,
                Source = source,
                RestorationTarget = target,
                TransfertTime = transfert,
                Size = size,
                Action = "Sauvegarde"
            };
            AddLog(logEntry);
        }

        public static void Restore(string logname, string source, string target, string transfert, string size)
        {
            LogEntry logEntry = new LogEntry
            {
                Logname = logname,
                Time = DateTime.Now,
                Source = source,
                RestorationTarget = target,
                TransfertTime = transfert,
                Size = size,
                Action = "Restauration"
            };
            AddLog(logEntry);
        }

        public void Delete(string logname)
        {
            LogEntry logEntry = new LogEntry
            {
                Time = DateTime.Now,
                Logname = logname,
                Action = "Remplacement"
            };
            AddLog(logEntry);
        }


        public static string AddLog(LogEntry logEntry)
        {
            switch (choix)
            {
                case "1":
                    List<LogEntry> logEntriesJson = new List<LogEntry>();

                    if (File.Exists(jsonfile))
                    {
                        try
                        {
                            string json = File.ReadAllText(jsonfile);
                            var existingLogs = JsonSerializer.Deserialize<List<LogEntry>>(json);
                            if (existingLogs != null)
                            {
                                logEntriesJson = existingLogs;
                            }
                        }
                        catch (Exception ex)
                        {
                            return $"Erreur lors de la lecture du fichier JSON : {ex.Message}";
                        }
                    }

                    logEntriesJson.Add(logEntry);


                    try
                    {
                        string newJson = JsonSerializer.Serialize(logEntriesJson, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(jsonfile, newJson);
                    }
                    catch (Exception ex)
                    {
                        return $"Erreur lors de l'écriture dans le fichier JSON : {ex.Message}";
                    }
                    return "";

                case "2":
                    List<LogEntry> logEntriesXaml = new List<LogEntry>();

                    if (File.Exists(xamlfile))
                    {
                        try
                        {
                            using (var reader = new StreamReader(xamlfile))
                            {
                                var existingLogs = XamlServices.Load(reader) as List<LogEntry>;
                                if (existingLogs != null)
                                {
                                    logEntriesXaml = existingLogs;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            return $"Erreur lors de la lecture du fichier XAML : {ex.Message}";
                        }
                    }

                    logEntriesXaml.Add(logEntry);

                    try
                    {
                        using (var writer = new StreamWriter(xamlfile))
                        {
                            XamlServices.Save(writer, logEntriesXaml);
                        }
                    }
                    catch (Exception ex)
                    {
                        return $"Erreur lors de l'écriture dans le fichier XAML : {ex.Message}";
                    }
                    return "";

                default:
                    return "Choix invalide.";
            }
        }

        public static (List<string> LogMessages, string ErrorMessage) DisplayLog()
        {
            string filename = choix == "1" ? jsonfile : xamlfile;
            List<string> logMessages = new List<string>();
            string errorMessage = string.Empty;

            if (!File.Exists(filename))
            {
                errorMessage = $"Le fichier {filename} n'existe pas.";
                return (logMessages, errorMessage);
            }

            try
            {
                List<LogEntry>? logEntries = choix == "1"
                    ? JsonSerializer.Deserialize<List<LogEntry>>(File.ReadAllText(filename))
                    : (List<LogEntry>?)XamlServices.Load(new StreamReader(filename));

                if (logEntries == null || logEntries.Count == 0)
                {
                    errorMessage = $"Le fichier {filename} est vide.";
                    return (logMessages, errorMessage);
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

                    logMessages.Add(string.Join(", ", displayParts));
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Erreur lors de la lecture du fichier {filename}: {ex.Message}";
            }

            return (logMessages, errorMessage);
        }



        public static string OpenLog()
        {
            string? path = null;

            switch (choix)
            {
                case "1":
                    path = jsonpath;
                    break;

                case "2":
                    path = xamlpath;
                    break;

                default:
                    return "Choix invalide.";
            }

            if (!Directory.Exists(path))
            {
                try
                {
                    Process.Start("explorer.exe", Path.GetFullPath(path));
                    return "";
                }
                catch (Exception ex)
                {
                    return $"Erreur lors de l'ouverture du dossier : {ex.Message}";
                }
            }
            else
            {
                return $"Le dossier {path} n'existe pas.";
            }
        }

        public static string CreateLogFile()
        {
            string? file;
            string directory;

            switch (choix)
            {
                case "1":
                    file = jsonfile;
                    directory = $"{Path.GetDirectoryName(file)}";
                    if (!Directory.Exists(directory))
                    {
                        try
                        {
                            Directory.CreateDirectory(directory);
                            return "";
                        }
                        catch (Exception ex)
                        {
                            return $"Erreur lors de la création du dossier : {ex.Message}";
                        }
                    }

                    if (!File.Exists(jsonfile))
                    {
                        try
                        {
                            File.WriteAllText(jsonfile, "[]");
                            return "";
                        }
                        catch (Exception ex)
                        {
                            return $"Erreur lors de la création du fichier JSON : {ex.Message}";
                        }
                    }
                    else
                    {
                        return "";
                    }

                case "2":
                    file = xamlfile;
                    directory = $"{Path.GetDirectoryName(file)}";
                    if (!Directory.Exists(directory))
                    {
                        try
                        {
                            Directory.CreateDirectory(directory);
                            return "";
                        }
                        catch (Exception ex)
                        {
                            return $"Erreur lors de la création du dossier : {ex.Message}";
                        }
                    }

                    if (!File.Exists(xamlfile))
                    {
                        try
                        {
                            File.WriteAllText(xamlfile, "");

                            return "";
                        }
                        catch (Exception ex)
                        {
                            return $"Erreur lors de la création du fichier XAML : {ex.Message}";
                        }
                    }
                    else
                    {
                        return "Le fichier XAML existe déjà, il n'a pas été écrasé.";
                    }

                default:
                    return "Choix invalide.";
            }
        }


        public static string DeleteFile()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C rmdir /s /q \"Historiques\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();
                }

                return "Le répertoire 'historique' a été supprimé avec succès.";
            }
            catch (Exception e)
            {
                return "Une erreur est survenue lors de la suppression forcée du répertoire : " + e.Message;
            }
        }

    }
}