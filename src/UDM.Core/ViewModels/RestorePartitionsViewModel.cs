using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UDM.Model;
using UDM.Model.Commands;

namespace UDM.Core.ViewModels
{
    public class RestorePartitionsViewModel : BaseViewModel
    {
        public ICommand BrowsePartitions { get; } = new DelegateCommand(BrowseAction, DelegateCommand.DefaultCanExecute);
        public ICommand ApplyCommand { get; } = new DelegateCommand(ApplyAction, ApplyCommandPredicate);

        public ObservableCollection<string> PartitionsList { get; } = new();

        public static void BrowseAction(object param)
        {
            if (param is ObservableCollection<string> list)
            {
                string[] files = MainModelStatic.UiDialogManager?.GetFiles("Select partitions to restore", "Image (*.img)|*.img|All files (*.*)|*.*") ?? new string[0];
                foreach(var file in files)
                {
                    if(!list.Contains(file)) list.Add(file);
                }
            }
        }

        public static void ApplyAction(object param)
        {
            var code = "";
            if(param is ObservableCollection<string> list)
            {
                foreach (var file in list)
                {
                    code += $"adb_restore \"{file}\"\r\n";
                }
                MainModel.CurrentScriptCode = code;
                MainModelStatic.ModelExecuteCode?.Invoke();
            }
        }

        public static bool ApplyCommandPredicate(object param)
        {
            return MainModelStatic.ModelDeviceManager.ActiveDevice.Type == DeviceConnectionType.adb;
        }
    }
}
