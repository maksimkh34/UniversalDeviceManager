using UDM.Model.LogService;

namespace UDM.Model.DIL
{
    public static class DeviceInteractionLanguage
    {
        public static void Execute(string script)
        {
            LogService.LogService.Log("Executing script... \n", LogLevel.Info);
            var scriptLines = script.Split("\r\n");
            foreach (var pCmd in scriptLines)
            {
                var cmd = MainModel.ReplaceCodeWars(pCmd);
                var instructions = cmd.Split(' ');
                switch (instructions[0])
                {
                    case "\r":
                    case "\n":
                    case "\r\n":
                    case "":
                        break;

                    case "fr":
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

                    case "fc":
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

                    case "ff":
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

                    case "wb":
                    case "wait_for_bl":
                        while (!MainModel.ModelDeviceManager.SelectedDeviceAlive())
                        {
                            Thread.Sleep(1000);
                        }
                        break;

                    case "iget":
                        var downloadFile = instructions[1];

                        if (cmd.Contains("--overwrite-if-exists") || cmd.Contains("-oie"))
                        {
                            if(File.Exists(downloadFile)) File.Delete(downloadFile);
                        }
                        else if (File.Exists(downloadFile)) break;

                        var downloadPath = Path.GetDirectoryName(downloadFile);
                        if (downloadPath != null) Directory.CreateDirectory(downloadPath);

                        LogService.LogService.Log("Downloading " + downloadFile + "...", LogLevel.DILOutput);
                        MainModel.DownloadFile(downloadFile, instructions[2]);
                        if (File.Exists(downloadFile))
                        {
                            LogService.LogService.Log("Downloaded.", LogLevel.DILOutput);
                        } else {LogService.LogService.Log("File download error.", LogLevel.Error);
                            return;
                        }

                        break;

                    case "msg":
                        var msg = string.Join(" ", instructions[1..]);
                        MainModel.UiMsgDialog?.Invoke("Console", msg);
                        break;

                    case "ww":
                    case "wait_win":
                        MainModel.UiWaitForInputDialog?.Invoke();
                        break;

                    case "py_exec":
                        var scriptPathArgs = instructions[1];
                        for (var i = 2; i < instructions.Length; i++)
                        {
                            scriptPathArgs += " " + instructions[i];
                        }

                        var pCommands = MainModel.GetBetween(string.Join("\r\n", scriptLines), "{", "}").Split("\n");
                        var commands = pCommands.Where(t => t is not ("\r" or "")).ToList();

                        var service = new InteractionService.InteractionService(scriptPathArgs, MainModel.Cwd + MainModel.PythonWd);
                        service.Run();
                        foreach (var line in commands)
                        {
                            var command = MainModel.ReplaceCodeWars(line).Replace("\r", "");
                            for (var i = 0; i < scriptLines.Length; i++)
                            {
                                if (!scriptLines[i].Contains(line.Replace("\r", ""))) continue;

                                scriptLines[i] = scriptLines[i].Replace(line.Replace("\r", ""), command.Replace("\r", ""));
                                break;
                            }
                            if (command.StartsWith("print"))
                            {
                                LogService.LogService.Log(service.Read(), LogLevel.OuterServices);
                            } else if (command.StartsWith("display"))
                            {
                                MainModel.UiMsgDialog?.Invoke("Python console", service.Read());
                            } else if (command.StartsWith("end"))
                            {
                                LogService.LogService.Log("Sending end_wait...", LogLevel.Debug);
                                service.EndWait();
                            }
                            else if (command.StartsWith("write_var_input"))
                            {
                                MainModel.Vars.Add(command.Replace("write_var_input ", ""), service.Read());
                            }
                            else if (command.StartsWith("write_var"))
                            {
                                var args = command.Split(' ');
                                var varValuePy = "";
                                for (var i = 3; i < args.Length; i++)
                                {
                                    varValuePy += " " + args[i];
                                }
                                MainModel.Vars.Add(args[1], varValuePy);
                            }
                            else if (command.StartsWith("write"))
                            {
                                var pyMsg = command.Replace("write ", "").Replace("\r", "");
                                LogService.LogService.Log("Writing " + pyMsg + "...", LogLevel.Debug);
                                service.Write(pyMsg);
                            } 
                            else LogService.LogService.Log("Command was not recognized!", LogLevel.Error);

                        }

                        // replacing script in code with newlines so its code will not be interpreted as new commands
                        for (var i = 0; i < scriptLines.Length; i++)
                        {
                            if (scriptLines[i] != pCmd) continue;
                            scriptLines[i] = "\r\n";
                            scriptLines[i+1] = "\r\n";

                            for (var j = i + 2; j < i + 3 + commands.Count; j++)
                            {
                                scriptLines[j] = "\r\n";
                            }

                            break;
                        }

                        break;

                    case "wv":
                    case "write_var":
                        var varValue = instructions[2];
                        for (var i = 3; i < instructions.Length; i++)
                        {
                            varValue += " " + instructions[i];
                        }
                        MainModel.Vars.Add(instructions[1], varValue);
                        break;

                    case "log":
                        LogService.LogService.Log(cmd.Replace("log ", ""), LogLevel.Info);
                        break;

                    case "cmdexec":
                        var wd = instructions[1];
                        var execCmd = string.Join(" ", instructions[2..]);
                        var split = execCmd.Split(' ');
                        SysCalls.Exec(wd, split[0], string.Join(" ", split[1..]));
                        break;

                    case "rem":
                        File.Delete(cmd.Replace("rem ", ""));
                        break;

                    case "unzip":
                        var zipFile = instructions[1];
                        var extractPath = string.Join(" ", instructions[2..]);
                        System.IO.Compression.ZipFile.ExtractToDirectory(zipFile, extractPath);
                        break;
                    default:
                        LogService.LogService.Log("Unknown command: " + cmd, LogLevel.Error);
                        return;
                }
            }
        }
    }
}
