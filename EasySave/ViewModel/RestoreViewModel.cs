using System;
using Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using CryptoSoft;
using System.Text.Json;
using Microsoft.VisualBasic;
using System.IO;
using System.IO.Packaging;

namespace ViewModel
{
    public partial class RestoreViewModel : ObservableObject
    {
        private readonly classModel _classModel = new classModel();

        public void RestoreBackup(BackupData selectedBackup)
        {
            if (selectedBackup == null)
            {
                MessageBox.Show("Aucune sauvegarde sélectionnée.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            bool isDifferential = selectedBackup.Strategy is "Differential";
           
            classModel.runRestore(selectedBackup.Name, selectedBackup.Target);

            string pathBackup = Path.Combine("Sauvegardes",selectedBackup.Name);
            string pathMetadata = Path.Combine(pathBackup, "metadata.json");

            if (File.Exists(pathMetadata)) 
            { 
                DecrypterSauvegarde(selectedBackup.Name, selectedBackup.Target);
            }
            MessageBox.Show($"Restauration faites avec succès !", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DecrypterSauvegarde(string backupName, string targetFolder)
        {
            try
            {
                string backupFolder = Path.Combine("Sauvegardes", backupName);
                string backupMetadataPath = Path.Combine(Path.GetFullPath(backupFolder), "metadata.json");

                if (!File.Exists(backupMetadataPath))
                {
                    MessageBox.Show("Aucun fichier de métadonnées trouvé dans le dossier de sauvegarde.",
                                    "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                string metadataJson = File.ReadAllText(backupMetadataPath);
                var metadata = JsonSerializer.Deserialize<BackupMetadata>(metadataJson);
                if (metadata == null || !metadata.Crypte)
                {
                    MessageBox.Show("La sauvegarde n'est pas cryptée.",
                                    "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                string key = Microsoft.VisualBasic.Interaction.InputBox("Cette sauvegarde est cryptée. Entrez la clé pour décrypter :",
                                                                         "Décryptage", "");
                while (string.IsNullOrEmpty(key) || key.Length < 8)
                {
                    key = Microsoft.VisualBasic.Interaction.InputBox("La clé doit faire au moins 8 caractères. Réessayez :",
                                                                     "Décryptage", "");
                }

                var extensionsToDecrypt = metadata.ExtensionsCryptees.ToList();

                CryptoSoftManager.StartCrypto(targetFolder, extensionsToDecrypt, key);

                string targetMetadataPath = Path.Combine(Path.GetFullPath(targetFolder), "metadata.json");

                if (!Path.GetFullPath(backupFolder).Equals(Path.GetFullPath(targetFolder), StringComparison.OrdinalIgnoreCase))
                {
                    if (File.Exists(targetMetadataPath))
                    {
                        File.Delete(targetMetadataPath);
                    }
                }

                MessageBox.Show("La sauvegarde a été décrytée avec succès.",
                                "Décryptage", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du décryptage : {ex.Message}",
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private class BackupMetadata
        {
            public bool Crypte { get; set; }
            public string Date { get; set; }
            public string[] ExtensionsCryptees { get; set; }
        }
    }


}