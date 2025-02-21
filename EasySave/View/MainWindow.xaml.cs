using System.Windows;
using ViewModels;

namespace Views
{
    public partial class MainWindow : Window
    {
        private readonly SaveViewModel _saveViewModel;
        private readonly ApplicationViewModel _appViewModel;

        public MainWindow()
        {
            InitializeComponent();

            // Initialiser les ViewModels
            _saveViewModel = new SaveViewModel();
            _appViewModel = new ApplicationViewModel();

            // Affecter SaveViewModel comme DataContext global
            DataContext = _saveViewModel;
        }

        private void AddBackup(object sender, RoutedEventArgs e)
        {
            _saveViewModel.LancerSauvegardeCommand.Execute(null);
            MessageBox.Show("LancerSauvegardeCommand exécuté avec succès !");
        }

        private void QuitterApplication(object sender, RoutedEventArgs e)
        {
            _appViewModel.QuitterCommand.Execute(null);
        }
    }
}
