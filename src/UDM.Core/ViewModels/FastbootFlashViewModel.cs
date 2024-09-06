using System.Windows.Input;
using UDM.Model;
using UDM.Model.Commands;
using UDM.Model.MainModelSpace;

namespace UDM.Core.ViewModels;

public class FastbootFlashViewModel(Action closeWindowAction) : BaseViewModel
{
    public ICommand BrowseCommand { get; } = new DelegateCommand(BrowseAction, DelegateCommand.DefaultCanExecute);
    public ICommand ApplyCommand { get; set; } = new DelegateCommand(FlashAction, ActiveDeviceConnectedAndImgSelected, closeWindowAction);

    private bool _isCustomSelected;
    public bool IsCustomSelected
    {
        get => _isCustomSelected;
        set
        {
            if (_isCustomSelected == value) return;
            _isCustomSelected = value;
            OnPropertyChanged();
        }
    }

    private bool _disableVerity;
    public bool DisableVerity
    {
        get => _disableVerity;
        set
        {
            if (_disableVerity == value) return;
            _disableVerity = value;
            OnPropertyChanged();
        }
    }

    private bool _disableVerification;
    public bool DisableVerification
    {
        get => _disableVerification;
        set
        {
            if (_disableVerification == value) return;
            _disableVerification = value;
            OnPropertyChanged();
        }
    }

    private string _selectedImagePath = MainModelStatic.FileNotSelected;
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
        var path = MainModelStatic.UiDialogManager?.GetFile("Select image to flash", "Image (*.img)|*.img|All files (*.*)|*.*");
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
        var disableVerity = enumerable.ElementAt(2) == "True";
        var disableVerification = enumerable.ElementAt(3) == "True";

        MainModelStatic.CurrentScriptCode = $"fastboot_flash {partition} " +
                                      $"{(disableVerity ? Model.DIL.DeviceInteractionLanguage.FastbootFlash_DisableVerity_Flag + " " : "")}" +
                                      $"{(disableVerification ? Model.DIL.DeviceInteractionLanguage.FastbootFlash_DisableVerification_Flag + " " : "")}" +
                                      $"{imgPath}";
        MainModelStatic.ModelExecuteCode?.Invoke();
    }

    public static bool ActiveDeviceConnectedAndImgSelected(object param)
    {
        if (!MainModelStatic.ModelDeviceManager.IsActiveDeviceConnected() || MainModelStatic.ModelDeviceManager.ActiveDevice.Type != DeviceConnectionType.fastboot) return false;
        if (param is not IEnumerable<string> list) return false;
        return list.ElementAt(1) != MainModelStatic.FileNotSelected;
    }

    public delegate void PathUpdater(string path);

    public static PathUpdater? Updater;
}