using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using UDM.Model.Log;

namespace UDM.WPF.Converters
{
    #region Converters

    public class LogEntriesToStringConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<LogEntry> logEntries)
            {
                return string.Join("\n", logEntries.Select(entry => entry.Message));
            }
            return string.Empty;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
