using System.Collections.ObjectModel;
using UDM.Model.Log;

namespace UDM.Core.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Properties

        public ObservableCollection<LogEntry> Logs { get; } = LogService.Logs;

        #endregion
    }
}
