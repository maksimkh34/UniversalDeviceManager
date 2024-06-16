using System.Windows;

namespace UDM.WPF.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WaitForInputWindow.xaml
    /// </summary>
    public partial class WaitForInputWindow : Window
    {
        public WaitForInputWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}