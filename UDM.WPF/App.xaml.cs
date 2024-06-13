using System.Windows;

namespace UDM.WPF
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Dialogs.MainWindow mw = new();
            mw.Show();
        }

        internal static void ShutdownApp()
        {
            Environment.Exit(0);
        }

    }

}
