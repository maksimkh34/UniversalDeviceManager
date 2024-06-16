using System.Windows;
using UDM.Core.ViewModels;
using UDM.Model.LogService;

namespace UDM.WPF.Dialogs
{
    public partial class MainWindow
    {
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
            var dataContext = new MainViewModel();
            dataContext.UpdateDevicesCommand?.Execute(null);
            DataContext = dataContext;

            LogService.Log("Hello!", LogLevel.Debug);
        }
    }
}