using System.Windows.Input;
using UDM.Model.Commands;
using UDM.Model.MainModelSpace;

// ReSharper disable InconsistentNaming

namespace UDM.Core.ViewModels
{
    public class PreDILViewModel(Action closeWindowAction) : BaseViewModel
    {
        public PreDILViewModel() : this(() => { }) { }

        public string ScriptCode
        {
            get => MainModelStatic.CurrentScriptCode;
            set
            {
                MainModelStatic.CurrentScriptCode = value;
                OnPropertyChanged();
            }
        }

        public void ExecNow()
        {
            ExecuteDILScript.Execute(ScriptCode);
        }

        public ICommand ExecuteDILScript { get; } = new DelegateCommand(Execution.ExecuteScriptCodeFunction, Execution.CanExecuteScriptCode, closeWindowAction);
    }
}
