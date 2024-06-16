using UDM.Model.SettingsService;

namespace UDM.Model;

internal static class MainModelHelpers
{

    public static SettingsStorage SettingsStorage = new("settings_storage.conf");
}