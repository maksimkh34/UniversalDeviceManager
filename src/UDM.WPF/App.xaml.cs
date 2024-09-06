using System.Windows;
using System.Windows.Threading;
using UDM.Core.ViewModels;
using UDM.Model;
using UDM.Model.LogService;
using UDM.Model.MainModelSpace;
using UDM.Model.SettingsService;
using UDM.WPF.Dialogs;

namespace UDM.WPF
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var translationService = new TranslationService((string key) => (string)FindResource(key));
            var dialogManager = new UiDialogManager(
                getFile: GetFileAction,
                getFileArr: GetFilesAction,
                getDirParam: GetDirectoryAction,
                getUserInput: GetUserInput,
                waitForInput: ShowWaitForInputWindow,
                msgDialog: ShowMessage
                );

            MainModel.RegisterMainModel(
                preExecuteCodeAction: OpenPreDil,
                autoExecuteCodection: AutoOpenPreDil,
                changelogTitle: translationService.Get("MsgChangelog")!,
                uiManager: dialogManager,
                translationService: translationService
                );
            LogService.Logs.Clear();    // ???
            // включить логи уровня дебаг на релизной сборке
            // MainModel.SettingsStorage.Set(MainModel.SnForceDebugLogs, true);

            MainWindow mw = new();
            mw.Show();

            MainModel.CheckStartup();

            MainModel.SettingsStorage.Get(MainModelStatic.SnCurrentLanguage).UpdateValueChanged(UpdateLang);
            UpdateLang(new SettingChangedContext("", MainModel.SettingsStorage.GetValue(MainModelStatic.SnCurrentLanguage) ?? "en-US"));

            LogService.Log(translationService.Get("MsgHello") ?? "lang_err", LogLevel.Info);
        }

        public static string GetFileAction(string title, string filter)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = false,
                ReadOnlyChecked = false,
                CheckFileExists = false,
                AddExtension = true,
                Filter = filter,
                Title = title
            };
            dialog.ShowDialog();

            return dialog.FileName;
        }

        public static string[] GetFilesAction(string title, string filter)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                ReadOnlyChecked = false,
                CheckFileExists = false,
                AddExtension = true,
                Filter = filter,
                Title = title
            };
            dialog.ShowDialog();

            return dialog.FileNames;
        }

        public static string GetDirectoryAction(string title)
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog
            {
                Multiselect = false,
                Title = title
            };
            dialog.ShowDialog();

            return dialog.FolderName;
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

        public static string GetUserInput(string message)
        {
            var window = new UserInputWindow
            {
                DataContext = new UserInputWindowViewModel(message)
            };
            window.ShowDialog();
            var result = MainModelStatic.CurrentUserInputFromUserInputWindow;
            MainModelStatic.CurrentUserInputFromUserInputWindow = null;
            return result ?? "no input provided";
        }

        public static void ShutdownApp()
        {
            LogService.Save((string)(MainModel.SettingsStorage.GetValue(MainModelStatic.SnLogPath) ?? "C:\\log.log"));
            MainModel.ExitMainModel();
            Environment.Exit(0);
        }

        public static void ShowWaitForInputWindow() => new WaitForInputWindow().ShowDialog();

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (MainModelStatic.IsDebugRelease) return;
            var msg1 = Resources["DialogUnhandledExceptionMsg"] + " " + (string)(MainModel.SettingsStorage.GetValue(MainModelStatic.SnLogPath) ?? "C:\\log.log");
            var msg2 = e.Exception.GetType() + ": " + e.Exception.Message;
            LogService.Log(msg1, LogLevel.Fatal);
            LogService.Log(msg2, LogLevel.Fatal);

            e.Handled = true;
            if (e.Exception is System.ComponentModel.Win32Exception)
            {
                ShowMessage("Warning!", msg2 + MainModelStatic.TranslationService?.Get("Win32ExceptionMsg"));
                return;
            }

            LogService.Save((string)(MainModel.SettingsStorage.GetValue(MainModelStatic.SnLogPath) ?? "C:\\log.log"));
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