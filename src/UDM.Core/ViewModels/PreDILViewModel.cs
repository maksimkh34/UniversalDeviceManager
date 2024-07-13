using System.Windows.Input;
using UDM.Model;
using UDM.Model.Commands;
// ReSharper disable InconsistentNaming

namespace UDM.Core.ViewModels
{
    public class PreDILViewModel(Action closeWindowAction) : BaseViewModel
    {
        public PreDILViewModel() : this(() => { }) { }

        public string ScriptCode
        {
            get => MainModel.CurrentScriptCode;
            set
            {
                MainModel.CurrentScriptCode = value;
                OnPropertyChanged();
            }
        }

        public ICommand ExecuteDILScript { get; } = new DelegateCommand(Execution.ExecuteScriptCodeFunction, Execution.CanExecuteScriptCode, closeWindowAction);
    }
}
