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
    }
}
