using System.Globalization;
using System.Windows;

namespace EasySaveApp
{
    public partial class SelectionLangueWindow : Window
    {
        public SelectionLangueWindow()
        {
            InitializeComponent();
        }

        private void SelectionnerFrancais_Click(object sender, RoutedEventArgs e)
        {
            ChangerLangue("fr");
        }

        private void SelectionnerAnglais_Click(object sender, RoutedEventArgs e)
        {
            ChangerLangue("en");
        }

        private void ChangerLangue(string langue)
        {
            // Change la culture de l'application
            CultureInfo culture = new CultureInfo(langue);
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;

            MessageBox.Show($"Langue changée en {culture.DisplayName}", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}
