using System.IO.Compression;
using System.Net;
using UDM.Model;
using UDM.Model.DIL;
using UDM.Model.LogService;
using UDM.Model.SettingsService;

namespace UDM.Model
{
    public static class MainModelStatic
    {
        public static string? CurrentUserInputFromUserInputWindow;
        public static string? ChangelogTitle;

        public static bool ChangelogFound;

        public static MainModel.ExecuteCode? ModelExecuteCode;

        public static MainModel.ExecuteCode? AutoExecuteCode;

        public static UiDialogManager? UiDialogManager;

        public static TranslationService? TranslationService;

        public static FileStream? ConfigFileLock;

        public static FileStream? InitFileLock;

        public static string[] Languages = { "en-US", "ru-RU" };

        public static DeviceManager ModelDeviceManager = new();
    }
}