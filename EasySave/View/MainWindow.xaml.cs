using System.Windows;
using ViewModel;
using ViewModels;

namespace Views
{
    public partial class MainWindow : Window
    {

        private readonly classInterfaceViewModel _interfaceViewModel;


        public MainWindow()
        {
            InitializeComponent();

            // Initialiser les ViewModels
            _interfaceViewModel = new classInterfaceViewModel();


            // Affecter SaveViewModel comme DataContext global
            DataContext = _interfaceViewModel;
        }

        private void AddBackup(object sender, RoutedEventArgs e)
        {
            _interfaceViewModel.LancerSauvegardeCommand.Execute(null);
            MessageBox.Show("LancerSauvegardeCommand exécuté avec succès !");
        }

        private void QuitterApplication(object sender, RoutedEventArgs e)
        {
            _interfaceViewModel.QuitterCommand.Execute(null);
        }
        private void RestaurerSauvegardeCommand(object sender, RoutedEventArgs e)
        {
            _interfaceViewModel.RestoreCommand.Execute(null);
            MessageBox.Show("LancerSauvegardeCommand exécuté avec succès !");

        }
    }
}