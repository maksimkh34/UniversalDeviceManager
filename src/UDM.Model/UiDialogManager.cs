using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDM.Model
{
    public class UiDialogManager
    {
        public delegate string GetStrParams(string param1, string param2);
        public delegate string[] GetStrArr(string param1, string param2);
        public delegate string GetStrParam(string param1);

        public UiDialogManager(GetStrParams GetFile, GetStrArr GetFileArr, GetStrParam GetDirParam, GetStrParam GetUserInput, Action WaitForInput)
        { 
            getFile = GetFile;
            getFileArr = GetFileArr;
            getDirParam = GetDirParam;
            getUserInput = GetUserInput;
            waitForInput = WaitForInput;
        }

        GetStrParams getFile;
        public string? GetFile(string title, string filter)
        {
            return getFile?.Invoke(title, filter);
        }

        GetStrArr getFileArr;
        public string[]? GetFiles(string title, string filter)
        {
            return getFileArr?.Invoke(title, filter);
        }

        GetStrParam getDirParam;
        public string? GetDirectory(string title)
        {
            return getDirParam?.Invoke(title);
        }

        GetStrParam getUserInput;
        public string? GetUserInput(string title)
        {
            return getUserInput?.Invoke(title);
        }

        Action waitForInput;
        public void WaitForInput()
        {
            waitForInput?.Invoke();
        }
    }
}
