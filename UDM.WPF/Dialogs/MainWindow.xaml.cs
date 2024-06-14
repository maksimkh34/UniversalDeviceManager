using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using UDM.Model;
using UDM.Model.Log;

namespace UDM.WPF.Dialogs
{
    public partial class MainWindow
    {
        #region Converters


        #endregion

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
            DataContext = new Core.ViewModels.MainViewModel();
        }

        #region DP



        #endregion
    }
}
