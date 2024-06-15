using System.Collections.ObjectModel;
using UDM.Model.LogService;

namespace UDM.Core.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Properties

        public ObservableCollection<LogEntry> Logs { get; } = LogService.Logs;

        #endregion
    }
}
