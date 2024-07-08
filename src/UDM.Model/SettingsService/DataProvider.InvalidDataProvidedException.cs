namespace UDM.Model.SettingsService
{
    public class InvalidDataProvidedException : Exception
    {
        public InvalidDataProvidedException()
        {
        }

        public InvalidDataProvidedException(string message)
            : base(message)
        {
        }

        public InvalidDataProvidedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
