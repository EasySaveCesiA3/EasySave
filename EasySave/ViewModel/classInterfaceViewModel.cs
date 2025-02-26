﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Model;
using Log;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ViewModels;
using System.IO;
using ControlzEx.Standard;
using System.Windows.Threading;

namespace ViewModel

{
    public partial class classInterfaceViewModel : ObservableObject
    {
        public ICommand SelectionnerDossierCommand { get; }
        public ICommand SelectionnerDossierCibleCommand { get; }
        public ICommand ListerTravailCommand { get; }
        public ICommand CrypterFichiersCommand { get; }
        public ICommand LancerSauvegardeCommand { get; }
        public ICommand ListerSauvegardesCommand { get; }
        public ICommand DeleteSauvegardeCommand { get; }
        public ICommand RestoreCommand { get; }
        public ICommand OuvrirParametresCommand { get; }
        public ICommand ViewLogsCommand { get; }
        public ICommand ChangeLogFormatCommand { get; }
        public RelayCommand QuitterCommand { get; }



        [ObservableProperty]
        private BackupData selectSauvegarde;

        [ObservableProperty]
        private BackupData selectTravail;

        [ObservableProperty]
        private string cheminSauvegardeSource;

        [ObservableProperty]
        private string cheminSauvegardeCible;

        [ObservableProperty]
        private string nomSauvegarde;

        [ObservableProperty]
        private string typeSauvegarde;

        [ObservableProperty]
        private bool crypterFichiers = false;

        [ObservableProperty]
        private bool nePascrypterFichiers = true;

        // Ajout des types de sauvegarde pour la ComboBox
        [ObservableProperty]
        private ObservableCollection<string> typesSauvegarde = new ObservableCollection<string>
        {
            "Complète",
            "Différentielle"
        };

        [ObservableProperty]
        private ObservableCollection<BackupData> listeSauvegardes = new ObservableCollection<BackupData>();

        [ObservableProperty]
        private ObservableCollection<Model.StateManager.BackupProgress> listeTravaux = new ObservableCollection<Model.StateManager.BackupProgress>();

        public classInterfaceViewModel()
        {

            SelectionnerDossierCommand = new RelayCommand(SelectionnerDossier);
            SelectionnerDossierCibleCommand = new RelayCommand(SelectionnerDossierCible);
            //ListerTravailCommand = new RelayCommand(ListerTravail);
            LancerSauvegardeCommand = new RelayCommand(LancerSauvegarde);
            ListerSauvegardesCommand = new RelayCommand(() => ListerSauvegardes(true));
            RestoreCommand = new RelayCommand(RestoreBackup);
            QuitterCommand = new RelayCommand(QuitterApplication);
            OuvrirParametresCommand = new RelayCommand(OuvrirParametres);
            DeleteSauvegardeCommand = new RelayCommand(LancerSuppression);
            ViewLogsCommand = new RelayCommand(ouvrirLog);
            ChangeLogFormatCommand = new RelayCommand(changerLogType);
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
            }
        }

        //private async Task ListerTravail()
        //{
        //    ListeTravaux.Clear();

        //    string[] fichiers = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Etat"), "*.txt");

        //    foreach (var path in fichiers)
        //    {
        //        string nomFichier = Path.GetFileName(path);

        //        await Dispatcher.InvokeAsync(() =>
        //        {
        //            ListeTravaux.Add(Model.Tools.ReadBackupState(path));
        //        });
        //    }
        //}


        private void ListerSauvegardes(bool afficherMessage = true)
        {
            ListeSauvegardes.Clear();

            try
            {
                var (logDictionaries, errorMessage) = Historic.LogsData();

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    if (afficherMessage)
                    {
                        MessageBox.Show(errorMessage);
                    }
                    return;
                }

                string sauvegardesPath = Path.Combine(Directory.GetCurrentDirectory(), "Sauvegardes");

                foreach (var logDict in logDictionaries)
                {
                    if (logDict.TryGetValue("Action", out string? action) && action == "Sauvegarde" &&
                        logDict.TryGetValue("BackupName", out string? name) &&
                        Directory.Exists(Path.Combine(sauvegardesPath, name)) && // Vérifie si BackupName est un dossier existant
                        logDict.TryGetValue("Source", out string? source) &&
                        logDict.TryGetValue("RestorationTarget", out string? target) &&
                        logDict.TryGetValue("StrategyType", out string? strategy))
                    {
                        ListeSauvegardes.Add(new BackupData
                        {
                            Name = name,
                            Source = source,
                            Target = target,
                            Strategy = strategy
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la récupération des sauvegardes : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ouvrirLog()
        {
            classModel.openLog();

        }
        private void LancerSauvegarde()
        {
            SaveViewModel.LancerSauvegarde(NomSauvegarde, CheminSauvegardeSource, CheminSauvegardeCible, TypeSauvegarde, CrypterFichiers);
        }
        private void RestoreBackup()
        {
            if (SelectSauvegarde == null)
            {
                MessageBox.Show("Veuillez sélectionner une sauvegarde à restaurer.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Restauration en cours...");

            RestoreViewModel restoreViewModel = new RestoreViewModel();
            restoreViewModel.RestoreBackup(SelectSauvegarde); 
        }
        private void OuvrirParametres()
        {
            Views.SettingsWindow settingsWindow = new Views.SettingsWindow();
            settingsWindow.Show();
        }
        private void LancerSuppression()
        {
            if (SelectSauvegarde == null)
            {
                MessageBox.Show("Veuillez sélectionner une sauvegarde à restaurer.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DeleteViewModel.DeleteBackup(SelectSauvegarde);
        }

        private void QuitterApplication()
        {
            //M essageBox.Show("Fermeture de l'application...");
            Application.Current.Shutdown();
        }

        private void changerLogType()
        {
            if (classModel.logtype())
            {
                MessageBox.Show("Format => XML");
            }
            else
            {
                MessageBox.Show("Format => JSON "); ;
            }

        }
    }
}