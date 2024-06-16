// ReSharper disable InconsistentNaming

using System.Collections.ObjectModel;

namespace UDM.Model
{
    public class DeviceManager
    {
        public ObservableCollection<DeviceConnection> DeviceConnections = new();

        public void UpdateFastbootDevices()
        {
            var fastbootResult = SysCalls.Exec(MainModel.PathToFastboot, "fastboot.exe", "devices");
            foreach (var device in fastbootResult.Split("\r\n"))
            {
                if (device != "")
                {
                    DeviceConnections.Add(DeviceConnection.Parse(device));
                }
                
            }
        }

        public void UpdateDevices()
        {
            DeviceConnections.Clear();
            UpdateFastbootDevices();
        }

        public ObservableCollection<DeviceConnection> ClearDevices(ObservableCollection<DeviceConnection> collection, DeviceConnectionType connectionType)
        {
            ObservableCollection<DeviceConnection> connections = new();
            foreach (var device in collection)
            {
                if (device.Type != connectionType)
                {
                    connections.Add(device);
                }
            }
            return connections;
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
