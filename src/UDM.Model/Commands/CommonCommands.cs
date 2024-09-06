using UDM.Model.MainModelSpace;

namespace UDM.Model.Commands;

public static class CommonCommands
{
    public static void DisconnectDevice(object param)
    {
        if (param is string id)
        {
            MainModelStatic.ModelDeviceManager.Disconnect(id);
        }
    }

    public static void InstallPython(object param)
    {
        MainModelStatic.CurrentScriptCode = File.ReadAllText(MainModelStatic.Cwd + MainModelStatic.InstallPythonScriptPath);
        MainModelStatic.ModelExecuteCode?.Invoke();
    }
}