using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using UDM.Core.ViewModels;
using UDM.Model;
using UDM.Model.MainModelSpace;

namespace UDM.WPF.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для BackupPartitionsDialog.xaml
    /// </summary>
    public partial class BackupPartitionsDialog : Window
    {
        private BackupPartitionsViewModel? _viewModel;
        public BackupPartitionsDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = new BackupPartitionsViewModel();
            BackupPartitionsViewModel.CloseWindow = Close;
            if (MainModelStatic.ModelDeviceManager.ActiveDevice.Type != DeviceConnectionType.adb 
                && MainModelStatic.ModelDeviceManager.ActiveDevice.Type != DeviceConnectionType.recovery)
            {
                App.ShowMessage("No compatible devices found! ");
                Close();
            }
            if (_viewModel.BeforeSelectPartitions.Count == 0)
            {
                MainModelStatic.ModelDeviceManager.ActiveDevice.UpdatePartitions();
                App.ShowMessage("Can't get partitions table! ");
                Close();
            }

            DataContext = _viewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var e1 = (ListBox)FindName("listbox1")!;
            ObservableCollection<string?> e2 = [];
            foreach (var e3 in e1.SelectedItems)
            {
                e2.Add(e3.ToString());
            }
            BackupPartitionsViewModel.ApplyFunction(e2);
        }
    }
}
