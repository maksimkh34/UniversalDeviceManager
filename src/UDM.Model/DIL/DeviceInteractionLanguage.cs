using System.Security.Cryptography.X509Certificates;
using UDM.Model.LogService;
// ReSharper disable InconsistentNaming

namespace UDM.Model.DIL
{
    public static class DeviceInteractionLanguage
    {
        public const string FastbootFlash_DisableVerity_Flag = "-dt";
        public const string FastbootFlash_DisableVerification_Flag = "-df";

        public static async void Execute(string script)
        {
            LogService.LogService.Log("Executing script... \n", LogLevel.Info);
            while (script.StartsWith(' ')) script = script.Substring(1);
            var scriptLines = script.Split("\r\n");
            foreach (var pCmd in scriptLines)
            {
                MainModel.ModelDeviceManager.UpdateDevices();

                var cmd = MainModel.ReplaceCodeWars(pCmd);
                var instructions = cmd.Split(' ');
                switch (instructions[0])
                {
                    case "\r":
                    case "\n":
                    case "\r\n":
                    case "":
                        break;

                    case "#":
                        continue;

                    case "select":
                        var deviceId = instructions[1];
                        MainModel.ModelDeviceManager.ActiveDevice = new DeviceConnection(deviceId,
                            MainModel.ModelDeviceManager.ActiveDevice.Type);
                        break;

                    case "flash_rom":
                        var type = instructions[1];
                        var fullFlashPath = instructions[2];
                        for (var i = 3; i < instructions.Length; i++)
                        {
                            fullFlashPath += " " + instructions[i];
                        }

                        var pathToBat = fullFlashPath + "\\";
                        switch (type)
                        {
                            case "-f":
                                pathToBat += "flash_all.bat";
                                break;
                            case "-l":
                                pathToBat += "flash_all_lock.bat";
                                break;
                            case "-k":
                                pathToBat += "flash_all_except_data_storage.bat";
                                break;
                            case "-d":
                                MainModel.UiMsgDialog?.Invoke("Error", "Invalid flashing mode specifed. Flashing in clean type...");
                                pathToBat += "flash_all.bat";
                                break;
                        }

                        var fullFlashOutput = SysCalls.Exec(fullFlashPath, pathToBat,
                            "");

                        if (fullFlashOutput.ErrOutput == string.Empty)
                        {
                            LogService.LogService.Log(fullFlashOutput.StdOutput, LogLevel.Error);
                        }
                        else
                        {
                            LogService.LogService.Log(fullFlashOutput.ErrOutput, LogLevel.DILOutput);
                        }
                        break;

                    case "dil":
                        var scriptPath = instructions[1];
                        for (int i = 2; i < instructions.Length; i++)
                        {
                            scriptPath += " " + instructions[i];
                        }
                        Execute(File.ReadAllText(scriptPath));
                        break;
                    case "if":
                        var expression = instructions[1];
                        for(int i = 2; i < instructions.Length; i++)
                        {
                            expression += " " + instructions[i];
                        }

                        var expressionList = expression.Split('=');
                        if(expressionList.Length != 2)
                        {
                            MainModel.UiMsgDialog?.Invoke("Error", "Invalid expression: " + expression + ". " + expressionList.Length + " equals found (expect 2)");
                        }

                        bool expressionResult = expressionList[0].ToString() == expressionList[1].ToString();

                        var codeIfTrue = MainModel.GetBetween(string.Join("\r\n", scriptLines), "begin", "else");
                        var codeIfFalse = MainModel.GetBetween(string.Join("\r\n", scriptLines), "else", "end");

                        if (expressionResult) Execute(codeIfTrue);
                        else Execute(codeIfFalse);

                        // replace if statement code so DIL will not execute it
                        for(int i = 0; i < scriptLines.Length; i++)
                        {
                            if (scriptLines[i] == "begin")
                            {
                                while (scriptLines[i] != "end")
                                {
                                    scriptLines[i] = "\r\n"; i += 1;
                                }
                                scriptLines[i] = "\r\n"; break;
                            }
                        }

                        break;

                    case "fr":
                    case "fastboot_reboot":
                        if (instructions.Length == 1)
                        {
                            instructions = new[] { instructions[0], "" };
                        }

                        var rebootCommand = instructions.ElementAt(1) == "EDL" ?
                            $"-s {MainModel.ModelDeviceManager.ActiveDevice.Id} oem edl" :
                            $"-s {MainModel.ModelDeviceManager.ActiveDevice.Id} reboot {instructions[1]}";
                        var rebootOutput = SysCalls.Exec(MainModel.PathToPlatformtools, "fastboot.exe",
                            rebootCommand);
                        if (rebootOutput.ErrOutput == string.Empty)
                        {
                            LogService.LogService.Log(rebootOutput.StdOutput, LogLevel.Error);
                        }
                        else
                        {
                            LogService.LogService.Log(rebootOutput.ErrOutput, LogLevel.DILOutput);
                        }
                        break;

                    case "adb_restore":
                        // sdc34_test_part.img
                        var preFile = instructions[1];
                        for(int i = 2; i < instructions.Length; i++)
                        {
                            preFile += instructions[i];
                        }
                        var file = MainModel.GetBetween(preFile, "\"", "\"");
                        var filepParts = Path.GetFileName(file).Replace(".img", "").Split("_");
                        var block = filepParts[0];
                        var partition = filepParts[1];
                        for (int i = 2; i < filepParts.Length; i++)
                        {
                            partition += "_" + filepParts[i];
                        }
                        SysCalls.Exec(MainModel.PathToPlatformtools, "adb.exe", $"shell mkdir /sdcard/UDMBackups/");
                        var pushResult = SysCalls.Exec(MainModel.PathToPlatformtools, "adb.exe", $"push /sdcard/UDMBackups/restore_part {file}");
                        var restoreResult = SysCalls.Exec(MainModel.PathToPlatformtools, "adb.exe", $"shell \"dd if=/sdcard/UDMBackups/restore_part of=/dev/block/{block}");
                        var rmResult = SysCalls.Exec(MainModel.PathToPlatformtools, "adb.exe", $"rm /sdcard/UDMBackups/restore_part");

                        if (pushResult.ErrOutput == string.Empty)
                        {
                            LogService.LogService.Log(pushResult.StdOutput, LogLevel.Error);
                        }
                        else
                        {
                            LogService.LogService.Log(pushResult.ErrOutput, LogLevel.DILOutput);
                        }

                        if (restoreResult.ErrOutput == string.Empty)
                        {
                            LogService.LogService.Log(restoreResult.StdOutput, LogLevel.Error);
                        }
                        else
                        {
                            LogService.LogService.Log(restoreResult.ErrOutput, LogLevel.DILOutput);
                        }

                        if (rmResult.ErrOutput == string.Empty)
                        {
                            LogService.LogService.Log(rmResult.StdOutput, LogLevel.Error);
                        }
                        else
                        {
                            LogService.LogService.Log(rmResult.ErrOutput, LogLevel.DILOutput);
                        }
                        break;

                    case "ar":
                    case "adb_reboot":
                        if (MainModel.ModelDeviceManager.ActiveDevice.Type != DeviceConnectionType.adb)
                        {
                            LogService.LogService.Log("Device is not in adb mode! ", LogLevel.Error);
                            return;
                        }

                        var aRebootCommand = $"-s {MainModel.ModelDeviceManager.ActiveDevice.Id} reboot {instructions[1]}";
                        var aRebootOutput = SysCalls.Exec(MainModel.PathToPlatformtools, "adb.exe",
                            aRebootCommand);
                        if (aRebootOutput.ErrOutput == string.Empty)
                        {
                            LogService.LogService.Log(aRebootOutput.StdOutput, LogLevel.Error);
                        }
                        else
                        {
                            LogService.LogService.Log(aRebootOutput.ErrOutput, LogLevel.DILOutput);
                        }
                        break;

                    case "adb_backup":
                        // adb_backup {pair.Value} {savePath}\\{pair.Value.Split("/")[^1]}_{pair.Key}.img\n
                        var devBlock = instructions[1];
                        var outPath = instructions[2];

                        SysCalls.Exec(MainModel.PathToPlatformtools, "adb.exe", $"shell mkdir /sdcard/UDMBackups/");
                        var shellResult = SysCalls.Exec(MainModel.PathToPlatformtools, "adb.exe", $"shell \"dd if={devBlock} of=/sdcard/UDMBackups/{Path.GetFileName(outPath)}\"");
                        var pullResult = SysCalls.Exec(MainModel.PathToPlatformtools, "adb.exe", $"pull /sdcard/UDMBackups/{Path.GetFileName(outPath)} {outPath}");
                        if (shellResult.ErrOutput == string.Empty)
                        {
                            LogService.LogService.Log(shellResult.StdOutput, LogLevel.Error);
                        }
                        else
                        {
                            LogService.LogService.Log(shellResult.ErrOutput, LogLevel.DILOutput);
                        }

                        if (pullResult.ErrOutput == string.Empty)
                        {
                            LogService.LogService.Log(pullResult.StdOutput, LogLevel.Error);
                        }
                        else
                        {
                            LogService.LogService.Log(pullResult.ErrOutput, LogLevel.DILOutput);
                        }

                        break;

                    case "sl":
                    case "sideload":
                        if (MainModel.ModelDeviceManager.ActiveDevice.Type != DeviceConnectionType.sideload)
                        {
                            LogService.LogService.Log("Device is not in Sideload mode! ", LogLevel.Error);
                            return;
                        }
                        var archive = instructions[1];
                        for (var i = 2; i < instructions.Length; i++)
                        {
                            archive += " " + instructions[i];
                        }
                        var sideloadCommand = $" -s {MainModel.ModelDeviceManager.ActiveDevice.Id} sideload \"{archive}\"";
                        var sideloadOutput = SysCalls.Exec(MainModel.PathToPlatformtools, "adb.exe",
                            sideloadCommand);
                        if (sideloadOutput.ErrOutput == string.Empty)
                        {
                            LogService.LogService.Log(sideloadOutput.StdOutput, LogLevel.Error);
                        }
                        else
                        {
                            LogService.LogService.Log(sideloadOutput.ErrOutput, LogLevel.DILOutput);
                        }
                        break;

                    case "fc":
                    case "fastboot_check_bl":
                        if (MainModel.ModelDeviceManager.ActiveDevice.Id == DeviceManager.Disconnected_id)
                        {
                            LogService.LogService.Log("Device is not connected!", LogLevel.Error);
                            return;
                        }
                        var checkCommand = $"-s {MainModel.ModelDeviceManager.ActiveDevice.Id} getvar unlocked";
                        var checkOutput = SysCalls.Exec(MainModel.PathToPlatformtools, MainModel.PathToPlatformtools + @"\fastboot.exe",
                            checkCommand);
                        if(checkOutput.StdOutput == string.Empty)
                            LogService.LogService.Log(checkOutput.ErrOutput.Split("\r\n")[0], LogLevel.DILOutput);
                        else
                            LogService.LogService.Log(checkOutput.StdOutput, LogLevel.Error);
                        break;

                    case "ff":
                    case "fastboot_flash":
                        if (MainModel.ModelDeviceManager.ActiveDevice.Id == DeviceManager.Disconnected_id)
                        {
                            LogService.LogService.Log("Device is not connected!", LogLevel.Error);
                            return;
                        }

                        var pathStartIndex = 2;

                        var dt = false;
                        var df = false;
                        try {
                            // if there are -dt or -df flags on 3 and 4 place, path to img starts from 4 or 5 instruction
                            if (instructions[2] is FastbootFlash_DisableVerity_Flag or FastbootFlash_DisableVerification_Flag)
                            {
                                pathStartIndex += 1;
                                if (instructions[2] == FastbootFlash_DisableVerity_Flag) dt = true;
                                else df = true;
                            }
                            if (instructions[3] is FastbootFlash_DisableVerity_Flag or FastbootFlash_DisableVerification_Flag)
                            {
                                pathStartIndex += 1;
                                if (instructions[3] == FastbootFlash_DisableVerity_Flag) dt = true;
                                else df = true;
                            }
                        } catch(IndexOutOfRangeException) { }

                        var path = instructions[pathStartIndex];
                        for (var i = pathStartIndex + 1; i < instructions.Length; i++)
                        {
                            path += " " + instructions[i];
                        }
                        var flashCommand = $"-s {MainModel.ModelDeviceManager.ActiveDevice.Id} {(dt ? "--disable-verity " : "")}{(df ? "--disable-verification " : "")}flash {instructions[1]} \"{path}\"";
                        var flashOutput = SysCalls.Exec(MainModel.PathToPlatformtools, "fastboot.exe",
                            flashCommand);
                        if(flashOutput.ErrOutput != "")
                            LogService.LogService.Log(flashOutput.ErrOutput, LogLevel.DILOutput);
                        else LogService.LogService.Log(flashOutput.StdOutput, LogLevel.Error);

                        break;

                    case "wb":
                    case "wait_for_bl":
                        while (!MainModel.ModelDeviceManager.ActiveDeviceAlive())
                        {
                            Thread.Sleep(1000);
                        }
                        break;

                    case "wr":
                    case "wait_for_recovery":
                        while (MainModel.ModelDeviceManager.ActiveDevice.Type != DeviceConnectionType.recovery)
                        {
                            MainModel.ModelDeviceManager.UpdateDevices();
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
                        Directory.CreateDirectory(extractPath);
                        try
                        {
                            var stream = LogService.LogService.OpenStream("Unzipping file... ", LogLevel.Info);
                            await Task.Run(() => { System.IO.Compression.ZipFile.ExtractToDirectory(zipFile, extractPath); } );
                            stream.Update(stream.Message + "Done", null);
                        }
                        catch (IOException)
                        {
                            Directory.Delete(extractPath, true);
                            System.IO.Compression.ZipFile.ExtractToDirectory(zipFile, extractPath);
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
