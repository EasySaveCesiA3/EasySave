using System;
using Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.IO;

namespace ViewModel
{
    public partial class DeleteViewModel : ObservableObject
    {

        public static void DeleteBackup(BackupData selectedBackup)
        {
            if (selectedBackup == null)
            {
                System.Windows.MessageBox.Show("Aucune sauvegarde sélectionnée.", "Erreur", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            Tools.DeleteFiles($"Sauvegardes/{selectedBackup.Name}");
            MessageBox.Show($"Sauvegarde {selectedBackup.Name} supprimée");
        }
    }
}
