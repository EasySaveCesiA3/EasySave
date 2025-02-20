using System.Windows;
using ViewModels;

namespace Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new SaveViewModel();
        }

        private void AddBackup(object sender, RoutedEventArgs e)
        {
            var viewModel = (SaveViewModel)DataContext;
            viewModel.LancerSauvegardeCommand.Execute(null);

            MessageBox.Show("LancerSauvegardeCommand exécuté avec succès !");
        }
    }
}

