using System.Windows.Input;
using UDM.Model;
using UDM.Model.Commands;
using UDM.Model.MainModelSpace;

namespace UDM.Core.ViewModels
{
    public class FlashFullRomViewModel(Action closeWindowAction) : BaseViewModel
    {
        public ICommand BrowseCommand { get; } = new DelegateCommand(BrowseAction, DelegateCommand.DefaultCanExecute);
        public ICommand ApplyCommand { get; set; } = new DelegateCommand(FlashAction, ActiveDeviceConnectedAndRomSelected, closeWindowAction);

        private string _selectedRomPath = MainModelStatic.FileNotSelected;
        public string SelectedRomPath
        {
            get => _selectedRomPath;
            set
            {
                _selectedRomPath = value;
                OnPropertyChanged();
            }
        }

        public static void BrowseAction(object param)
        {
            var path = MainModelStatic.UiDialogManager?.GetDirectory("Select ROM dir");
            if (path is null or "") return;
            if (!File.Exists(path + "\\flash_all.bat") ||
                !File.Exists(path + "\\flash_all_except_data_storage.bat") ||
                !File.Exists(path + "\\flash_all_lock.bat")) { MainModelStatic.UiDialogManager?.ShowMsg("Error", "Invalid path! "); return; }

            Updater?.Invoke(path);
        }

        public static void FlashAction(object param)
        {
            if (param is not IEnumerable<string> list) return;

            IEnumerable<string> enumerable = list as string[] ?? list.ToArray();
            var romPath = enumerable.ElementAt(0);

            var type = enumerable.ElementAt(1) switch
            {
                "Flash" => "f",
                "Flash and lock" => "l",
                "Keep user files" => "k",
                _ => "d"
            };

            MainModelStatic.CurrentScriptCode = $"flash_rom -{type} {romPath}";
            MainModelStatic.ModelExecuteCode?.Invoke();
        }

        public static bool ActiveDeviceConnectedAndRomSelected(object param)
        {
            return true;
            if (!MainModelStatic.ModelDeviceManager.IsActiveDeviceConnected() || MainModelStatic.ModelDeviceManager.ActiveDevice.Type != DeviceConnectionType.fastboot) return false;
            if (param is not IEnumerable<string> list) return false;
            return list.ElementAt(1) != MainModelStatic.FileNotSelected;
        }

        public delegate void PathUpdater(string path);

        public static PathUpdater? Updater;
    }
}
