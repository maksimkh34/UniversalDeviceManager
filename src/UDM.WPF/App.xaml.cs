using System.Windows;
using UDM.Model.LogService;
using UDM.WPF.Dialogs;

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

        public static void ShowWaitForInputWindow()
        {
            WaitForInputWindow window = new();
            window.ShowDialog();
        }
    }

}
