using System.Collections.ObjectModel;
using System.Windows.Input;
using UDM.Model;
using UDM.Model.Commands;
using UDM.Model.MainModelSpace;

namespace UDM.Core.ViewModels
{
    public class RestorePartitionsViewModel : BaseViewModel
    {
        public ICommand BrowsePartitions { get; } = new DelegateCommand(BrowseAction, DelegateCommand.DefaultCanExecute);
        public ICommand ApplyCommand { get; } = new DelegateCommand(ApplyAction, ApplyCommandPredicate);

        public ObservableCollection<string> PartitionsList { get; } = [];

        public static void BrowseAction(object param)
        {
            if (param is not ObservableCollection<string> list) return;
            var files = MainModelStatic.UiDialogManager?.GetFiles("Select partitions to restore", "Image (*.img)|*.img|All files (*.*)|*.*") ?? [];
            foreach(var file in files)
            {
                if(!list.Contains(file)) list.Add(file);
            }
        }

        public static void ApplyAction(object param)
        {
            if (param is not ObservableCollection<string> list) return;
            var code = list.Aggregate("", (current, file) => current + $"adb_restore \"{file}\"\r\n");
            MainModelStatic.CurrentScriptCode = code;
            MainModelStatic.ModelExecuteCode?.Invoke();
        }

        public static bool ApplyCommandPredicate(object param)
        {
            return MainModelStatic.ModelDeviceManager.ActiveDevice.Type == DeviceConnectionType.adb;
        }
    }
}
