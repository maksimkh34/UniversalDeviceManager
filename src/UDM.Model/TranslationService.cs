namespace UDM.Model
{
    public delegate string GetLocalized(string key);
    public class TranslationService(GetLocalized? getLocalized)
    {
        public string? Get(string key)
        {
            return getLocalized?.Invoke(key);
        }
    }
}
