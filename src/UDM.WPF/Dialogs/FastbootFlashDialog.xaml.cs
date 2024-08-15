using System.Windows;
using UDM.Core.ViewModels;
using UDM.Model;

namespace UDM.WPF.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для FastbootFlashDialog.xaml
    /// </summary>
    public partial class FastbootFlashDialog
    {
        private FastbootFlashViewModel? _dataContext;

        public FastbootFlashDialog()
        {
            InitializeComponent();
        }

        private void FastbootFlashDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            _dataContext = new FastbootFlashViewModel(Close);
            DataContext = _dataContext;
            ActiveDeviceTextBox.Text += MainModel.ModelDeviceManager.ActiveDevice.Id + ")";
            FastbootFlashViewModel.Updater = UpdatePath;
        }

        public void UpdatePath(string path)
        {
            _dataContext!.SelectedImagePath = path;
        }
    }
}
