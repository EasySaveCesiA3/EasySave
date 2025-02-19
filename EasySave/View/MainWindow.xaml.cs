using System.Diagnostics;
using System.IO;
using System.Windows;
using Logs;

namespace EasySaveApp
{
    public partial class MainWindow : Window
    {
        public static Historic historic = new Historic();

        private GestionnaireSauvegarde gestionnaire = new GestionnaireSauvegarde();
        Process process = new Process();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DemarrerSauvegarde_Click(object sender, RoutedEventArgs e)
        {
            SauvegardeWindow sauvegardeWindow = new SauvegardeWindow();
            sauvegardeWindow.Show();
        }


        private void RestaurerSauvegarde_Click(object sender, RoutedEventArgs e)
        {
            RestoreWindow restoreWindow = new RestoreWindow();
            restoreWindow.ShowDialog();
        }


        private void ChangerLangue_Click(object sender, RoutedEventArgs e)
        {
            SelectionLangueWindow languageWindow = new SelectionLangueWindow();
            languageWindow.ShowDialog();
        }

        private void Quitter_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
