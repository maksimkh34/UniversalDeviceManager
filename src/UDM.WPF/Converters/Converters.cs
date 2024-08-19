using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Documents;
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

    public class MultiBindingToStrListConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Select(value => value.ToString()).ToList();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PathShorter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string pathValue)
            {
                return pathValue.Length > MainModel.MaxPathLength ? "..." + pathValue[^(MainModel.MaxPathLength-3)..] : pathValue;
            }

            return "value_type_error";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PathToFilename : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string pathValue)
            {
                return Path.GetFileName(pathValue);
            }

            return "";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FlashTypeCustomValueConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool obj)
            {
                return obj ? 4 : 0;
            }

            return 0;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                return index == 4;
            }

            return false;
        }
    }
}