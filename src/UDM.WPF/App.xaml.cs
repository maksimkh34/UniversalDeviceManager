using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using UDM.Core.ViewModels;
using UDM.Model;
using UDM.Model.LogService;
using UDM.Model.SettingsService;
using UDM.WPF.Dialogs;

namespace UDM.WPF
{
    public class MessageWindow(string message, string textBoxMessage = "$unfilled$")
    {
        private readonly MessageBoxWindow _window = new(message, textBoxMessage);

        public void Show() => _window.Show();
        public void ShowDialog() => _window.ShowDialog();
        public void Close() => _window.Close();
    }

    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var pythonDownloadWindow = new MessageWindow("Downloading python...");
            MainModel.RegisterMainModel(
                msgDialog: ShowMessage,
                executeCode: OpenPreDil,
                autoExecuteCode: AutoOpenPreDil,
                getImageStrAction: GetImagePath,
                getArchiveStrAction: GetArchivePath,
                changelogTitle: (string)FindResource("MsgChangelog")!,
                pythonDownloadMsgShow: pythonDownloadWindow.Show,
                pythonDownloadMsgClose: pythonDownloadWindow.Close,
                waitForInputDialog: ShowWaitForInputWindow,
                getUserInput: GetUserInput
                );
            LogService.Logs.Clear();    // ???
            // включить логи уровня дебаг на релизной сборке
            // MainModel.SettingsStorage.Set(MainModel.SnForceDebugLogs, true);

            MainWindow mw = new();
            mw.Show();

            MainModel.CheckStartup();

            MainModel.SettingsStorage.Get(MainModel.SnCurrentLanguage).UpdateValueChanged(UpdateLang);
            UpdateLang(new SettingChangedContext("", MainModel.SettingsStorage.GetValue(MainModel.SnCurrentLanguage) ?? "en-US"));

            LogService.Log(FindResource("MsgHello")?.ToString() ?? "lang_err", LogLevel.Info);
        }

        public static string GetImagePath()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = false,
                ReadOnlyChecked = false,
                CheckFileExists = false,
                AddExtension = true,
                DefaultExt = "img",
                Filter = "Image (*.img)|*.img|All files (*.*)|*.*",
                Title = "Select image to flash"
            };
            dialog.ShowDialog();

            return dialog.FileName;
        }

        public static string GetArchivePath()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = false,
                ReadOnlyChecked = false,
                CheckFileExists = false,
                AddExtension = true,
                DefaultExt = "zip",
                Filter = "ZIP archive (*.zip)|*.zip|All files (*.*)|*.*",
                Title = "Select archive to flash"
            };
            dialog.ShowDialog();

            return dialog.FileName;
        }

        public static bool? OpenPreDil()
        {
            var dialog = new PreDIL();
            return dialog.ShowDialog();
        }

        public static bool? AutoOpenPreDil()
        {
            var dialog = new PreDIL();
            dialog.Show();
            ((PreDILViewModel)dialog.DataContext).ExecNow();
            return true;
        }

        public static string? GetUserInput(string message)
        {
            var window = new UserInputWindow
            {
                DataContext = new UserInputWindowViewModel(message)
            };
            window.ShowDialog();
            var result = MainModel.CurrentUserInputFromUserInputWindow;
            MainModel.CurrentUserInputFromUserInputWindow = null;
            return result;
        }

        public static void ShutdownApp()
        {
            LogService.Save((string)(MainModel.SettingsStorage.GetValue(MainModel.SnLogPath) ?? "C:\\log.log"));
            MainModel.SettingsStorage.SaveSettings();
            Environment.Exit(0);
        }

        public static void ShowWaitForInputWindow()
        {
            WaitForInputWindow window = new();
            window.ShowDialog();
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (MainModel.IsDebugRelease) return;
            var msg1 = Resources["DialogUnhandledExceptionMsg"] + " " + (string)(MainModel.SettingsStorage.GetValue(MainModel.SnLogPath) ?? "C:\\log.log");
            var msg2 = e.Exception.GetType() + ": " + e.Exception.Message;
            LogService.Log(msg1, LogLevel.Fatal);
            LogService.Log(msg2, LogLevel.Fatal);

            e.Handled = true;
            if (e.Exception is System.ComponentModel.Win32Exception)
            {
                ShowMessage("Warning!", msg2 + FindResource("Win32ExceptionMsg"));
                return;
            }

            LogService.Save((string)(MainModel.SettingsStorage.GetValue(MainModel.SnLogPath) ?? "C:\\log.log"));
            ShowMessage(msg1, msg2);

            Environment.Exit(0);
        }

        public static void ShowMessage(string message, string textBoxMessage = "$unfilled$")
        {
            MessageBoxWindow window = new(message, textBoxMessage);
            window.ShowDialog();
        }

        private static void UpdateLang(SettingChangedContext context)
        {
            ResourceDictionary dict;
            try
            {
                dict = new ResourceDictionary
                {
                    Source = new Uri("Resource/Localization/" + context.NewValue + ".xaml", UriKind.Relative)
                };
            }
            catch (System.IO.IOException)
            {
                throw new Exception("Error loading files for " + context.NewValue + " locale. ");
            }

            foreach (var dictionary in Current.Resources.MergedDictionaries)
            {
                if (!dictionary.Source.ToString().Contains("Resource/Localization/")) continue;
                var ind = Current.Resources.MergedDictionaries.IndexOf(dictionary);
                Current.Resources.MergedDictionaries.Remove(dictionary);
                Current.Resources.MergedDictionaries.Insert(ind, dict);
                break;
            }
        }
    }
}