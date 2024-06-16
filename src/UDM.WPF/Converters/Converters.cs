using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using UDM.Model;
using UDM.Model.LogService;

namespace UDM.WPF.Converters
{
    public class LogEntryCollectionToTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<string> logsStr = new();
            if (values[0] is not ObservableCollection<LogEntry> logEntries) return "Log init error. ";
            foreach (var logEntry in logEntries)
            {
                logsStr.Add(logEntry.GetReadable());
            }

            return logEntries.Count > 0 ? string.Join("\r\n", logsStr) : string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}