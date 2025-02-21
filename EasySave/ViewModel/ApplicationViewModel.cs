using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ViewModels
{
    public partial class ApplicationViewModel : ObservableObject
    {
        public RelayCommand QuitterCommand { get; }

        public ApplicationViewModel()
        {
            QuitterCommand = new RelayCommand(QuitterApplication);
        }

        private void QuitterApplication()
        {
            //M essageBox.Show("Fermeture de l'application...");
            Application.Current.Shutdown();
        }
    }
}
