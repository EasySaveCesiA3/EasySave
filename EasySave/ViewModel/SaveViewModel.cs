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


namespace ViewModels
{
    public partial class SaveViewModel : ObservableObject
    {
        private readonly classModel classModel = new classModel();

        private void CrypterFichiers()
        {

        }

        public static void LancerSauvegarde(string nomSauvegarde, string cheminSauvegardeSource, string cheminSauvegardeCible, string typeSauvegarde)
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout de la sauvegarde : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



    }
}