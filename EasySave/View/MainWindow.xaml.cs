﻿using System.Windows;
using ViewModels;

namespace Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModels.MainViewModel();

        }
    }
}
