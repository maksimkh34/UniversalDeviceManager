using System.Windows;
using System.Windows.Threading;
using UDM.Model;
using UDM.Model.LogService;
using UDM.WPF.Dialogs;

namespace UDM.WPF
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainModel.RegisterMainModel();
            // включить логи уровня дебаг на релизной сборке
            // MainModel.SettingsStorage.Set(MainModel.SnForceDebugLogs, true);

            MainWindow mw = new();
            mw.Show();
        }

        public static void ShutdownApp()
        {
            LogService.Save(MainModel.LogPath);
            Environment.Exit(0);
        }

        public static void ShowWaitForInputWindow()
        {
            WaitForInputWindow window = new();
            window.ShowDialog();
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            const string msg1 = "Unhandled exception captured. Logs will be saved to " + MainModel.LogPath;
            var msg2 = e.Exception.GetType() + ": " + e.Exception.Message;
            LogService.Log(msg1, LogLevel.Fatal);
            LogService.Log(msg2, LogLevel.Fatal);
            LogService.Save(MainModel.LogPath);

            e.Handled = true;

            ShowMessage(msg1, msg2);

            Environment.Exit(0);
        }

        public static void ShowMessage(string message, string textBoxMessage= "$unfilled$")
        {
            MessageBoxWindow window = new(message, textBoxMessage);
            window.ShowDialog();
        }
    }
}