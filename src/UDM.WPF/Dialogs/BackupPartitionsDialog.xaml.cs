using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            if(MainModelStatic.ModelDeviceManager.ActiveDevice.Type != DeviceConnectionType.adb 
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
            ListBox e1 = (ListBox)this.FindName("listbox1");
            ObservableCollection<string> e2 = new ObservableCollection<string>();
            foreach (var e3 in e1.SelectedItems)
            {
                e2.Add(e3.ToString());
            }
            BackupPartitionsViewModel.ApplyFunction(e2);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ListBox e1 = (ListBox)this.FindName("listbox1");
            e1.SelectedItems.Clear();
            foreach (var e2 in e1.Items)
            {
                e1.SelectedItems.Add(e2.ToString());
            }
        }
    }
}
