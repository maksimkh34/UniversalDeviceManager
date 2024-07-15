namespace UDM.Core.ViewModels
{
    public class UserInputWindowViewModel(string msg) : BaseViewModel
    {
        public string InMsg { get; set; } = msg;
        public string? OutMsg { get; set; }
    }
}