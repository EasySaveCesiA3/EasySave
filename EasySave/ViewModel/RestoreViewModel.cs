using System;
using Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace ViewModel
{
    public partial class RestoreViewModel : ObservableObject
    {

        public void RestoreBackup(BackupData selectedBackup)
        {
            if (selectedBackup == null)
            {
                MessageBox.Show("Aucune sauvegarde sélectionnée.", "Erreur", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            bool isDifferential = selectedBackup.Strategy is "Differential";
            classModel.runRestore(selectedBackup.Name, selectedBackup.Target);
        }
    }
}