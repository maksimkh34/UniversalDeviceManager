using System.Windows;
using UDM.Core.ViewModels;
using UDM.Model;
using UDM.Model.LogService;

namespace UDM.WPF.Dialogs
{
    public partial class MainWindow
    {
        MainViewModel? _dataContext;

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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
        }
    }
}