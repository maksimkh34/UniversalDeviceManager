using System.Linq;
using System.Windows.Input;
using UDM.Model;
using UDM.Model.Commands;

namespace UDM.Core.ViewModels;

public class FastbootFlashDialogViewModel(Action closeWindowAction) : BaseViewModel
{
    public ICommand BrowseCommand { get; } = new DelegateCommand(BrowseAction, DelegateCommand.DefaultCanExecute);
    public ICommand ApplyCommand { get; set; } = new DelegateCommand(FlashAction, ActiveDeviceConnectedAndImgSelected, closeWindowAction);

    private string _selectedImagePath = MainModel.ImageNotSelected;
    public string SelectedImagePath
    {
        get => _selectedImagePath;
        set
        {
            _selectedImagePath = value;
            OnPropertyChanged();
        }
    }

    public static void BrowseAction(object param)
    {
        var path = MainModel.GetImagePath?.Invoke();
        if (path is not null && path != "")
        {
            Updater?.Invoke(path);
        }
    }

    public static void FlashAction(object param)
    {
        if (param is not IEnumerable<string> list) return;

        IEnumerable<string> enumerable = list as string[] ?? list.ToArray();
        var partition = enumerable.ElementAt(0);
        var imgPath = enumerable.ElementAt(1);

        MainModel.CurrentScriptCode = $"fastboot_flash {partition} {imgPath}";
        MainModel.ModelExecuteCode?.Invoke();
        // fastboot_flash recovery recovery.img
    }

    public static bool ActiveDeviceConnectedAndImgSelected(object param)
    {
        if (!MainModel.ModelDeviceManager.ActiveDeviceConnected()) return false;
        if (param is not IEnumerable<string> list) return false;
        return list.ElementAt(1) != MainModel.ImageNotSelected;
    }

    public delegate void PathUpdater(string path);

    public static PathUpdater? Updater;
}