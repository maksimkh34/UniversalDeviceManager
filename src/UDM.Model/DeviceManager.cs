﻿// ReSharper disable InconsistentNaming

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using UDM.Model.Commands;
using UDM.Model.LogService;
using UDM.Model.MainModelSpace;

namespace UDM.Model
{
    // Never remove devices from list via DeviceConnections.Remove! Use Disconnect(id)!
    public class DeviceManager
    {
        public const string Disconnected_id = "disconnected_id";

        public const bool EmulateDevice = true;

        public ObservableCollection<DeviceConnection> DeviceConnections = [];
        private readonly DeviceConnection _connection = new();

        public DeviceConnection ActiveDevice
        {
            get => _connection;
            set
            {
                if (value.Type == DeviceConnectionType.Disconnected)
                {
                    if (_connection.Type == DeviceConnectionType.Disconnected) return;
                    LogService.LogService.Log("Resetting active device", LogLevel.Debug);
                    _connection.Id = "";
                    _connection.Type = DeviceConnectionType.Disconnected;
                    return;
                }

                if (!DeviceConnections.Contains(value)) throw new DeviceDisconnectedException("Selected device is not connected. ");
                _connection.Id = value.Id;
                _connection.Type = value.Type;
            }
        }

        public bool ActiveDeviceAlive() => SysCalls.Exec(MainModelStatic.PathToPlatformtools, "fastboot.exe", "devices").Result.StdOutput
            .Contains(ActiveDevice.Id);

        public bool DeviceConnected(string id)
        {
            return DeviceConnections.Any(device => device.Id == id);
        }

        public bool IsActiveDeviceConnected() => DeviceConnected(ActiveDevice.Id);

        public void UpdateFastbootDevices()
        {
            LogService.LogService.Log("Updating fastboot devices", LogLevel.Debug);
            var fastbootResult = SysCalls.Exec(MainModelStatic.PathToPlatformtools, "fastboot.exe", "devices");
            foreach (var device in fastbootResult.Result.StdOutput.Split("\r\n"))
            {
                if (device == "") continue;
                var parsedDevice = DeviceConnection.Parse(device);
                LogService.LogService.Log("New device: " + parsedDevice.DeviceToStr, LogLevel.Debug);
                DeviceConnections.Add(parsedDevice);
            }

            if (!DeviceConnected(ActiveDevice.Id))
            {
                ActiveDevice = new DeviceConnection();
            }
        }

        public void UpdateSideloadDevices()
        {
            LogService.LogService.Log("Updating sideload devices", LogLevel.Debug);
            var commandResult = SysCalls.Exec(MainModelStatic.PathToPlatformtools, "adb.exe", "devices");
            foreach (var device in commandResult.Result.StdOutput.Split("\r\n"))
            {
                if (device is "" or "List of devices attached") continue;
                var parsedDevice = DeviceConnection.Parse(device);
                LogService.LogService.Log("New device: " + parsedDevice.DeviceToStr, LogLevel.Debug);
                DeviceConnections.Add(parsedDevice);
            }

            if (!DeviceConnected(ActiveDevice.Id))
            {
                ActiveDevice = new DeviceConnection();
            }
        }

        public void UpdateADBDevices()
        {
            LogService.LogService.Log("Updating adb devices", LogLevel.Debug);
            var commandResult = SysCalls.Exec(MainModelStatic.PathToPlatformtools, "adb.exe", "devices");
            foreach (var device in commandResult.Result.StdOutput.Split("\r\n"))
            {
                if (device is "" or "List of devices attached") continue;
                var parsedDevice = DeviceConnection.Parse(device);
                LogService.LogService.Log("New device: " + parsedDevice.DeviceToStr, LogLevel.Debug);
                DeviceConnections.Add(parsedDevice);
            }

            if (!DeviceConnected(ActiveDevice.Id))
            {
                ActiveDevice = new DeviceConnection();
            }
        }

        public void UpdateDevices()
        {
            DeviceConnections.Clear();
            UpdateFastbootDevices();
            UpdateSideloadDevices();
            if (DeviceConnections.Count == 0 && EmulateDevice && MainModelStatic.IsDebugRelease) {
                DeviceConnections.Add(new DeviceConnection("emulator", DeviceConnectionType.adb));
            }
            if(DeviceConnections.Count == 1) ActiveDevice = DeviceConnections[0];
            if (ActiveDevice.Type == DeviceConnectionType.adb) ActiveDevice.UpdatePartitions();
        }

        public void Disconnect(string id)
        {
            UpdateDevices();
            if (!DeviceConnected(id))
            {
                MainModelStatic.ModelDeviceManager.UpdateDevices(); return;
            }

            foreach (var device in DeviceConnections)
            {
                if (device.Id != id) continue;
                LogService.LogService.Log("Disconnecting " + device.Id + "...", LogLevel.Debug);
                if (device.DeviceToStr == ActiveDevice.DeviceToStr)
                {
                    ActiveDevice = new DeviceConnection();
                }
                DeviceConnections.Remove(device);
                break;
            }
            ActiveDevice = new DeviceConnection();
        }

        public void Select(string id)
        {
            UpdateDevices();
            if (!DeviceConnected(id))
            {
                MainModelStatic.ModelDeviceManager.UpdateDevices(); return;
            }

            foreach (var device in DeviceConnections)
            {
                if (device.Id != id) continue;
                LogService.LogService.Log("Selected: " + device.Id + "...", LogLevel.Info);
                ActiveDevice = device;
                break;
            }
        }

        public static Dictionary<string, string> GetPartitions(string input)
        {
            if(MainModelStatic.ModelDeviceManager.ActiveDevice.Id == "emulator")
            {
                var result = new Dictionary<string, string>
                {
                    { "boot_a", "/dev/block/sdc32" },
                    { "boot_b", "/dev/block/sdc33" },
                    { "test_part", "/dev/block/sdc34" }
                };
                return result;
            }
            else
            {
                var partitions = new Dictionary<string, string>();

                foreach (var part in input.Split('\n'))
                {
                    if (string.IsNullOrEmpty(part) || part.Contains("total")) continue;

                    var split = part.Split(' ');
                    var block = split[^1];
                    var name = split[^3];

                    partitions[name] = block;
                }

                return partitions;
            }
        }

        public void BackupPartitions(Dictionary<string, string> blocks, string SavePath)
        {
            foreach (var block in blocks)
            {
                var shortName = block.Value.Split("/")[^1];
                MainModelStatic.ModelExecuteCode?.Invoke();
                SysCalls.Exec(MainModelStatic.PathToPlatformtools, "adb.exe", $"shell \"dd if={block.Value} of=/sdcard/UDMBackups/{shortName}_{block.Key}.img\"");
                SysCalls.Exec(MainModelStatic.PathToPlatformtools, "adb.exe", $"pull /sdcard/UDMBackups/{shortName}_{block.Key}.img {SavePath}" + @$"\{shortName}_{block.Key}.img");
            }
        }
    }

    public class DeviceConnection(string id = DeviceManager.Disconnected_id, DeviceConnectionType type = DeviceConnectionType.Disconnected) : INotifyPropertyChanged
    {
        public void UpdatePartitions()
        {
            if(Type != DeviceConnectionType.adb && Type != DeviceConnectionType.recovery)
            {
                LogService.LogService.Log("Device is not in adb mode! ", LogLevel.Error);
                return;
            }

            var result = SysCalls.ExecAndRead(MainModelStatic.PathToPlatformtools, "adb.exe", "shell \"cd /dev/block/by-name && ls -l\"");
            if (result == null) { 
                throw new Exception("ADB Shell error: " + result);
            }

            Partitions = DeviceManager.GetPartitions(result);
        }

        public string Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }

        public Dictionary<string, string> Partitions = new();
        public bool IsPartitioned
        {
            get => Partitions.Count > 0;
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
            try
            {
                return new DeviceConnection(unparsed.Split('\t')[0],
                    Enum.Parse<DeviceConnectionType>(unparsed.Split('\t')[1]
                    // in `adb devices`, if device is not connected as sideload, it will display as 'device'
                    .Replace("device", "adb")));
            }
            catch (ArgumentException)
            {
                return new DeviceConnection();
            }
            catch (IndexOutOfRangeException)
            {
                return new DeviceConnection();
            }
        }

        public string DeviceToStr => Type == DeviceConnectionType.Disconnected ? "Disconnected" : (Id + $"\t({Type})");

        public ICommand DisconnectCommand { get; } = new DelegateCommand(CommonCommands.DisconnectDevice, DelegateCommand.DefaultCanExecute);

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
        adb,
        sideload,
        recovery,
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