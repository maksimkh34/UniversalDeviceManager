using System.Collections.ObjectModel;

namespace UDM.Model.LogService;

public static class LogService
{
    public static ObservableCollection<LogEntry> Logs = new();

    public static void Log(string message, LogLevel level)
    {
        while (message.Length < 5)
        {
            message += '\0';
        }
        if (!(MainModel.IsDebugRelease || (bool)(MainModel.SettingsStorage.GetValue(MainModel.SnForceDebugLogs) ?? false)) && level == LogLevel.Debug) return;
        if (message.Contains("\r\n"))
        {
            foreach (var logMsg in message.Split("\r\n"))
            {
                if(logMsg != "") Log(logMsg, level);
            }
        }
        else if (!message.StartsWith('\0') && message != "" && message[..5] != "\0\0\0\0\0") Logs.Add(new LogEntry(message.Replace("\0", "").Replace("\t", " ").Replace("\n", "\t").Replace("\r", "\t"), level));
    }

    public static void Save(string filename)
    {
        var output = Logs.Aggregate("", (current, entry) => current + $"[{entry.Date?.ToString("g")}] [{entry.Invoker}] {entry.Level}:\t{entry.Message}\r\n");
        File.WriteAllText(filename, output);
    }
}
