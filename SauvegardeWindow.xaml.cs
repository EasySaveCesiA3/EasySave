using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;


namespace EasySaveApp
{
    public partial class SauvegardeWindow : Window
    {
        private GestionnaireSauvegarde gestionnaire = new GestionnaireSauvegarde();

        public SauvegardeWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Ouvre l'explorateur Windows pour sélectionner un dossier
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
                CheminSauvegardeTextBox.Text = openFolderDialog.FolderName;
            }   
        }

        private void CrypterFichiers_Click(object sender, RoutedEventArgs e)
        {
            string cheminDossier = CheminSauvegardeTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(cheminDossier) || !Directory.Exists(cheminDossier))
            {
                MessageBox.Show("Veuillez sélectionner un dossier valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Lancer CryptoSoft comme un processus
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "",
                Arguments = $"\"{cheminDossier}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    MessageBox.Show("Chiffrement terminé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'exécution de CryptoSoft : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Valider et enregistrer la sauvegarde
        private void LancerSauvegarde_Click(object sender, RoutedEventArgs e)
        {
            string nomSauvegarde = NomSauvegardeTextBox.Text.Trim();
            string cheminSauvegarde = CheminSauvegardeTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(nomSauvegarde))
            {
                MessageBox.Show("Veuillez entrer un nom de sauvegarde.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(cheminSauvegarde))
            {
                MessageBox.Show("Veuillez sélectionner un dossier de sauvegarde.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                gestionnaire.DemarrerSauvegarde(nomSauvegarde, cheminSauvegarde);

                MessageBox.Show($"Sauvegarde '{nomSauvegarde}' enregistrée avec succès !", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout de la sauvegarde : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
