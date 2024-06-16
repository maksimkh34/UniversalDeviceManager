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
            LogService.Log("Unhandled exception captured. Logs will be saved to " + MainModel.LogPath, LogLevel.Fatal);
            LogService.Log(e.Exception.GetType() + ": " + e.Exception.Message, LogLevel.Fatal);
            LogService.Save(MainModel.LogPath);

            e.Handled = true;

            ShowWaitForInputWindow();

            Environment.Exit(0);
        }
    }
}