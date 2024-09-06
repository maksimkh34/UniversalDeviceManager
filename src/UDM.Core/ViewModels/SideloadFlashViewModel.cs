using System.Windows.Input;
using UDM.Model.Commands;
using UDM.Model.MainModelSpace;

namespace UDM.Core.ViewModels
{
    public class SideloadFlashViewModel(Action closeWindowAction) : BaseViewModel
    {
        public ICommand ApplyCommand { get; set; } = new DelegateCommand(FlashAction, ActiveDeviceConnectedAndZipSelected, closeWindowAction);
        public ICommand BrowseCommand { get; } = new DelegateCommand(BrowseAction, DelegateCommand.DefaultCanExecute);

        private string _selectedArchivePath = MainModelStatic.FileNotSelected;
        public string SelectedArchivePath
        {
            get => _selectedArchivePath;
            set
            {
                _selectedArchivePath = value;
                OnPropertyChanged();
            }
        }

        private static void BrowseAction(object obj)
        {
            var path = MainModelStatic.UiDialogManager?.GetFile("Select archive to sideload", "ZIP Archive (*.zip)|*.zip|All files (*.*)|*.*");
            if (path is not null && path != "")
            {
                Updater?.Invoke(path);
            }
        }


        private static bool ActiveDeviceConnectedAndZipSelected(object obj)
        {
            if (!MainModelStatic.ModelDeviceManager.IsActiveDeviceConnected()) return false;
            if (obj is not string str) return false;
            return str != MainModelStatic.FileNotSelected;
        }

        private static void FlashAction(object obj)
        {
            if (obj is not string archivePath) return;

            MainModelStatic.CurrentScriptCode = $"sideload {archivePath}";
            MainModelStatic.ModelExecuteCode?.Invoke();
        }

        public delegate void PathUpdater(string path);

        public static PathUpdater? Updater;
    }
}
