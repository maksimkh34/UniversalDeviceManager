using System.Collections.ObjectModel;

namespace UDM.Model.LogService;

public static class LogService
{
    public static bool IsDebugRelease
    {
        get
        {
#if DEBUG
            return true;
#else
                return false;
#endif
        }
    }

    public static ObservableCollection<LogEntry> Logs = new();

    public static void Log(string message, LogLevel level)
    {
        if (!IsDebugRelease && level == LogLevel.Debug) return;
        Logs.Add(new LogEntry(message, level));
    }

    public static void Save(string filename)
    {
        var output = Logs.Aggregate("", (current, entry) => current + $"[{entry.Date?.ToString("g")}] [{entry.Invoker}] {entry.Level}:\t{entry.Message}\r\n");
        File.WriteAllText(filename, output);
    }
}