// ReSharper disable InconsistentNaming

using System.Collections.ObjectModel;
using System.Windows.Input;
using UDM.Model.Commands;
using UDM.Model.LogService;

namespace UDM.Model
{
    // Never remove devices from list via DeviceConnections.Remove! Use Disconnect(id)!
    public class DeviceManager
    {
        public ObservableCollection<DeviceConnection> DeviceConnections = new();
        public int SelectedDeviceIndex = -1;

        public DeviceConnection SelectedDevice
        {
            get => SelectedDeviceIndex == -1 ? new DeviceConnection() : DeviceConnections[SelectedDeviceIndex];
            set
            {
                var index = 0;
                foreach (var device in DeviceConnections)
                {
                    if (device.Id == value.Id)
                    {
                        SelectedDeviceIndex = index;
                    }
                    index += 1;
                }
            }
        }

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
        }

        public void UpdateDevices()
        {
            DeviceConnections.Clear();
            UpdateFastbootDevices();
        }

        public void Disconnect(string id)
        {
            foreach (var device in DeviceConnections)
            {
                if (device.Id != id) continue;
                DeviceConnections.Remove(device);
                break;
            }
            SelectedDevice = new DeviceConnection();
        }

        public void Select(string id)
        {
            foreach (var device in DeviceConnections)
            {
                if (device.Id != id) continue;
                SelectedDevice = device;
                break;
            }
        }
    }

    public class DeviceConnection(string id = "disconnected_id", DeviceConnectionType type = DeviceConnectionType.Disconnected)
    {
        public string Id { get; set; } = id;
        public DeviceConnectionType Type { get; set; } = type;

        public static DeviceConnection Parse(string unparsed)
        {
            return new DeviceConnection(unparsed.Split('\t')[0], Enum.Parse<DeviceConnectionType>(unparsed.Split('\t')[1]));
        }

        public string DeviceToStr => Id + $"\t({Type})";

        public ICommand DisconnectCommand { get; }= new DelegateCommand(DeviceCommands.DisconnectDevice, DelegateCommand.DefaultCanExecute);
        public ICommand SelectCommand { get; } = new DelegateCommand(DeviceCommands.SelectDevice, DelegateCommand.DefaultCanExecute);
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
}
