using System.Collections.ObjectModel;
using System.Windows.Input;
using UDM.Model;
using UDM.Model.LogService;

namespace UDM.Core.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Properties

        public ObservableCollection<LogEntry> Logs { get; } = LogService.Logs;  // Connected to LogService and Log text box in main window
        public ObservableCollection<DeviceConnection> Devices { get; } = Model.MainModel.ModelDeviceManager.DeviceConnections;  // Connected to LogService and Log text box in main window

        public ICommand UpdateDevicesCommand => Model.Commands.DeviceCommands.UpdateDevicesCommand;

        #endregion Properties
    }
}