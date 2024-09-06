using System.Windows;
using UDM.Core.ViewModels;
using UDM.Model.MainModelSpace;

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
