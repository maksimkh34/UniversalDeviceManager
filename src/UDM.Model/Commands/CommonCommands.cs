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
        MainModel.CurrentScriptCode = File.ReadAllText(MainModel.Cwd + MainModel.InstallPythonScriptPath);
        MainModelStatic.ModelExecuteCode?.Invoke();
    }
}