using System.Windows;

namespace UDM.WPF.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для MessageBoxWindow.xaml
    /// </summary>
    public partial class MessageBoxWindow
    {
        private readonly string _msg;
        private readonly string _textBoxMessage;
        public MessageBoxWindow(string message, string textBoxMessage )
        {
            InitializeComponent();
            _msg = message;
            _textBoxMessage = textBoxMessage;
        }

        private void MessageBoxWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dataContext = new Core.ViewModels.MessageBoxWindowViewModel(_msg, _textBoxMessage);
            DataContext = dataContext;

            if (dataContext.TextBoxMessage == "$unfilled$")
            {
                DisplayTextBox.Visibility = Visibility.Collapsed;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}