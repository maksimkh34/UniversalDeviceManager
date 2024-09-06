namespace UDM.Model.LogService
{
    public class LogStream
    {
        private bool _sealed;
        private LogEntry? Entry
        {
            get => LogService.Logs[_streamedIndex];
            set
            {
                if (value != null) return;
                _sealed = true;
            }
        }
        private readonly int _streamedIndex;

        public LogStream(LogEntry entry)
        {
            _streamedIndex = LogService.Logs.Count;
            LogService.Logs.Add(entry);
        }

        public string Message => Entry?.Message ?? "";
        public LogLevel Level => Entry?.Level ?? LogLevel.Error;

        public void Update(string? text, LogLevel? level)
        {
            if(_sealed) return;
            if (text != null) LogService.Logs[_streamedIndex] = new LogEntry(text, Level);
            if(level != null) LogService.Logs[_streamedIndex] = new LogEntry(Message, (LogLevel)level);
            LogService.Refresh();
        }

        public void Seal()
        {
            Entry = null!;
        }
    }
}
