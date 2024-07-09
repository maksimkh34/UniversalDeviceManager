// ReSharper disable InconsistentNaming

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using UDM.Model.Commands;
using UDM.Model.LogService;

namespace UDM.Model
{
    // Never remove devices from list via DeviceConnections.Remove! Use Disconnect(id)!
    public class DeviceManager
    {
        public ObservableCollection<DeviceConnection> DeviceConnections = new();

        private readonly DeviceConnection _connection = new();

        public DeviceConnection SelectedDevice
        {
            get => _connection;
            set
            {
                if (value.Type == DeviceConnectionType.Disconnected)
                {
                    if (_connection.Type == DeviceConnectionType.Disconnected) return;
                    LogService.LogService.Log("Resetting selected device", LogLevel.Debug);
                    _connection.Id = "";
                    _connection.Type = DeviceConnectionType.Disconnected;
                    return;
                }

                if (!DeviceConnections.Contains(value)) throw new DeviceDisconnectedException("Selected device is not connected. ");
                _connection.Id = value.Id;
                _connection.Type = value.Type;
            }
        }

        public bool DeviceConnected(string id)
        {
            return DeviceConnections.Any(device => device.Id == id);
        }

        public bool ActiveDeviceConnected() => DeviceConnected(SelectedDevice.Id);

        public void UpdateFastbootDevices()
        {
            LogService.LogService.Log("Updating fastboot devices", LogLevel.Debug);
            var fastbootResult = SysCalls.Exec(MainModel.PathToFastboot, "fastboot.exe", "devices");
            foreach (var device in fastbootResult.Split("\r\n"))
            {
                if (device == "") continue;
                var parsedDevice = DeviceConnection.Parse(device);
                LogService.LogService.Log("New device: " + parsedDevice.DeviceToStr, LogLevel.Debug);
                DeviceConnections.Add(parsedDevice);
            }

            if (!DeviceConnected(SelectedDevice.Id))
            {
                SelectedDevice = new DeviceConnection();
            }
        }

        public void UpdateDevices()
        {
            DeviceConnections.Clear();
            UpdateFastbootDevices();
        }

        public void Disconnect(string id)
        {
            UpdateDevices();
            if (!DeviceConnected(id))
            {
                MainModel.ModelDeviceManager.UpdateDevices(); return;
            }

            foreach (var device in DeviceConnections)
            {
                if (device.Id != id) continue;
                LogService.LogService.Log("Disconnecting " + device.Id + "...", LogLevel.Debug);
                if (device.DeviceToStr == SelectedDevice.DeviceToStr)
                {
                    SelectedDevice = new DeviceConnection();
                }
                DeviceConnections.Remove(device);
                break;
            }
            SelectedDevice = new DeviceConnection();
        }

        public void Select(string id)
        {
            UpdateDevices();
            if (!DeviceConnected(id))
            {
                MainModel.ModelDeviceManager.UpdateDevices(); return;
            }

            foreach (var device in DeviceConnections)
            {
                if (device.Id != id) continue;
                LogService.LogService.Log("Selected: " + device.Id + "...", LogLevel.Info);
                SelectedDevice = device;
                break;
            }
        }
    }

    public class DeviceConnection(string id = "disconnected_id", DeviceConnectionType type = DeviceConnectionType.Disconnected) : INotifyPropertyChanged
    {
        public string Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }

        public DeviceConnectionType Type
        {
            get => type;
            set
            {
                type = value;
                OnPropertyChanged();
            }
        }

        public static DeviceConnection Parse(string unparsed)
        {
            return new DeviceConnection(unparsed.Split('\t')[0], Enum.Parse<DeviceConnectionType>(unparsed.Split('\t')[1]));
        }

        public string DeviceToStr => Type == DeviceConnectionType.Disconnected ? "Disconnected" : (Id + $"\t({Type})");

        public ICommand DisconnectCommand { get; } = new DelegateCommand(DeviceCommands.DisconnectDevice, DelegateCommand.DefaultCanExecute);

        public ICommand SelectCommand { get; } = new DelegateCommand(DeviceCommands.SelectDevice, DelegateCommand.DefaultCanExecute);

        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum DeviceConnectionType
    {
        fastboot,
        ADB,
        Sideload,
        BROM,
        EDL,
        Disconnected
    }

    public class DeviceDisconnectedException : Exception
    {
        public DeviceDisconnectedException()
        {
        }

        public DeviceDisconnectedException(string message)
            : base(message)
        {
        }

        public DeviceDisconnectedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}