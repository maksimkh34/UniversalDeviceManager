using System.Windows.Input;
using UDM.Model.MainModelSpace;

namespace UDM.Model.Commands
{
    public static class DeviceCommands
    {
        #region Commands

        public static ICommand UpdateDevicesCommand = new DelegateCommand(UpdateDevices, DelegateCommand.DefaultCanExecute);

        #endregion Commands

        #region Command Funcs

        public static void UpdateDevices(object param)
        {
            MainModelStatic.ModelDeviceManager.UpdateDevices();
        }

        public static void SelectDevice(object param)
        {
            if (param is string id)
            {
                MainModelStatic.ModelDeviceManager.Select(id);
            }
        }

        public static bool ActiveDeviceConnected(object param)
        {
            return MainModelStatic.ModelDeviceManager.IsActiveDeviceConnected() &&
MainModelStatic.ModelDeviceManager.ActiveDevice.Type == DeviceConnectionType.fastboot;
        }

        public static void ExecuteCode(object param)
        {
            if (param is not string mode) return;
            MainModelStatic.CurrentScriptCode = $"fastboot_reboot {(mode == "system" ? string.Empty : mode)}";
            MainModelStatic.ModelExecuteCode?.Invoke();
        }

        #endregion Command Funcs
    }
}