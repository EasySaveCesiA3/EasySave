using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private const string FICHIER_EXTENSIONS = "extensions.txt";

        [ObservableProperty]
        private bool crypterFichiers;

        [ObservableProperty]
        private string nouvelleExtension;

        [ObservableProperty]
        private string extensionSelectionnee;

        [ObservableProperty]
        private ObservableCollection<string> extensionsCryptees = new ObservableCollection<string>();

        [ObservableProperty]
        private string logicielMetier;

        [ObservableProperty]
        private string langueSelectionnee;

        [ObservableProperty]
        private ObservableCollection<string> langues = new ObservableCollection<string> { "Français", "Anglais" };

        public SettingsViewModel()
        {
            SauvegarderCommand = new RelayCommand(SauvegarderParametres);
            RetourCommand = new RelayCommand(Retour);
            AjouterExtensionCommand = new RelayCommand(AjouterExtension);
            SupprimerExtensionCommand = new RelayCommand(SupprimerExtension);
            SelectionnerLogicielMetierCommand = new RelayCommand(SelectionnerLogicielMetier);

            ChargerExtensions();
        }

        public RelayCommand SauvegarderCommand { get; }
        public RelayCommand RetourCommand { get; }
        public RelayCommand AjouterExtensionCommand { get; }
        public RelayCommand SupprimerExtensionCommand { get; }
        public RelayCommand SelectionnerLogicielMetierCommand { get; }

        private void SauvegarderExtensions()
        {
            try
            {
                File.WriteAllLines(FICHIER_EXTENSIONS, ExtensionsCryptees);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Erreur lors de la sauvegarde des extensions : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChargerExtensions()
        {
            if (File.Exists(FICHIER_EXTENSIONS))
            {
                foreach (var extension in File.ReadAllLines(FICHIER_EXTENSIONS))
                {
                    ExtensionsCryptees.Add(extension);
                }
            }
        }

        private void SauvegarderParametres()
        {
            SauvegarderExtensions(); 
            MessageBox.Show("Paramètres sauvegardés avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Retour()
        {
            SauvegarderExtensions();
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }

        private void AjouterExtension()
        {
            if (!string.IsNullOrWhiteSpace(NouvelleExtension) && !ExtensionsCryptees.Contains(NouvelleExtension))
            {
                ExtensionsCryptees.Add(NouvelleExtension);
                SauvegarderExtensions();
                NouvelleExtension = ""; 
            }
        }

        private void SupprimerExtension()
        {
            if (!string.IsNullOrWhiteSpace(ExtensionSelectionnee) && ExtensionsCryptees.Contains(ExtensionSelectionnee))
            {
                ExtensionsCryptees.Remove(ExtensionSelectionnee);
                SauvegarderExtensions();
                ExtensionSelectionnee = null; 
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une extension à supprimer.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SelectionnerLogicielMetier()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Sélectionner un logiciel métier",
                Filter = "Fichiers exécutables (*.exe)|*.exe|Tous les fichiers (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                LogicielMetier = openFileDialog.FileName;
                Model.Copie.Instance.BusinessSoftwarePath = LogicielMetier;
            }
        }
    }
}
