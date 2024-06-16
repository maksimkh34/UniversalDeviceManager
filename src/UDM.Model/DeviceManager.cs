// ReSharper disable InconsistentNaming

using System.Collections.ObjectModel;
using UDM.Model.LogService;

namespace UDM.Model
{
    public class DeviceManager
    {
        public ObservableCollection<DeviceConnection> DeviceConnections = new();

        public void UpdateFastbootDevices()
        {
            LogService.LogService.Log("Updating fastboot devices", LogLevel.Debug);
            var fastbootResult = SysCalls.Exec(MainModel.PathToFastboot, "fastboot.exe", "devices");
            foreach (var device in fastbootResult.Split("\r\n"))
            {
                if (device != "")
                {
                    var parsedDevice = DeviceConnection.Parse(device);
                    LogService.LogService.Log("New device: " + parsedDevice.DeviceToStr, LogLevel.Debug);
                    DeviceConnections.Add(parsedDevice);
                }
                
            }
        }

        public void UpdateDevices()
        {
            DeviceConnections.Clear();
            UpdateFastbootDevices();
        }
    }

    public class DeviceConnection(string id, DeviceConnectionType type)
    {
        public string Id { get; set; } = id;
        public DeviceConnectionType Type { get; set; } = type;

        public static DeviceConnection Parse(string unparsed)
        {
            return new DeviceConnection(unparsed.Split('\t')[0], Enum.Parse<DeviceConnectionType>(unparsed.Split('\t')[1]));
        }

        public string DeviceToStr => Id + $"\t({Type})";
    }

    public enum DeviceConnectionType
    {
        fastboot,
        ADB,
        Sideload,
        BROM,
        EDL
    }
}
