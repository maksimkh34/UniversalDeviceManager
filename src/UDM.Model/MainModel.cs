﻿using System.IO.Compression;
using System.Net;
using UDM.Model.DIL;
using UDM.Model.LogService;
using UDM.Model.SettingsService;

namespace UDM.Model
{
    public static class MainModel
    {
        public static SettingsStorage SettingsStorage = new(Cwd + SettingsConfFilePath);
        public static SettingChanged? LangChanged;

        public static Dictionary<string, string> Vars = new();

        public static bool IsDebugRelease
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public static string Cwd
        {
            get
            {
#if DEBUG
                return LocalEnvVars.LocalCwd;
#else
                return Directory.GetCurrentDirectory();
#endif
            }
        }

        public static string PathToPlatformtools
        {
            get
            {
                if (IsDebugRelease)
                {
                    return LocalEnvVars.LocalPlatformtoolsPath;
                }
                return Directory.GetCurrentDirectory() + @"\fastboot\";
            }
        }

        public static string CurrentScriptCode
        {
            get;
            set;
        } = NoCodeExecutedDefaultMsg;
        public static int MaxPathLength { get => maxPathLength; set => maxPathLength = value; }

        // In GUI, there are shorters (if path more than this value, it will start
        // with "..." and last (MaxPathLength-3) symbols of provided path
        private static int maxPathLength = 35;

        public const string SnForceDebugLogs = nameof(SnForceDebugLogs); // Sn - SettingName
        public const string SnLogPath = nameof(SnLogPath);
        public const string SnCurrentLanguage = nameof(SnCurrentLanguage);

        public const string NoCodeExecutedDefaultMsg = "No code is being executed.";
        public const string FileNotSelected = "File not selected";

        public const string ChangelogPath = @"\changelog";
        public const string InitFilePath = @"\config\init";
        public const string PythonWd = @"\python";
        public const string SettingsConfFilePath = @"\config\settings_storage.conf";
        public const string FirstInstallScriptPath = @"\python\install.py";
        // ReSharper disable once InconsistentNaming
        public const string FirstInstallDILScriptPath = @"\script\install.dil";
        public const string InstallPythonScriptPath = @"\script\py_installer.dil";
        public static readonly string PathToPython = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Programs\Python\Python312-32\python.exe";

        public delegate void WaitForInputDialog();
        public delegate bool? ExecuteCode();
        public delegate string GetStrAction();
        public delegate string? GetStrActionMsg(string msg);
        public delegate string[] GetFilesDelegate();

        public delegate void MsgWindowAction();

        public static void ExitMainModel()
        {
            MainModelStatic.InitFileLock?.Close();
            MainModelStatic.ConfigFileLock?.Close();
            SettingsStorage.SaveSettings();
        }

        public static void RegisterMainModel(ExecuteCode preExecuteCodeAction, ExecuteCode autoExecuteCodection, UiDialogManager uiManager,
            string changelogTitle, TranslationService translationService)
        {
            MainModelStatic.ChangelogTitle = changelogTitle;
            MainModelStatic.ModelExecuteCode = preExecuteCodeAction;
            MainModelStatic.AutoExecuteCode = autoExecuteCodection;
            MainModelStatic.UiDialogManager = uiManager;
            MainModelStatic.TranslationService = translationService;

            try {
                MainModelStatic.InitFileLock = File.Open(Cwd + InitFilePath, FileMode.Open);
                MainModelStatic.ConfigFileLock = File.Open(Cwd + SettingsConfFilePath, FileMode.Open);
            } catch (FileNotFoundException) { }

            // Do not forget to update SettingsViewModel! 

            SettingsStorage.Register(new Setting(SnForceDebugLogs,
                false, typeof(bool),
                null, null, null,
                "StForceDebugLogs", false, null));

            SettingsStorage.Register(new Setting(SnCurrentLanguage,
MainModelStatic.Languages[0], typeof(string),
                null, null, LangChanged,
                "StCurrentLanguage", true, MainModelStatic.Languages));

            SettingsStorage.Register(new Setting(SnLogPath,
                Cwd + @"\Logs.log", typeof(string),
ModelCore.ValidateLogPath, null, null,
                "StLogPath", false, null));

            MainModelStatic.
                        ConfigFileLock?.Close();
            SettingsStorage.LoadSettings();
            MainModelStatic.ConfigFileLock = File.Open(Cwd + SettingsConfFilePath, FileMode.Open);
        }

        /// <summary>
        /// Проверяет, первый ли это запуск. Если первый, проводит инициализацию. Если не первый, пропускает действия
        /// </summary>
        public static void CheckStartup()
        {
            LogService.LogService.Log("CheckStartup Running!", LogLevel.Debug);

            Directory.CreateDirectory(Cwd + @"\config");
            // Changelog
            if (File.Exists(Cwd + ChangelogPath))
            {
                MainModelStatic.ChangelogFound = true;
                MainModelStatic.UiDialogManager?.ShowMsg(MainModelStatic.ChangelogTitle ?? "Changelog", File.ReadAllText(Cwd + ChangelogPath));
                File.Delete(Cwd + ChangelogPath);
            }

            // python install
            if (File.Exists(Cwd + InitFilePath)) return;

            LogService.LogService.Log("Executing first install...", LogLevel.Debug);

            CurrentScriptCode = File.ReadAllText(Cwd + FirstInstallDILScriptPath);
            MainModelStatic.AutoExecuteCode?.Invoke();

            File.Create(Cwd + InitFilePath);
        }

        public static void CheckBlStatus() => DeviceInteractionLanguage.Execute("fastboot_check_bl");
    }
}