using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UDM.Model;
using UDM.Model.Commands;

namespace UDM.Core.ViewModels
{
    public class FlashFullRomViewModel(Action closeWindowAction) : BaseViewModel
    {
        public ICommand BrowseCommand { get; } = new DelegateCommand(BrowseAction, DelegateCommand.DefaultCanExecute);
        public ICommand ApplyCommand { get; set; } = new DelegateCommand(FlashAction, ActiveDeviceConnectedAndRomSelected, closeWindowAction);

        private string _selectedRomPath = MainModel.FileNotSelected;
        public string SelectedRomPath
        {
            get => _selectedRomPath;
            set
            {
                _selectedRomPath = value;
                OnPropertyChanged();
            }
        }

        public static Action? UpdateApplyCommand;

        public static void BrowseAction(object param)
        {
            var path = MainModelStatic.UiDialogManager?.GetDirectory("Select ROM dir");
            if (path is not null && path != "")
            {
                if (!File.Exists(path + "\\flash_all.bat") ||
                    !File.Exists(path + "\\flash_all_except_data_storage.bat") ||
                    !File.Exists(path + "\\flash_all_lock.bat")) { MainModelStatic.UiDialogManager?.ShowMsg("Error", "Invalid path! "); return; }

                Updater?.Invoke(path);
                
            }
        }

        public static void FlashAction(object param)
        {
            if (param is not IEnumerable<string> list) return;

            IEnumerable<string> enumerable = list as string[] ?? list.ToArray();
            var romPath = enumerable.ElementAt(0);
            string type = "d";

            switch(enumerable.ElementAt(1))
            {
                case "Flash": type = "f"; break;
                case "Flash and lock": type = "l"; break;
                case "Keep user files": type = "k"; break;
            }

            MainModel.CurrentScriptCode = $"flash_rom -{type} {romPath}";
            MainModelStatic.ModelExecuteCode?.Invoke();
        }

        public static bool ActiveDeviceConnectedAndRomSelected(object param)
        {
            return true;
            if (!MainModelStatic.ModelDeviceManager.IsActiveDeviceConnected() || MainModelStatic.ModelDeviceManager.ActiveDevice.Type != DeviceConnectionType.fastboot) return false;
            if (param is not IEnumerable<string> list) return false;
            return list.ElementAt(1) != MainModel.FileNotSelected;
        }

        public delegate void PathUpdater(string path);

        public static PathUpdater? Updater;
    }
}
