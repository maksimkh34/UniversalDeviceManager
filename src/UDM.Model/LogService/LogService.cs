using System.Collections.ObjectModel;

namespace UDM.Model.LogService;

public static class LogService
{
    public static ObservableCollection<LogEntry> Logs = new();

    public static void Log(string message, LogLevel level)
    {
        var filtered = Filter(message, level);
        foreach (LogEntry entry in filtered) { 
            Logs.Add(entry);
        }
    }

    public static void Refresh()
    {
        var tempEntry = new LogEntry("", LogLevel.Debug);
        Logs.Add(tempEntry);
        Logs.Remove(tempEntry);
    }

    public static LogEntry[] Filter(string message, LogLevel level)
    {
        List<LogEntry> list = new();
        while (message.Length < 5)
        {
            message += '\0';
        }
        if (!(MainModel.IsDebugRelease || (bool)(MainModel.SettingsStorage.GetValue(MainModel.SnForceDebugLogs) ?? false)) && level == LogLevel.Debug) return new LogEntry[0];
        if (message.Contains("\r\n"))
        {
            foreach (var logMsg in message.Split("\r\n"))
            {
                if (logMsg != "") list.Add(Filter(logMsg, level)[0]);
            }
        }
        else if (!message.StartsWith('\0') && message != "" && message[..5] != "\0\0\0\0\0") list.Add(new LogEntry(message.Replace("\0", "").Replace("\t", " ").Replace("\n", "\t").Replace("\r", "\t"), level));
        return list.ToArray();
    }

    public static void Save(string filename)
    {
        var output = Logs.Aggregate("", (current, entry) => current + $"[{entry.Date?.ToString("g")}] [{entry.Invoker}] {entry.Level}:\t{entry.Message}\r\n");
        File.WriteAllText(filename, output);
    }
    /// <summary>
    /// This will filter only one log line! Do not use with multiple logs in one and check if its empty before use!
    /// </summary>
    /// <param name="message"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static LogStream OpenStream(string message, LogLevel level)
    {
        return new LogStream(Filter(message, level)[0]);
    }
}
