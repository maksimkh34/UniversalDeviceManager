using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UDM.Model;
using UDM.Model.Commands;

namespace UDM.Core.ViewModels
{
    public class BackupPartitionsViewModel(Action closeWindow) : BaseViewModel
    {
        public ObservableCollection<string> BeforeSelectPartitions { get; set; } = new(MainModelStatic.ModelDeviceManager.ActiveDevice.Partitions.Keys.ToList());
        public string? SelectedPartitions;

        public static void ApplyFunction(object param)
        {
            if (param is ObservableCollection<string> partitions)
            {
                var scriptCode = "";
                string savePath = MainModelStatic.UiDialogManager?.GetDirectory("Select folder to put backups to") ?? MainModel.Cwd + @"\images";
                if(partitions.Count == 0)
                {
                    MainModelStatic.UiDialogManager?.ShowMsg("Warning", "No partitions selected! ");
                    return;
                }
                Dictionary<string, string> blocks = new();    
                foreach (var partition in partitions) { 
                    foreach(var pair in MainModelStatic.ModelDeviceManager.ActiveDevice.Partitions)
                    {
                        if (pair.Key == partition) scriptCode += $"adb_backup {pair.Value.Replace("\n", "").Replace("\r", "")}" +
                                $" {savePath}\\{pair.Value.Split("/")[^1].Replace("\n", "").Replace("\r", "")}_{pair.Key.Replace("\n", "").Replace("\r", "")}.img\r\n";
                    }
                }

                MainModel.CurrentScriptCode = scriptCode;
                MainModelStatic.ModelExecuteCode?.Invoke();
                MainModelStatic.UiDialogManager?.ShowMsg("Backup", "Backup saved to " + savePath);
            }
            else return;
        }
    }
}
