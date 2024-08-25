using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDM.Model
{
    public class UiDialogManager(UiDialogManager.GetStrParams GetFile, UiDialogManager.GetStrArr GetFileArr, UiDialogManager.GetStrParam GetDirParam, UiDialogManager.GetStrParam GetUserInput, Action WaitForInput, UiDialogManager.MsgDialog MsgDialog)
    {
        public delegate string GetStrParams(string param1, string param2);
        public delegate string[] GetStrArr(string param1, string param2);
        public delegate void MsgDialog(string titleText, string textboxText);
        public delegate string GetStrParam(string param1);

        private readonly GetStrParams getFile = GetFile;
        public string? GetFile(string title, string filter)
        {
            return getFile?.Invoke(title, filter);
        }

        private readonly GetStrArr getFileArr = GetFileArr;
        public string[]? GetFiles(string title, string filter)
        {
            return getFileArr?.Invoke(title, filter);
        }

        private readonly GetStrParam getDirParam = GetDirParam;
        public string? GetDirectory(string title)
        {
            return getDirParam?.Invoke(title);
        }

        private readonly GetStrParam getUserInput = GetUserInput;
        public string? GetUserInput(string title)
        {
            return getUserInput?.Invoke(title);
        }

        private readonly MsgDialog msgDialog = MsgDialog;
        public void ShowMsg(string title, string textBox)
        {
            msgDialog?.Invoke(title, textBox);
        }

        private readonly Action waitForInput = WaitForInput;
        public void WaitForInput()
        {
            waitForInput?.Invoke();
        }
    }
}
