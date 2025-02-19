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
        //private readonly GestionnaireSauvegarde _gestionnaireSauvegarde = new GestionnaireSauvegarde();
        private readonly classModel classModel = new classModel();

        [ObservableProperty]
        private string cheminSauvegardeSource;

        [ObservableProperty]
        private string nomSauvegarde;

        [ObservableProperty]
        private string cheminSauvegardeCible;


        [ObservableProperty]
        private string typeSauvegarde;

        [ObservableProperty]
        private ObservableCollection<string> extensions = new();

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
                //Console.WriteLine($"Dossier sélectionné : {CheminSauvegarde}");
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
            }
        }


        private void CrypterFichiers()
            // OUI ou NON
        {
            //if (string.IsNullOrWhiteSpace(cheminSauvegarde) || !Directory.Exists(cheminSauvegarde))
            //{
            //    MessageBox.Show("Veuillez sélectionner un dossier valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}

            //var selectedExtensions = extensions.ToList();

            //if (!selectedExtensions.Any())
            //{
            //    MessageBox.Show("Veuillez sélectionner au moins une extension à crypter.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}

            //try
            //{
            //    CryptoSoftManager.StartCrypto(cheminSauvegarde, selectedExtensions);
            //    MessageBox.Show("Chiffrement terminé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Erreur lors du chiffrement : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        private void LancerSauvegarde()
        {
            if (string.IsNullOrWhiteSpace(nomSauvegarde))
            {
                MessageBox.Show("Veuillez entrer un nom de sauvegarde.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(CheminSauvegardeSource))
            {
                MessageBox.Show("Veuillez sélectionner un dossier de sauvegarde.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(CheminSauvegardeCible))
            {
                MessageBox.Show("Veuillez sélectionner un dossier de à sauvegarde.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(typeSauvegarde))
            {
                MessageBox.Show("Veuillez sélectionner un dossier de sauvegarde.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var backupResult = classModel.runBackup(CheminSauvegardeSource, CheminSauvegardeCible, nomSauvegarde, typeSauvegarde);
                //_gestionnaireSauvegarde.DemarrerSauvegarde(nomSauvegarde, CheminSauvegarde);
                MessageBox.Show($"Sauvegarde '{nomSauvegarde}' enregistrée avec succès !", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
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
