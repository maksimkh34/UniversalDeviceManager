using UDM.Model.SettingsService;

namespace UDM.Model
{
    public static class MainModelHelpers
    {
        public static SettingsStorage SettingsStorage = new("settings_storage.conf");

        public static SettingChanged? LangChanged;
    }
}