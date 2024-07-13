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
        private FastbootFlashDialogViewModel? _dataContext;

        public FastbootFlashDialog()
        {
            InitializeComponent();
        }

        private void FastbootFlashDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            _dataContext = new FastbootFlashDialogViewModel(Close);
            DataContext = _dataContext;
            ActiveDeviceTextBox.Text += MainModel.ModelDeviceManager.SelectedDevice.Id + ")";
            FastbootFlashDialogViewModel.Updater = UpdatePath;
        }

        public void UpdatePath(string path)
        {
            _dataContext!.SelectedImagePath = path;
        }
    }
}
