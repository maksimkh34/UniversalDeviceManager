using System.ComponentModel;
using UDM.Model.DIL;
using UDM.Model.LogService;
using UDM.Model.SettingsService;

namespace UDM.Model
{
    public static class MainModel
    {
        public static SettingsStorage SettingsStorage = new(SettingsConfFilePath);
        public static SettingChanged? LangChanged;

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

        public static string PathToFastboot
        {
            get
            {
                if (IsDebugRelease)
                {
                    return LocalEnvVars.LocalFastbootPath;
                }
                return Directory.GetCurrentDirectory() + @"\fastboot\";
            }
        }

        public static string CurrentScriptCode
        {
            get;
            set;
        } = NoCodeExecutedDefaultMsg;

        public const string SnForceDebugLogs = nameof(SnForceDebugLogs); // Sn - SettingName
        public const string SnLogPath = nameof(SnLogPath);
        public const string SnCurrentLanguage = nameof(SnCurrentLanguage);

        public const string NoCodeExecutedDefaultMsg = "No code is being executed.";

        public const string ChangelogPath = @"\changelog";
        public const string InitFilePath = @"\config\init";
        public const string SettingsConfFilePath = @"\config\settings_storage.conf";

        public const string FirstInstallScriptPath = @"\python\install.py";

        public delegate void ChangelogDialog(string titleText, string textboxText);
        public delegate bool? ExecuteCode();
        public delegate string GetPath();

        public static bool ChangelogFound;

        public static ChangelogDialog? UiChangelogDialog;
        public static ExecuteCode? ModelExecuteCode;
        public static GetPath? GetImagePath;
        public static string? ChangelogTitle;

        public static void RegisterMainModel(ChangelogDialog changelogDialog, ExecuteCode executeCode, GetPath getImagePath, string changelogTitle)
        {
            UiChangelogDialog = changelogDialog;
            ChangelogTitle = changelogTitle;
            ModelExecuteCode = executeCode;
            GetImagePath = getImagePath;

            // Do not forget to update SettingsViewModel! 

            MainModel.SettingsStorage.Register(new Setting(SnForceDebugLogs,
                false, typeof(bool),
                null, null, null,
                "StForceDebugLogs", false, null));

            MainModel.SettingsStorage.Register(new Setting(SnCurrentLanguage,
                Languages[0], typeof(string),
                null, null, MainModel.LangChanged,
                "StCurrentLanguage", true, Languages));

            MainModel.SettingsStorage.Register(new Setting(SnLogPath,
                Cwd + @"\Logs.log", typeof(string),
                ValidateLogPath, null, null,
                "StLogPath", false, null));

            MainModel.
                        SettingsStorage.LoadSettings();
        }

        public static string[] Languages = { "en-US", "ru-RU" };

        public static DeviceManager ModelDeviceManager = new();

        /// <summary>
        /// Проверяет, первый ли это запуск. Если первый, проводит инициализацию. Если не первый, пропускает действия
        /// </summary>
        public static void CheckStartup()
        {
            LogService.LogService.Log("CheckStartup Running!", LogLevel.Debug);
            if (File.Exists(Cwd + ChangelogPath))
            {
                ChangelogFound = true;
                UiChangelogDialog?.Invoke(ChangelogTitle ?? "Changelog", File.ReadAllText(Cwd + ChangelogPath));
                File.Delete(Cwd + ChangelogPath);
            }

            if (File.Exists(Cwd + InitFilePath)) return;
            LogService.LogService.Log("Execution python install...", LogLevel.Debug);

            InteractionService.InteractionService service = new(Cwd + FirstInstallScriptPath);
            service.Run();
            LogService.LogService.Log(service.Read(), LogLevel.OuterServices);
            File.Create(Cwd + InitFilePath);
        }

        public static bool ValidateLogPath(object value)
        {
            var path = Path.GetDirectoryName((string)value)!;
            if (path == string.Empty) return false;
            Directory.CreateDirectory(path);
            File.Create(value.ToString()!).Close();
            return File.Exists((string)value);
        }

        public static void CheckBlStatus()
        {
            DeviceInteractionLanguage.Execute("fastboot_check_bl");
        }

        public static string ReplaceCodeWars(string code)
        {
            return code.Replace("%cwd%", Cwd);
        }
    }
}