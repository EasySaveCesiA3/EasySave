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

        [ObservableProperty]
        private string cheminSauvegardeSource;

        [ObservableProperty]
        private string cheminSauvegardeCible;

        [ObservableProperty]
        private string nomSauvegarde;

        [ObservableProperty]
        private string typeSauvegarde;

        // Ajout des types de sauvegarde pour la ComboBox
        [ObservableProperty]
        private ObservableCollection<string> typesSauvegarde = new ObservableCollection<string>
        {
            "Complète",
            "Différentielle"
        };

        public ICommand SelectionnerDossierCommand { get; }
        public ICommand SelectionnerDossierCibleCommand { get; }
        public ICommand ChargerExtensionsCommand { get; }
        public ICommand CrypterFichiersCommand { get; }
        public ICommand LancerSauvegardeCommand { get; }
        public RelayCommand<Window> CloseWindowCommand { get; }

        public SaveViewModel()
        {
            SelectionnerDossierCommand = new RelayCommand(SelectionnerDossier);
            SelectionnerDossierCibleCommand = new RelayCommand(SelectionnerDossierCible);
            CrypterFichiersCommand = new RelayCommand(CrypterFichiers);
            LancerSauvegardeCommand = new RelayCommand(LancerSauvegarde);
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow);
            TypeSauvegarde = TypesSauvegarde.First();
        }

        private void SelectionnerDossier()
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog
            {
                Title = "Sélectionnez un dossier source",
                ValidateNames = false,
                FolderName = "Sélectionner un dossier"
            };

            if (openFolderDialog.ShowDialog() == true)
            {
                CheminSauvegardeSource = openFolderDialog.FolderName;
                MessageBox.Show($"DEBUG: CheminSauvegardeSource sélectionné = {CheminSauvegardeSource}");
            }
        }

        private void SelectionnerDossierCible()
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog
            {
                Title = "Sélectionnez un dossier cible",
                ValidateNames = false,
                FolderName = "Sélectionner un dossier"
            };

            if (openFolderDialog.ShowDialog() == true)
            {
                CheminSauvegardeCible = openFolderDialog.FolderName;
                MessageBox.Show($"DEBUG: CheminSauvegardeCible sélectionné = {CheminSauvegardeCible}");
            }
        }

        private void CrypterFichiers()
        {
            
        }

        private void LancerSauvegarde()
        {
            MessageBox.Show($"DEBUG: NomSauvegarde = {NomSauvegarde}");
            MessageBox.Show($"DEBUG: CheminSauvegardeSource = {CheminSauvegardeSource}");
            MessageBox.Show($"DEBUG: CheminSauvegardeCible = {CheminSauvegardeCible}");
            MessageBox.Show($"DEBUG: TypeSauvegarde avant conversion aze = {TypeSauvegarde}");

            string typeSanitized = TypeSauvegarde.Trim();

            Console.WriteLine($"DEBUG: TypeSauvegarde après Trim() = '{typeSanitized}'");

            string backupType = typeSanitized switch
            {
                "Complète" => "Complete",
                "Différentielle" => "Differential",
                _ => "INVALID"  // Ajoute un cas pour voir si la valeur ne correspond à rien
            };

            MessageBox.Show($"DEBUG: TypeSauvegarde après conversion aze = {backupType}");

            if (string.IsNullOrWhiteSpace(NomSauvegarde))
            {
                MessageBox.Show("Veuillez entrer un nom de sauvegarde.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(CheminSauvegardeSource))
            {
                MessageBox.Show("Veuillez sélectionner un dossier de sauvegarde source.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(CheminSauvegardeCible))
            {
                MessageBox.Show("Veuillez sélectionner un dossier cible.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(backupType))
            {
                MessageBox.Show("Veuillez sélectionner un type de sauvegarde.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var backupResult = classModel.runBackup(CheminSauvegardeSource, CheminSauvegardeCible, NomSauvegarde, backupType);
                MessageBox.Show($"Sauvegarde '{NomSauvegarde}' enregistrée avec succès !", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout de la sauvegarde : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseWindow(Window window)
        {
            window?.Close();
        }
    }
}
