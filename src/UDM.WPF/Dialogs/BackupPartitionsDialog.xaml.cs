using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UDM.Core.ViewModels;
using UDM.Model;

namespace UDM.WPF.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для BackupPartitionsDialog.xaml
    /// </summary>
    public partial class BackupPartitionsDialog : Window
    {
        BackupPartitionsViewModel? _viewModel;
        public BackupPartitionsDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = new(Close);
            if(MainModel.ModelDeviceManager.ActiveDevice.Type != DeviceConnectionType.adb 
                && MainModel.ModelDeviceManager.ActiveDevice.Type != DeviceConnectionType.recovery)
            {
                App.ShowMessage("No compatible devices found! ");
                Close();
            }
            if (_viewModel.BeforeSelectPartitions.Count == 0)
            {
                MainModel.ModelDeviceManager.ActiveDevice.UpdatePartitions();
                App.ShowMessage("Can't get partitions table! ");
                Close();
            }

            DataContext = _viewModel;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _viewModel?.SelectPartition(e.AddedItems[0]?.ToString());
            }
            catch (IndexOutOfRangeException) { }
        }
    }
}
