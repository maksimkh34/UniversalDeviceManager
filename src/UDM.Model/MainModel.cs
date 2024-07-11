using System.IO;
using UDM.Model.SettingsService;

namespace UDM.Model
{
    public static class MainModel
    {
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

        public static string CurrentScriptCode { get; set; } = NoCodeExecutedDefaultMsg;

        public const string SnForceDebugLogs = nameof(SnForceDebugLogs); // Sn - SettingName
        public const string SnLogPath = nameof(SnLogPath);
        public const string SnCurrentLanguage = nameof(SnCurrentLanguage);
        public const string NoCodeExecutedDefaultMsg = "No code is being executed.";

        public static void RegisterMainModel()
        {
            // Do not forget to update SettingsViewModel! 

            MainModelHelpers.SettingsStorage.Register(new Setting(SnForceDebugLogs,
                false, typeof(bool),
                null, null, null,
                "StForceDebugLogs", false, null));

            MainModelHelpers.SettingsStorage.Register(new Setting(SnCurrentLanguage,
                Languages[0], typeof(string),
                null, null, MainModelHelpers.LangChanged,
                "StCurrentLanguage", true, Languages));

            MainModelHelpers.SettingsStorage.Register(new Setting(SnLogPath,
                LocalEnvVars.LocalLogPath, typeof(string),
                ValidateLogPath, null, null,
                "StLogPath", false, null));

            MainModelHelpers.
                        SettingsStorage.LoadSettings();
        }

        public static string[] Languages = { "en-US", "ru-RU" };

        public static DeviceManager ModelDeviceManager = new();

        /// <summary>
        /// Проверяет, первый ли это запуск. Если первый, проводит инициализацию. Если не первый, пропускает действия
        /// </summary>
        public static void CheckStartup()
        {
            if(File.Exists(Cwd + @"\init"))
            {
                return;
            }
            else
            {
                // адреналин работай !!!
            }
        }

        public static bool ValidateLogPath(object value)
        {
            var path = Path.GetDirectoryName((string)value)!;
            if (path == string.Empty) return false;
            Directory.CreateDirectory(path);
            return File.Exists((string)value);
        }
    }
}