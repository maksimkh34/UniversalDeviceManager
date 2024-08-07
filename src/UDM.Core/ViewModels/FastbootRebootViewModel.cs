using System.Windows.Input;
using UDM.Model.Commands;

namespace UDM.Core.ViewModels
{
    public class FastbootRebootViewModel(Action closeWindowAction) : BaseViewModel
    {
        public ICommand ApplyCommand { get; } = new DelegateCommand(DeviceCommands.ExecuteCode, DeviceCommands.ActiveDeviceConnected, closeWindowAction);
        public ICommand? CloseCommand { get; set; }
    }
}
