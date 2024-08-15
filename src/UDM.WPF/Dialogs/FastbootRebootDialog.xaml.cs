﻿using System.Windows;
using UDM.Core.ViewModels;
using UDM.Model;

namespace UDM.WPF.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для FastbootRebootDialog.xaml
    /// </summary>
    public partial class FastbootRebootDialog
    {
        public FastbootRebootDialog()
        {
            InitializeComponent();
        }

        private void FastbootRebootDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dataContext = new FastbootRebootViewModel(Close);
            DataContext = dataContext;
            ActiveDeviceTextBox.Text += MainModel.ModelDeviceManager.ActiveDevice.Id + ")";
        }

    }
}
