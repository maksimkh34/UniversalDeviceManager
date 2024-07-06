namespace UDM.Model.SettingsService
{
    internal static class Writable
    {
        internal static dynamic GetWritableValue(string value)
        {
            return value switch
            {
                "True" => "true",
                "False" => "false",
                _ => value.Replace(";", "&qt$").Replace(" ", "&spc$")
            };
        }

        internal static dynamic GetOriginalFromWritable(string value)
        {
            return value switch
            {
                "true" => true,
                "false" => false,
                _ => value.Replace("&qt$", ";").Replace("&spc$", " ")
            };
        }
    }
}