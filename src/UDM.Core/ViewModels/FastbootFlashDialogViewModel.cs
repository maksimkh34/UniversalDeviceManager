using System.Windows.Input;
using UDM.Model;
using UDM.Model.Commands;

namespace UDM.Core.ViewModels;

public class FastbootFlashDialogViewModel(Action closeWindowAction) : BaseViewModel
{
    public ICommand BrowseCommand { get; } = new DelegateCommand(BrowseAction, DelegateCommand.DefaultCanExecute);
    public ICommand ApplyCommand { get; set; } = new DelegateCommand(FlashAction, DeviceCommands.ActiveDeviceConnected, closeWindowAction);

    private string _selectedImagePath = "Image not selected";
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
        if (path is not null)
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

        Model.DIL.DeviceInteractionLanguage.Execute($"fastboot_flash {partition} {imgPath}");
        // fastboot_flash recovery recovery.img
    }

    public delegate void PathUpdater(string path);

    public static PathUpdater? Updater;
}