using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EasySaveApp
{
    public partial class RestoreWindow : Window
    {
        private GestionnaireSauvegarde gestionnaire = new GestionnaireSauvegarde();
        private List<string> sauvegardes = new List<string>();

        public RestoreWindow()
        {
            InitializeComponent();
            ChargerSauvegardes();
        }

        private void ChargerSauvegardes()
        {
            string backupsRoot = "Sauvegardes";
            if (!Directory.Exists(backupsRoot))
            {
                MessageBox.Show("Aucune sauvegarde disponible.");
                return;
            }

            sauvegardes = Directory.GetDirectories(backupsRoot).Select(Path.GetFileName).ToList();

            if (sauvegardes.Count == 0)
            {
                MessageBox.Show("Aucune sauvegarde trouvée.");
                return;
            }

            BackupListBox.ItemsSource = sauvegardes;
        }

        private void SelectionnerDossier_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog
            {
                Title = "Sélectionnez un dossier",
                ValidateNames = false,
                FolderName = "Sélectionner un dossier"
            };

            if (openFolderDialog.ShowDialog() == true)
            {
                DestinationsTextBox.Text += string.IsNullOrEmpty(DestinationsTextBox.Text)
                    ? openFolderDialog.FolderName
                    : Environment.NewLine + openFolderDialog.FolderName;
            }
        }

        private void Restaurer_Click(object sender, RoutedEventArgs e)
        {
            var selectedBackups = BackupListBox.SelectedItems.Cast<string>().ToList();
            var destinationPaths = DestinationsTextBox.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (selectedBackups.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner au moins une sauvegarde.");
                return;
            }

            if (selectedBackups.Count != destinationPaths.Count)
            {
                MessageBox.Show("Veuillez entrer un chemin pour chaque sauvegarde sélectionnée.");
                return;
            }

            bool isDifferential = RestoreTypeComboBox.SelectedIndex == 1;

            for (int i = 0; i < selectedBackups.Count; i++)
            {
                string backupName = selectedBackups[i];
                string backupPath = Path.Combine("Sauvegardes", backupName);
                string destination = destinationPaths[i];

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
            Close();
        }

        // Permet de déplacer la fenêtre en cliquant sur la barre de titre
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        // Ferme la fenêtre quand on clique sur "X"
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
