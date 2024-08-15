using System.Windows.Input;
using UDM.Model;
using UDM.Model.Commands;

namespace UDM.Core.ViewModels
{
    public class SideloadFlashViewModel(Action closeWindowAction) : BaseViewModel
    {
        public ICommand ApplyCommand { get; set; } = new DelegateCommand(FlashAction, ActiveDeviceConnectedAndZipSelected, closeWindowAction);
        public ICommand BrowseCommand { get; } = new DelegateCommand(BrowseAction, DelegateCommand.DefaultCanExecute);

        private string _selectedArchivePath = MainModel.FileNotSelected;
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
            var path = MainModel.GetArchivePath?.Invoke();
            if (path is not null && path != "")
            {
                Updater?.Invoke(path);
            }
        }


        private static bool ActiveDeviceConnectedAndZipSelected(object obj)
        {
            if (!MainModel.ModelDeviceManager.IsActiveDeviceConnected()) return false;
            if (obj is not string str) return false;
            return str != MainModel.FileNotSelected;
        }

        private static void FlashAction(object obj)
        {
            if (obj is not string archivePath) return;

            MainModel.CurrentScriptCode = $"sideload {archivePath}";
            MainModel.ModelExecuteCode?.Invoke();
        }

        public delegate void PathUpdater(string path);

        public static PathUpdater? Updater;
    }
}
