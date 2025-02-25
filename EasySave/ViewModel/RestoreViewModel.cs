﻿using System;
using Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace ViewModel
{
    public partial class RestoreViewModel : ObservableObject
    {
        private readonly classModel _classModel = new classModel();

        public void RestoreBackup(BackupData selectedBackup)
        {
            if (selectedBackup == null)
            {
                System.Windows.MessageBox.Show("Aucune sauvegarde sélectionnée.", "Erreur", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            bool isDifferential = selectedBackup.Strategy is "Differential";
            classModel.runRestore(selectedBackup.Name, selectedBackup.Target);
        }
    }
}