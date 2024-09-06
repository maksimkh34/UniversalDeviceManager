using System.Windows;
using UDM.Core.ViewModels;
using UDM.Model.MainModelSpace;

namespace UDM.WPF.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для UserInputWindow.xaml
    /// </summary>
    public partial class UserInputWindow
    {
        public UserInputWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MainModelStatic.CurrentUserInputFromUserInputWindow = ((UserInputWindowViewModel)DataContext).OutMsg;
            Close();
        }
    }
}