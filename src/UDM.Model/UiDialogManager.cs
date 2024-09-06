namespace UDM.Model
{
    public class UiDialogManager(UiDialogManager.GetStrParams? getFile, UiDialogManager.GetStrArr? getFileArr, UiDialogManager.GetStrParam? getDirParam, UiDialogManager.GetStrParam? getUserInput, Action? waitForInput, UiDialogManager.MsgDialog? msgDialog)
    {
        public delegate string GetStrParams(string param1, string param2);
        public delegate string[] GetStrArr(string param1, string param2);
        public delegate void MsgDialog(string titleText, string textboxText);
        public delegate string GetStrParam(string param1);

        public string? GetFile(string title, string filter)
        {
            return getFile?.Invoke(title, filter);
        }

        public string[]? GetFiles(string title, string filter)
        {
            return getFileArr?.Invoke(title, filter);
        }

        public string? GetDirectory(string title)
        {
            return getDirParam?.Invoke(title);
        }

        public string? GetUserInput(string title)
        {
            return getUserInput?.Invoke(title);
        }

        public void ShowMsg(string title, string textBox)
        {
            msgDialog?.Invoke(title, textBox);
        }

        public void WaitForInput()
        {
            waitForInput?.Invoke();
        }
    }
}
