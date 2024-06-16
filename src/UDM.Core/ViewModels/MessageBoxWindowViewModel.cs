namespace UDM.Core.ViewModels
{
    public class MessageBoxWindowViewModel(string message, string textBoxMessage="$unfilled$") : BaseViewModel
    {
        public string Message { get; set; } = message;
        public string TextBoxMessage { get; set; } = textBoxMessage;

        public MessageBoxWindowViewModel() : this("Message is unfilled")
        {

        }
    }
}
