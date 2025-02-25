using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ViewModels;

namespace ViewModel

{
    public partial class classInterfaceViewModel : ObservableObject
    {
        public ICommand SelectionnerDossierCommand { get; }
        public ICommand SelectionnerDossierCibleCommand { get; }
        public ICommand CrypterFichiersCommand { get; }
        public ICommand LancerSauvegardeCommand { get; }
        public ICommand ListerSauvegardesCommand { get; }
        public ICommand RestoreCommand { get; }
        public RelayCommand QuitterCommand { get; }



        [ObservableProperty]
        private BackupData selectBackup;

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

        [ObservableProperty]
        private ObservableCollection<BackupData> listeSauvegardes = new ObservableCollection<BackupData>();

        public classInterfaceViewModel()
        {

            SelectionnerDossierCommand = new RelayCommand(SelectionnerDossier);
            SelectionnerDossierCibleCommand = new RelayCommand(SelectionnerDossierCible);
            LancerSauvegardeCommand = new RelayCommand(LancerSauvegarde);
            ListerSauvegardesCommand = new RelayCommand(() => ListerSauvegardes(true));
            RestoreCommand = new RelayCommand(RestoreBackup);
            QuitterCommand = new RelayCommand(QuitterApplication);
            //public RelayCommand<Window> CloseWindowCommand { get; }

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
                //MessageBox.Show($"DEBUG: CheminSauvegardeSource sélectionné = {CheminSauvegardeSource}");
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
                //MessageBox.Show($"DEBUG: CheminSauvegardeCible sélectionné = {CheminSauvegardeCible}");
            }
        }
        private void ListerSauvegardes(bool afficherMessage = true)
        {
            ListeSauvegardes.Clear();

            try
            {
                List<BackupData> sauvegardes = classModel.listBackups();

                if (sauvegardes == null || sauvegardes.Count == 0)
                {
                    if (afficherMessage)
                    {
                        MessageBox.Show("Aucune sauvegarde disponible.");
                    }
                    return;
                }

                foreach (var sauvegarde in sauvegardes)
                {
                    ListeSauvegardes.Add(new BackupData
                    {
                        Name = sauvegarde.Name,
                        Source = sauvegarde.Source,
                        Target = sauvegarde.Target,
                        Strategy = sauvegarde.Strategy
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la récupération des sauvegardes : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LancerSauvegarde()
        {
            SaveViewModel.LancerSauvegarde(NomSauvegarde, CheminSauvegardeSource, CheminSauvegardeCible, TypeSauvegarde);
        }
        private void RestoreBackup()
        {
            if (SelectBackup == null)
            {
                MessageBox.Show("Veuillez sélectionner une sauvegarde à restaurer.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Restauration en cours...");

            RestoreViewModel restoreViewModel = new RestoreViewModel();
            restoreViewModel.RestoreBackup(SelectBackup);

            MessageBox.Show("Restauration terminée avec succès !");
        }

        private void QuitterApplication()
        {
            //M essageBox.Show("Fermeture de l'application...");
            Application.Current.Shutdown();
        }
    }
}