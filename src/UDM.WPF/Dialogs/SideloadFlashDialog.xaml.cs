﻿using System.Windows;
using UDM.Core.ViewModels;
using UDM.Model;

namespace UDM.WPF.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для FastbootFlashDialog.xaml
    /// </summary>
    public partial class SideloadFlashDialog : Window
    {
        private SideloadFlashDialogViewModel? _dataContext;

        public SideloadFlashDialog()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _dataContext = new SideloadFlashDialogViewModel(Close);
            DataContext = _dataContext;
            ActiveDeviceTextBox.Text += MainModel.ModelDeviceManager.SelectedDevice.Id + ")";
            SideloadFlashDialogViewModel.Updater = UpdatePath;
        }

        public void UpdatePath(string path)
        {
            _dataContext!.SelectedArchivePath = path;
        }
    }
}
