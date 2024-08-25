using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDM.Model.LogService
{
    public class LogStream
    {
        private bool _sealed = false;
        private LogEntry _entry
        {
            get => LogService.Logs[streamedIndex];
            set
            {
                if (value != null) return;
                _sealed = true;
            }
        }
        private int streamedIndex;

        public LogStream(LogEntry entry)
        {
            streamedIndex = LogService.Logs.Count;
            LogService.Logs.Add(entry);
        }

        public string Message => _entry.Message;
        public LogLevel Level => _entry.Level;

        public void Update(string? text, LogLevel? level)
        {
            if(_sealed) return;
            if (text != null) LogService.Logs[streamedIndex] = new LogEntry(text, Level);
            if(level != null) LogService.Logs[streamedIndex] = new LogEntry(Message, (LogLevel)level);
            LogService.Refresh();
        }

        public void Seal()
        {
            _entry = null!;
        }
    }
}
