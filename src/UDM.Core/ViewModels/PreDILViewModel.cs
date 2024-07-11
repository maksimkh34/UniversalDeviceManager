using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UDM.Model;
using UDM.Model.Commands;

namespace UDM.Core.ViewModels
{
    public class PreDILViewModel
    {
        public string ScriptCode
        {
            get => MainModel.CurrentScriptCode;
            set => MainModel.CurrentScriptCode = value;
        }

        public ICommand ExecuteDILScript { get; }= new DelegateCommand(Execution.ExecuteScriptCodeFunction, Execution.CanExecuteScriptCode);
    }
}
