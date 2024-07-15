using UDM.Model.LogService;

namespace UDM.Model.DIL
{
    public static class DeviceInteractionLanguage
    {
        public static void Execute(string script)
        {
            LogService.LogService.Log("Executing script... \n", LogLevel.Info);
            foreach (var pCmd in script.Split("\r\n"))
            {
                var cmd = MainModel.ReplaceCodeWars(pCmd);
                var instructions = cmd.Split(' ');
                switch (instructions[0])
                {
                    case "fastboot_reboot":
                        if (instructions.Length == 1)
                        {
                            instructions = new[] { instructions[0], "" };
                        }

                        var rebootCommand = instructions.ElementAt(1) == "EDL" ?
                            $"-s {MainModel.ModelDeviceManager.SelectedDevice.Id} oem edl" :
                            $"-s {MainModel.ModelDeviceManager.SelectedDevice.Id} reboot {instructions[1]}";
                        var rebootOutput = SysCalls.Exec(MainModel.PathToFastboot, "fastboot.exe",
                            rebootCommand);
                        if (rebootOutput.StdOutput == string.Empty)
                        {
                            LogService.LogService.Log(rebootOutput.ErrOutput, LogLevel.Error);
                        }
                        else
                        {
                            LogService.LogService.Log(rebootOutput.StdOutput, LogLevel.DILOutput);
                        }
                        break;

                    case "fastboot_check_bl":
                        if (MainModel.ModelDeviceManager.SelectedDevice.Id == DeviceManager.Disconnected_id)
                        {
                            LogService.LogService.Log("Device is not connected!", LogLevel.Error);
                            return;
                        }
                        var checkCommand = $"-s {MainModel.ModelDeviceManager.SelectedDevice.Id} getvar unlocked";
                        var checkOutput = SysCalls.Exec(MainModel.PathToFastboot, MainModel.PathToFastboot + @"\fastboot.exe",
                            checkCommand);
                        if(checkOutput.ErrOutput == string.Empty)
                            LogService.LogService.Log(checkOutput.StdOutput.Split("\r\n")[0], LogLevel.DILOutput);
                        else
                            LogService.LogService.Log(checkOutput.ErrOutput, LogLevel.Error);
                        break;

                    case "fastboot_flash":
                        if (MainModel.ModelDeviceManager.SelectedDevice.Id == DeviceManager.Disconnected_id)
                        {
                            LogService.LogService.Log("Device is not connected!", LogLevel.Error);
                            return;
                        }

                        var path = instructions[2];
                        for (var i = 3; i < instructions.Length; i++)
                        {
                            path += " " + instructions[i];
                        }
                        var flashCommand = $"-s {MainModel.ModelDeviceManager.SelectedDevice.Id} flash {instructions[1]} \"{path}\"";
                        var flashOutput = SysCalls.Exec(MainModel.PathToFastboot, "fastboot.exe",
                            flashCommand);
                        if(flashOutput.StdOutput != "")
                            LogService.LogService.Log(flashOutput.StdOutput, LogLevel.DILOutput);
                        else LogService.LogService.Log(flashOutput.ErrOutput, LogLevel.Error);

                        break;

                    case "wait_for_bl":
                        while (!MainModel.ModelDeviceManager.SelectedDeviceAlive())
                        {
                            Thread.Sleep(1000);
                        }
                        break;

                    case "eget":
                        var downloadFile = instructions[1];
                        for (var i = 3; i < instructions.Length; i++)
                        {
                            downloadFile += " " + instructions[i];
                        }

                        var downloadPath = Path.GetDirectoryName(downloadFile);
                        Directory.CreateDirectory(downloadPath);

                        LogService.LogService.Log("Downloading " + downloadFile + "...", LogLevel.DILOutput);
                        Task.Run(async () => await MainModel.DownloadFile(downloadFile, instructions[2])).Wait();
                        if (File.Exists(downloadFile))
                        {
                            LogService.LogService.Log("Downloaded.", LogLevel.DILOutput);
                        } else LogService.LogService.Log("File download error.", LogLevel.Error);

                        break;

                    case "msg":
                        var msg = instructions[1];
                        for (var i = 2; i < instructions.Length; i++)
                        {
                            msg += " " + instructions[i];
                        }
                        MainModel.UiMsgDialog?.Invoke("Console", msg);
                        break;

                    case "wait_win":
                        MainModel.UiWaitForInputDialog?.Invoke();
                        break;

                    case "py_exec":
                        var scriptPathArgs = instructions[1];
                        for (var i = 2; i < instructions.Length; i++)
                        {
                            scriptPathArgs += " " + instructions[i];
                        }

                        break;

                    default:
                        LogService.LogService.Log("Unknown command: " + cmd, LogLevel.Error);
                        return;
                }
            }
        }
    }
}
