using System.Windows;
using UDM.Model.Log;

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

        public static void ShutdownApp()
        {
            LogService.Save("D:\\log.log");
            Environment.Exit(0);
        }

    }

}
