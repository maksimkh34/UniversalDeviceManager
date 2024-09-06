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
    /// Логика взаимодействия для FlashFullRomDialog.xaml
    /// </summary>
    public partial class FlashFullRomDialog : Window
    {
        private FlashFullRomViewModel? _dataContext;
        public FlashFullRomDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _dataContext = new FlashFullRomViewModel(Close);
            FlashFullRomViewModel.Updater = UpdatePath;
            ActiveDeviceTextBox.Text += MainModelStatic.ModelDeviceManager.ActiveDevice.Id + ")";
            DataContext = _dataContext;
        }

        private void UpdatePath(string path)
        {
            _dataContext!.SelectedRomPath = path;
        }
    }
}
