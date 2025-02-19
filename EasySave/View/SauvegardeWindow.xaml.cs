﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using CryptoSoft;
using ViewModels;

namespace Views
{
    public partial class SauvegardeWindow : Window
    {
        public SauvegardeWindow()
        {
            InitializeComponent();
            DataContext = new ViewModels.SaveViewModel();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
