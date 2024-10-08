﻿using UDM.Model.MainModelSpace;

namespace UDM.Model.Commands
{
    public class Execution
    {
        public static void ExecuteScriptCodeFunction(object? param)
        {
            DIL.DeviceInteractionLanguage.Execute(param?.ToString() ?? "");
        }

        public static bool CanExecuteScriptCode(object? param)
        {
            var code = param?.ToString() ?? MainModelStatic.NoCodeExecutedDefaultMsg;
            return code != MainModelStatic.NoCodeExecutedDefaultMsg;
        }
    }
}
