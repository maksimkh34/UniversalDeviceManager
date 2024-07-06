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

        public static string PathToFastboot
        {
            get
            {
                if (IsDebugRelease)
                {
                    //return LocalEnvVars.LocalFastbootPath;
                }
                return Directory.GetCurrentDirectory() + @"fastboot\";
            }
        }

        public const string LogPath = "D:\\log.log";
        public const string SnForceDebugLogs = nameof(SnForceDebugLogs); // Sn - SettingName
        public const string SnCurrentLanguage = nameof(SnCurrentLanguage);

        public static void RegisterMainModel()
        {
            MainModelHelpers.SettingsStorage.Register(new Setting(SnForceDebugLogs,
                false, typeof(bool),
                null, null, null,
                "Force debug logs to show", false, null));
            MainModelHelpers.SettingsStorage.Register(new Setting(SnCurrentLanguage,
                Languages[0], typeof(string),
                null, null, MainModelHelpers.LangChanged,
                "App language", true, Languages));


            MainModelHelpers.
                        SettingsStorage.LoadSettings();
        }

        public static string[] Languages = { "en-US", "ru-RU", "te-ST" };

        public static DeviceManager ModelDeviceManager = new();
    }
}
