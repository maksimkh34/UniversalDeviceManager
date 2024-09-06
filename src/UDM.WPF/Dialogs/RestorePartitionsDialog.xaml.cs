using System.Windows;
using UDM.Core.ViewModels;

namespace UDM.WPF.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для RestorePartitionsDialog.xaml
    /// </summary>
    public partial class RestorePartitionsDialog : Window
    {
        RestorePartitionsViewModel? _viewModel;

        public RestorePartitionsDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = new RestorePartitionsViewModel();
            DataContext = _viewModel;
        }
    }
}
