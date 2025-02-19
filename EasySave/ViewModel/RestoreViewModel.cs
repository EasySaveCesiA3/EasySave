using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace EasySaveApp.ViewModels
{
    public class RestoreViewModel : INotifyPropertyChanged
    {
        private readonly GestionnaireSauvegarde gestionnaire;
        private string _selectedDestination;
        private int _selectedRestoreTypeIndex;

        public ObservableCollection<string> BackupList { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> SelectedBackups { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> DestinationPaths { get; set; } = new ObservableCollection<string>();

        public string SelectedDestination
        {
            get => _selectedDestination;
            set { _selectedDestination = value; OnPropertyChanged(); }
        }

        public int SelectedRestoreTypeIndex
        {
            get => _selectedRestoreTypeIndex;
            set { _selectedRestoreTypeIndex = value; OnPropertyChanged(); }
        }

        public ICommand LoadBackupsCommand { get; }
        public ICommand SelectFolderCommand { get; }
        public ICommand RestoreCommand { get; }
        public ICommand CloseCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public RestoreViewModel()
        {
            gestionnaire = new GestionnaireSauvegarde();

            LoadBackupsCommand = new RelayCommand(LoadBackups);
            SelectFolderCommand = new RelayCommand(SelectFolder);
            RestoreCommand = new RelayCommand(Restore);
            CloseCommand = new RelayCommand(Close);

            LoadBackups();
        }

        private void LoadBackups()
        {
            BackupList.Clear();
            string backupsRoot = "Sauvegardes";

            if (!Directory.Exists(backupsRoot))
            {
                MessageBox.Show("Aucune sauvegarde disponible.");
                return;
            }

            var directories = Directory.GetDirectories(backupsRoot).Select(Path.GetFileName).ToList();
            if (directories.Count == 0)
            {
                MessageBox.Show("Aucune sauvegarde trouvée.");
                return;
            }

            foreach (var dir in directories)
                BackupList.Add(dir);
        }

        private void SelectFolder()
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog
            {
                Title = "Sélectionnez un dossier"
            };

            if (dialog.ShowDialog() == true)
            {
                DestinationPaths.Add(dialog.FolderName);
            }
        }

        private void Restore()
        {
            if (SelectedBackups.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner au moins une sauvegarde.");
                return;
            }

            if (SelectedBackups.Count != DestinationPaths.Count)
            {
                MessageBox.Show("Veuillez entrer un chemin pour chaque sauvegarde sélectionnée.");
                return;
            }

            bool isDifferential = SelectedRestoreTypeIndex == 1;

            for (int i = 0; i < SelectedBackups.Count; i++)
            {
                string backupName = SelectedBackups[i];
                string backupPath = Path.Combine("Sauvegardes", backupName);
                string destination = DestinationPaths[i];

                if (string.IsNullOrEmpty(destination) || !Directory.Exists(Path.GetDirectoryName(destination)))
                {
                    MessageBox.Show($"Chemin invalide pour {backupName}. Restauration annulée.");
                    continue;
                }

                if (isDifferential)
                {
                    gestionnaire.RestoreBackupDifferential(backupPath, destination, backupName);
                }
                else
                {
                    gestionnaire.RestoreBackup(backupPath, destination, backupName);
                }
            }

            MessageBox.Show("Restauration terminée !");
        }

        private void Close()
        {
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
