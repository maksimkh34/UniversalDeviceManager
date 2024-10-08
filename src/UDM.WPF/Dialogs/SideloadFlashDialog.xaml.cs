﻿using System.Windows;
using UDM.Core.ViewModels;
using UDM.Model.MainModelSpace;

namespace UDM.WPF.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для FastbootFlashDialog.xaml
    /// </summary>
    public partial class SideloadFlashDialog : Window
    {
        private SideloadFlashViewModel? _dataContext;

        public SideloadFlashDialog()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _dataContext = new SideloadFlashViewModel(Close);
            DataContext = _dataContext;
            ActiveDeviceTextBox.Text += MainModelStatic.ModelDeviceManager.ActiveDevice.Id + ")";
            SideloadFlashViewModel.Updater = UpdatePath;
        }

        public void UpdatePath(string path)
        {
            _dataContext!.SelectedArchivePath = path;
        }
    }
}
