using UDM.Model.SettingsService;

namespace UDM.Core.ViewModels
{
    internal static class MainModel
    {
        public static SettingsStorage SettingsStorage = new("settings_storage.conf");

        public const string SnForceDebugLogs = nameof(SnForceDebugLogs);

        public static void RegisterMainModel()
        {
            SettingsStorage.Register(new Setting(SnForceDebugLogs,
                false, typeof(bool),
            null, null, null,
            "Force debug logs to show", false, null));

            SettingsStorage.LoadSettings();
        }
    }
}