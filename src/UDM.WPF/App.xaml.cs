﻿using System.Windows;
using System.Windows.Threading;
using UDM.Model;
using UDM.Model.LogService;
using UDM.Model.SettingsService;
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

            MainModelHelpers.SettingsStorage.Get(MainModel.SnCurrentLanguage).UpdateValueChanged(UpdateLang);
            UpdateLang(new SettingChangedContext("", MainModelHelpers.SettingsStorage.GetValue(MainModel.SnCurrentLanguage) ?? "en-US"));
        }

        public static void ShutdownApp()
        {
            LogService.Save((string)(MainModelHelpers.SettingsStorage.GetValue(MainModel.SnLogPath) ?? "C:\\log.log"));
            // MainModelHelpers.SettingsStorage.SaveSettings();
            Environment.Exit(0);
        }

        public static void ShowWaitForInputWindow()
        {
            WaitForInputWindow window = new();
            window.ShowDialog();
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var msg1 = Resources["DialogUnhandledExceptionMsg"] + " " +(string)(MainModelHelpers.SettingsStorage.GetValue(MainModel.SnLogPath) ?? "C:\\log.log");
            var msg2 = e.Exception.GetType() + ": " + e.Exception.Message;
            LogService.Log(msg1, LogLevel.Fatal);
            LogService.Log(msg2, LogLevel.Fatal);
            LogService.Save((string)(MainModelHelpers.SettingsStorage.GetValue(MainModel.SnLogPath) ?? "C:\\log.log"));

            e.Handled = true;

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
            catch(System.IO.IOException)
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