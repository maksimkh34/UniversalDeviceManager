using System.Collections.ObjectModel;
using UDM.Model.MainModelSpace;

namespace UDM.Core.ViewModels
{
    public class BackupPartitionsViewModel : BaseViewModel
    {
        public ObservableCollection<string> BeforeSelectPartitions { get; set; } = new(MainModelStatic.ModelDeviceManager.ActiveDevice.Partitions.Keys.ToList());
        public string? SelectedPartitions;
        public static Action CloseWindow;

        public static void ApplyFunction(object param)
        {
            if (param is not ObservableCollection<string> partitions) return;
            var savePath = MainModelStatic.UiDialogManager?.GetDirectory("Select folder to put backups to") ?? MainModelStatic.Cwd + @"\images";
            if(partitions.Count == 0)
            {
                MainModelStatic.UiDialogManager?.ShowMsg("Warning", "No partitions selected! ");
                return;
            }
            var scriptCode = (from partition in partitions 
                    from pair in MainModelStatic.ModelDeviceManager.ActiveDevice.Partitions 
                    where pair.Key == partition select pair)
                .Aggregate("", (current, pair) => current + 
                                                  $"adb_backup {pair.Value.Replace("\n", "")
                                                      .Replace("\r", "")}" + $" {savePath}\\{pair.Value.Split("/")[^1]
                                                          .Replace("\n", "").Replace("\r", "")}_{pair.Key.Replace("\n", "")
                                                          .Replace("\r", "")}.img\r\n");

            MainModelStatic.CurrentScriptCode = scriptCode;
            MainModelStatic.ModelExecuteCode?.Invoke();
            MainModelStatic.UiDialogManager?.ShowMsg("Backup", "Backup saved to " + savePath);
            CloseWindow.Invoke();
        }
    }
}
