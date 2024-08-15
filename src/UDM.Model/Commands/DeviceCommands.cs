using System.Windows.Input;

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
            MainModel.ModelDeviceManager.UpdateDevices();
        }

        public static void SelectDevice(object param)
        {
            if (param is string id)
            {
                MainModel.ModelDeviceManager.Select(id);
            }
        }

        public static bool ActiveDeviceConnected(object param)
        {
            return MainModel.ModelDeviceManager.IsActiveDeviceConnected() &&
                   MainModel.ModelDeviceManager.ActiveDeviceType == DeviceConnectionType.fastboot;
        }

        public static void ExecuteCode(object param)
        {
            if (param is not string mode) return;
            MainModel.CurrentScriptCode = $"fastboot_reboot {(mode == "system" ? string.Empty : mode)}";
            MainModel.ModelExecuteCode?.Invoke();
        }

        #endregion Command Funcs
    }
}