using System.Collections.ObjectModel;
using UDM.Model.MainModelSpace;

namespace UDM.Model.LogService;

public static class LogService
{
    public static ObservableCollection<LogEntry> Logs = [];

    public static void Log(string message, LogLevel level)
    {
        var filtered = Filter(message, level);
        foreach (var entry in filtered) { 
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
        List<LogEntry> list = [];
        while (message.Length < 5)
        {
            message += '\0';
        }
        if (!(MainModelStatic.IsDebugRelease || (bool)(MainModel.SettingsStorage.GetValue(MainModelStatic.SnForceDebugLogs) ?? false)) && level == LogLevel.Debug) return [];
        if (message.Contains("\r\n"))
        {
            list.AddRange(from logMsg in message.Split("\r\n") where logMsg != "" select Filter(logMsg, level)[0]);
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
