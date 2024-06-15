using System.Windows;
using UDM.Core.ViewModels;

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
            DataContext = dataContext;
        }
    }
}
