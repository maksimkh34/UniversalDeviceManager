using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using UDM.Core.ViewModels;
using UDM.Model;
using UDM.Model.LogService;

namespace UDM.WPF.Dialogs
{
    public partial class MainWindow
    {
        private MainViewModel? _dataContext;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.ShutdownApp();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _dataContext = new MainViewModel();
            //dataContext.UpdateDevicesCommand.Execute(null);
            DataContext = _dataContext;

            MainModel.ModelDeviceManager.UpdateDevices();

            LogService.Log("MainWindow Loaded!", LogLevel.Debug);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)   // test button
        {
            new PreDIL().ShowDialog();
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Settings window = new();
            window.Show();
        }

        private void Menu_ScriptNew_Click(object sender, RoutedEventArgs e)
        {
            MainModel.CurrentScriptCode = string.Empty;
            new PreDIL().Show();
        }

        private void Menu_ScriptExec_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = false,
                ReadOnlyChecked = false,
                CheckFileExists = false,
                AddExtension = true,
                DefaultExt = "dil",
                Filter = "DIL Scripts (*.dil)|*.dil|All files (*.*)|*.*",
                Title = "Open DIL Script"
            };
            dialog.ShowDialog();

            if(dialog.FileName == string.Empty) return;
            var scriptFile = dialog.FileName;

            MainModel.CurrentScriptCode = File.ReadAllText(scriptFile);
            new PreDIL().Show();
        }

        private void Menu_UpdatePartitionsClick(object sender, RoutedEventArgs e)
        {
            if (!MainModel.ModelDeviceManager.IsActiveDeviceConnected() || 
                (MainModel.ModelDeviceManager.ActiveDevice.Type != DeviceConnectionType.adb && MainModel.ModelDeviceManager.ActiveDevice.Type != DeviceConnectionType.recovery))
                MainModel.UiMsgDialog?.Invoke("Error", "Device must in ADB mode!");

            MainModel.ModelDeviceManager.ActiveDevice.UpdatePartitions();
            if(MainModel.ModelDeviceManager.ActiveDevice.IsPartitioned)
            {
                MainModel.UiMsgDialog?.Invoke("OK", "Partitions table updated! ");
                LogService.Log("Found partitions: " + MainModel.ModelDeviceManager.ActiveDevice.Partitions.Count, LogLevel.Debug);
            } else MainModel.UiMsgDialog?.Invoke("Error", "Error updating partiotins table!");
        }

        private void Menu_FastbootFlash_Click(object sender, RoutedEventArgs e)
        {
            new FastbootFlashDialog().ShowDialog();
        }

        private void Menu_FastbootReboot_Click(object sender, RoutedEventArgs e)
        {
            new FastbootRebootDialog().ShowDialog();
        }

        private void Menu_FastbootCheckBootloader_Click(object sender, RoutedEventArgs e)
        {
            MainModel.CheckBlStatus();
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true});
            e.Handled = true;

        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            LogTextBox.ScrollToEnd();
        }

        private void Menu_SideloadFlash(object sender, RoutedEventArgs e)
        {
            new SideloadFlashDialog().ShowDialog();
        }

        private void Menu_FlashFullRom_Click(object sender, RoutedEventArgs e)
        {
            new FlashFullRomDialog().ShowDialog();
        }

        private void Menu_BackupPartitionsADB(object sender, RoutedEventArgs e)
        {
            new BackupPartitionsDialog().ShowDialog();
        }

        private void Menu_RestorePartitionsADB(object sender, RoutedEventArgs e)
        {
            new RestorePartitionsDialog().ShowDialog();
        }
    }
}