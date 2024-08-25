using UDM.Model;
using UDM.Model.LogService;
using UDM.Model.SettingsService;

namespace UDM.Core.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public delegate void ShowMsgDelegate(string msg, string textBoxMsg = "$unfilled$");

        public ShowMsgDelegate? ShowMsg;
        public string? InvalidPathMsg;

        public string GetPropertyNameBySettingName(string settingName)
        {
            return settingName switch
            {
                MainModel.SnForceDebugLogs => nameof(ForceDebugLogs),
                MainModel.SnCurrentLanguage => nameof(CurrentLanguageIndex),
                MainModel.SnLogPath => nameof(LogPath),
                _ => throw new KeyNotFoundException("Setting was not found")
            };
        }

        public bool ForceDebugLogs
        {
            get => (bool)(MainModel.SettingsStorage.GetValue(nameof(MainModel.SnForceDebugLogs)) ?? false);
            set
            {
                MainModel.SettingsStorage.Set(nameof(MainModel.SnForceDebugLogs), value);
                OnPropertyChanged();
            }
        }

        public int CurrentLanguageIndex
        {
            get
            {
                var lang = MainModel.SettingsStorage.GetValue(nameof(MainModel.SnCurrentLanguage));
                var index = 0;
                foreach (var s in MainModelStatic.Languages)
                {
                    if ((string)lang! == s)
                    {
                        return index;
                    }

                    index += 1;
                }

                return -1;
            }
            set => MainModel.SettingsStorage.Set(nameof(MainModel.SnCurrentLanguage), MainModelStatic.Languages[value]);
        }

        public string LogPath
        {
            get => (string)(MainModel.SettingsStorage.GetValue(nameof(MainModel.SnLogPath)) ?? "-error loading path-");
            set
            {
                try
                {
                    MainModel.SettingsStorage.Set(nameof(MainModel.SnLogPath), value);
                }
                catch (SettingsExceptions.InvalidSettingValueType)
                {
                    LogService.Log("Invalid Log Path provided. ", LogLevel.Warning);
                    ShowMsg?.Invoke(InvalidPathMsg ?? "Invalid path provided! ", value);
                }
                OnPropertyChanged();
            }
        }
    }
}

