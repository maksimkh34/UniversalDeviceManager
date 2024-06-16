using System.Windows.Input;

namespace UDM.Model.Commands
{
    public static class DeviceCommands
    {

        #region Commands

        public static ICommand UpdateDevicesCommand = new DelegateCommand(UpdateDevices, DelegateCommand.DefaultCanExecute);


        #endregion

        #region Command Funcs

        public static void UpdateDevices(object param)
        {
            MainModel.ModelDeviceManager.UpdateDevices();
        }

        #endregion
    }
}
