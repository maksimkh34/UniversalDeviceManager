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
        public const string SnForceDebugLogs = nameof(SnForceDebugLogs);

        public static void RegisterMainModel()
        {
            MainModelHelpers.SettingsStorage.Register(new Setting(SnForceDebugLogs, 
                false, typeof(bool), 
            null, null, null, 
            "Force debug logs to show", false, null));
            MainModelHelpers.
                        SettingsStorage.LoadSettings();
        }

        public static DeviceManager ModelDeviceManager = new();
    }
}
