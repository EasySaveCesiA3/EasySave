using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CryptoSoft;
using Microsoft.Win32;
using Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Xml.Linq;
using System.Text.Json;


namespace ViewModels
{
    public partial class SaveViewModel : ObservableObject
    {
        private readonly classModel classModel = new classModel();


        public static void LancerSauvegarde(string nomSauvegarde, string cheminSauvegardeSource, string cheminSauvegardeCible, string typeSauvegarde, bool crypter)
        {
            // On "nettoie" et convertit le type de sauvegarde
            string typeSanitized = typeSauvegarde?.Trim();
            string backupType = typeSanitized switch
            {
                "Complète" => "Complete",
                "Différentielle" => "Differential",
                _ => "INVALID"
            };

            // Vérifications des entrées utilisateur
            if (string.IsNullOrWhiteSpace(nomSauvegarde))
            {
                MessageBox.Show("Veuillez entrer un nom de sauvegarde.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(cheminSauvegardeSource))
            {
                MessageBox.Show("Veuillez sélectionner un dossier de sauvegarde source.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(cheminSauvegardeCible))
            {
                MessageBox.Show("Veuillez sélectionner un dossier cible.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (backupType == "INVALID")
            {
                MessageBox.Show("Veuillez sélectionner un type de sauvegarde.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            { 
                // Instanciation du modèle et exécution de la sauvegarde
                classModel classModelInstance = new classModel();
                classModel.runBackup(cheminSauvegardeSource, cheminSauvegardeCible, nomSauvegarde, backupType);
                
                string cheminSauvegarde = Path.Combine("Sauvegardes", nomSauvegarde);
                
                if (crypter)
                {
                    CrypterFichiers(cheminSauvegarde);

                    CreerMetadata(cheminSauvegarde, crypter);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout de la sauvegarde : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void CrypterFichiers(string cheminSauvegarde)
        {
            try
            {
                if (string.IsNullOrEmpty(cheminSauvegarde) || !Directory.Exists(cheminSauvegarde))
                {
                    MessageBox.Show("Le dossier de sauvegarde n'existe pas.",
                                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Charger les extensions à crypter depuis "extensions.txt"
                var extensionsACrypter = File.Exists("extensions.txt")
                    ? File.ReadAllLines("extensions.txt")
                           .Select(e => e.Trim().ToLower())
                           .Where(e => !string.IsNullOrWhiteSpace(e))
                           .ToList()
                    : new List<string>();

                if (extensionsACrypter.Count == 0)
                {
                    MessageBox.Show("Aucune extension définie pour le cryptage.",
                                    "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Récupérer la liste des fichiers dans le dossier cible correspondant aux extensions
                var fichiersACrypter = Directory.GetFiles(cheminSauvegarde)
                                                .Where(f => extensionsACrypter.Contains(Path.GetExtension(f).ToLower()))
                                                .ToList();

                if (!fichiersACrypter.Any())
                {
                    MessageBox.Show("Aucun fichier à crypter trouvé dans la sauvegarde.",
                                    "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Demander la clé de cryptage (8 caractères minimum)
                string key = Microsoft.VisualBasic.Interaction.InputBox(
                    "Entrez votre clé de cryptage (min. 8 caractères) :",
                    "Clé de Cryptage",
                    "");
                while (string.IsNullOrEmpty(key) || key.Length < 8)
                {
                    key = Microsoft.VisualBasic.Interaction.InputBox(
                        "La clé doit faire au moins 8 caractères. Réessayez :",
                        "Clé de Cryptage",
                        "");
                }

                // Appel de la DLL CryptoSoft (qui ne dépend pas de WPF)
                CryptoSoftManager.StartCrypto(cheminSauvegarde, extensionsACrypter, key);

                MessageBox.Show("Cryptage terminé. Les fichiers ont été cryptés dans la sauvegarde.",
                                "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du cryptage : {ex.Message}",
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void CreerMetadata(string cheminSauvegarde, bool crypter)
        {
            try
            {
                // On récupère éventuellement la liste des extensions cryptees
                string[] extensions = new string[0];
                if (crypter && File.Exists("extensions.txt"))
                {
                    extensions = File.ReadAllLines("extensions.txt")
                                     .Select(e => e.Trim())
                                     .Where(e => !string.IsNullOrWhiteSpace(e))
                                     .ToArray();
                }

                var metadata = new
                {
                    Crypte = crypter, // indique que la sauvegarde a été filtrée (cryptée)
                    Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    ExtensionsCryptees = extensions
                };

                string jsonMetadata = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
                string metadataFilePath = Path.Combine(cheminSauvegarde, "metadata.json");
                File.WriteAllText(metadataFilePath, jsonMetadata);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la création du fichier de métadonnées : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}