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

        public static void DisconnectDevice(object param)
        {
            if (param is string id)
            {
                MainModel.ModelDeviceManager.Disconnect(id);
            }
        }

        public static void SelectDevice(object param)
        {
            if (param is string id)
            {
                MainModel.ModelDeviceManager.Select(id);
            }
        }

        #endregion Command Funcs
    }
}