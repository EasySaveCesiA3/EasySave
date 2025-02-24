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
        //private List<string> sauvegardes = new List<string>();

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

        public ICommand SelectionnerDossierCommand { get; }
        public ICommand SelectionnerDossierCibleCommand { get; }
        public ICommand CrypterFichiersCommand { get; }
        public ICommand LancerSauvegardeCommand { get; }
        public ICommand ListerSauvegardesCommand { get; }


        //public RelayCommand<Window> CloseWindowCommand { get; }


        public SaveViewModel()
        {
            SelectionnerDossierCommand = new RelayCommand(SelectionnerDossier);
            SelectionnerDossierCibleCommand = new RelayCommand(SelectionnerDossierCible);
            LancerSauvegardeCommand = new RelayCommand(LancerSauvegarde);
            ListerSauvegardesCommand = new RelayCommand(() => ListerSauvegardes(true));

            TypeSauvegarde = TypesSauvegarde.First();
            ListerSauvegardes(false);
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

        private void CrypterFichiers()
        {

        }

        private void LancerSauvegarde()
        {
            //MessageBox.Show($"DEBUG: NomSauvegarde = {NomSauvegarde}");
            //MessageBox.Show($"DEBUG: CheminSauvegardeSource = {CheminSauvegardeSource}");
            //MessageBox.Show($"DEBUG: CheminSauvegardeCible = {CheminSauvegardeCible}");
            //MessageBox.Show($"DEBUG: TypeSauvegarde avant conversion aze = {TypeSauvegarde}");

            string typeSanitized = TypeSauvegarde.Trim();

            //Console.WriteLine($"DEBUG: TypeSauvegarde après Trim() = '{typeSanitized}'");

            string backupType = typeSanitized switch
            {
                "Complète" => "Complete",
                "Différentielle" => "Differential",
                _ => "INVALID"
            };

            //MessageBox.Show($"DEBUG: TypeSauvegarde après conversion aze = {backupType}");

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
                classModel.runBackup(CheminSauvegardeSource, CheminSauvegardeCible, NomSauvegarde, backupType);
                MessageBox.Show($"Sauvegarde '{NomSauvegarde}' enregistrée avec succès !", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                ListerSauvegardes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout de la sauvegarde : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        Target = sauvegarde.Target
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la récupération des sauvegardes : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}