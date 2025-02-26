using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Portable.Xaml;
using System.Diagnostics;
using System.Xml;


namespace Log
{
    public class LogEntry
    {
        public required string BackupName { get; set; }
        public required string Action { get; set; }
        public required DateTime Time { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? StrategyType { get; set; }

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

        public static bool choix = false;

        public static void Backup(string backupName, string source, string target, string transfert, string size, string strategyType)
        {
            LogEntry logEntry = new LogEntry
            {
                BackupName = backupName,
                Time = DateTime.Now,
                Source = source,
                RestorationTarget = target,
                StrategyType = strategyType,
                TransfertTime = transfert,
                Size = size,
                Action = "Sauvegarde"
            };
            AddLog(logEntry);
        }

        public static void Restore(string backupName, string source, string target, string transfert, string size, string strategyType)
        {
            LogEntry logEntry = new LogEntry
            {
                BackupName = backupName,
                Time = DateTime.Now,
                Source = source,
                RestorationTarget = target,
                StrategyType = strategyType,
                TransfertTime = transfert,
                Size = size,
                Action = "Restauration"
            };
            AddLog(logEntry);
        }

        public void Delete(string backupName)
        {
            LogEntry logEntry = new LogEntry
            {
                Time = DateTime.Now,
                BackupName = backupName,
                Action = "Remplacement"
            };
            AddLog(logEntry);
        }


        public static string AddLog(LogEntry logEntry)
        {
            CreateLogFile();
            switch (choix)
            {
                case false:
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

                case true:
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

        public static (List<Dictionary<string, string>> LogDictionaries, string ErrorMessage) LogsData()
        {
            string filename = choix == false ? jsonfile : xamlfile;
            List<Dictionary<string, string>> logDictionaries = new List<Dictionary<string, string>>();
            string errorMessage = string.Empty;

            if (!File.Exists(filename))
            {
                errorMessage = $"Le fichier {filename} n'existe pas.";
                return (logDictionaries, errorMessage);
            }

            try
            {
                List<LogEntry>? logEntries = choix == false
                    ? JsonSerializer.Deserialize<List<LogEntry>>(File.ReadAllText(filename))
                    : (List<LogEntry>?)XamlServices.Load(new StreamReader(filename));

                if (logEntries == null || logEntries.Count == 0)
                {
                    errorMessage = $"Le fichier {filename} est vide.";
                    return (logDictionaries, errorMessage);
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

                    var logDict = displayParts
                        .Select(part => part.Split(": ", 2))
                        .Where(parts => parts.Length == 2)
                        .ToDictionary(e => e[0], e => e[1]);

                    logDictionaries.Add(logDict);
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Erreur lors de la lecture du fichier {filename}: {ex.Message}";
            }

            return (logDictionaries, errorMessage);
        }


        public static string OpenLog()
        {
            string? path = null;


            switch (choix)
            {
                case false:
                    path = jsonpath;
                    break;

                case true:
                    path = xamlpath;
                    break;

                default:
                    return "Choix invalide.";
            }

            if (Directory.Exists(path))
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
                case false:
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

                case true:
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