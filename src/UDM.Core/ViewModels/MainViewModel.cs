﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using UDM.Model;
using UDM.Model.Commands;
using UDM.Model.LogService;
using UDM.Model.MainModelSpace;

namespace UDM.Core.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Properties

        public ObservableCollection<LogEntry> Logs { get; } = LogService.Logs;  // Connected to LogService and Log text box in main window
        public ObservableCollection<DeviceConnection> Devices { get; } = MainModelStatic.ModelDeviceManager.DeviceConnections;  // Connected to LogService and Log text box in main window

        public ICommand UpdateDevicesCommand => DeviceCommands.UpdateDevicesCommand;

        public DeviceConnection Connection => MainModelStatic.ModelDeviceManager.ActiveDevice;

        #endregion Properties

        public static DelegateCommand InstallPythonCommand { get; } =
            new(CommonCommands.InstallPython, DelegateCommand.DefaultCanExecute);
    }
}