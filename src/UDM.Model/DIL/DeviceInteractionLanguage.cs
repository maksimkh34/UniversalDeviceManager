using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDM.Model.LogService;

namespace UDM.Model.DIL
{
    internal static class DeviceInteractionLanguage
    {
        public static void Execute(string script)
        {
            foreach (var cmd in script.Split(";\n"))
            {
                var instructions = cmd.Split(' ');
                switch (instructions[0])
                {
                    case "fastboot_reboot":
                        if (instructions.Length == 1)
                        {
                            instructions = new[] { instructions[0], "" };
                        }
                        var command = $"-s {MainModel.ModelDeviceManager.SelectedDevice.Id} reboot {instructions[1]}";
                        var output = SysCalls.Exec(MainModel.PathToFastboot, "fastboot.exe",
                            command);
                        LogService.LogService.Log(output == string.Empty ? "OK!" : output, LogLevel.Info);
                        break;
                }
            }
        }
    }
}
