using UDM.Model.LogService;

namespace UDM.Model.DIL
{
    public static class DeviceInteractionLanguage
    {
        public static void Execute(string script)
        {
            foreach (var cmd in script.Split("\r\n"))
            {
                var instructions = cmd.Split(' ');
                switch (instructions[0])
                {
                    case "fastboot_reboot":
                        if (instructions.Length == 1)
                        {
                            instructions = new[] { instructions[0], "" };
                        }
                        var rebootCommand = $"-s {MainModel.ModelDeviceManager.SelectedDevice.Id} reboot {instructions[1]}";
                        var rebootOutput = SysCalls.Exec(MainModel.PathToFastboot, "fastboot.exe",
                            rebootCommand);
                        LogService.LogService.Log(rebootOutput == string.Empty ? "OK!" : rebootOutput, LogLevel.Info);
                        break;

                    case "fastboot_check_bl":
                        if (MainModel.ModelDeviceManager.SelectedDevice.Id == DeviceManager.Disconnected_id)
                        {
                            LogService.LogService.Log("Device is not connected!", LogLevel.Error);
                            return;
                        }
                        var checkCommand = $"-s {MainModel.ModelDeviceManager.SelectedDevice.Id} getvar unlocked";
                        var checkOutput = SysCalls.Exec(MainModel.PathToFastboot, "fastboot.exe",
                            checkCommand);
                        LogService.LogService.Log(checkOutput.Split("\r\n")[0], LogLevel.Info);
                        break;

                    case "fastboot_flash":
                        if (MainModel.ModelDeviceManager.SelectedDevice.Id == DeviceManager.Disconnected_id)
                        {
                            LogService.LogService.Log("Device is not connected!", LogLevel.Error);
                            return;
                        }

                        var flashCommand = $"-s {MainModel.ModelDeviceManager.SelectedDevice.Id} flash {instructions[1]} {instructions[2]}";
                        var flashOutput = SysCalls.Exec(MainModel.PathToFastboot, "fastboot.exe",
                            flashCommand);
                        LogService.LogService.Log(flashOutput, LogLevel.Info);

                        break;

                    case "wait_for_bl":
                        while (!MainModel.ModelDeviceManager.SelectedDeviceAlive())
                        {
                            Thread.Sleep(1000);
                        }
                        break;

                    default:
                        LogService.LogService.Log("Unknown command: " + cmd, LogLevel.Error);
                        break;
                }
            }
        }
    }
}
