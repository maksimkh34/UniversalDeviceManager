using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDM.Model
{
    public delegate string GetLocalized(string key);
    public class TranslationService(GetLocalized getLocalized)
    {
        public string? GetTranslation(string key)
        {
            return getLocalized?.Invoke(key);
        }
    }
}
