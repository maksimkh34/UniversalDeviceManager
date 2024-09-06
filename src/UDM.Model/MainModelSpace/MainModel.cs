using UDM.Model.DIL;
using UDM.Model.LogService;
using UDM.Model.SettingsService;

namespace UDM.Model.MainModelSpace
{
    public static class MainModel
    {
        public static SettingsStorage SettingsStorage = new(MainModelStatic.Cwd + MainModelStatic.SettingsConfFilePath);
        public static SettingChanged? LangChanged;

        public static Dictionary<string, string> Vars = new();

        // In GUI, there are shorters (if path more than this value, it will start
        // with "..." and last (MaxPathLength-3) symbols of provided path

        // ReSharper disable once InconsistentNaming

        public delegate void WaitForInputDialog();
        public delegate bool? ExecuteCode();
        public delegate string GetStrAction();
        public delegate string? GetStrActionMsg(string msg);
        public delegate string[] GetFilesDelegate();

        public delegate void MsgWindowAction();

        public static void ExitMainModel()
        {
            MainModelStatic.InitFileLock?.Close();
            MainModelStatic.ConfigFileLock?.Close();
            SettingsStorage.SaveSettings();
        }

        public static void RegisterMainModel(ExecuteCode preExecuteCodeAction, ExecuteCode autoExecuteCodection, UiDialogManager uiManager,
            string changelogTitle, TranslationService translationService)
        {
            MainModelStatic.ChangelogTitle = changelogTitle;
            MainModelStatic.ModelExecuteCode = preExecuteCodeAction;
            MainModelStatic.AutoExecuteCode = autoExecuteCodection;
            MainModelStatic.UiDialogManager = uiManager;
            MainModelStatic.TranslationService = translationService;

            try
            {
                MainModelStatic.InitFileLock = File.Open(MainModelStatic.Cwd + MainModelStatic.InitFilePath, FileMode.Open);
                MainModelStatic.ConfigFileLock = File.Open(MainModelStatic.Cwd + MainModelStatic.SettingsConfFilePath, FileMode.Open);
            }
            catch (FileNotFoundException) { }

            // Do not forget to update SettingsViewModel! 

            SettingsStorage.Register(new Setting(MainModelStatic.SnForceDebugLogs,
                false, typeof(bool),
                null, null, null,
                "StForceDebugLogs", false, null));

            SettingsStorage.Register(new Setting(MainModelStatic.SnCurrentLanguage,
MainModelStatic.Languages[0], typeof(string),
                null, null, LangChanged,
                "StCurrentLanguage", true, MainModelStatic.Languages));

            SettingsStorage.Register(new Setting(MainModelStatic.SnLogPath, MainModelStatic.Cwd + @"\Logs.log", typeof(string),
ModelCore.ValidateLogPath, null, null,
                "StLogPath", false, null));

            MainModelStatic.
                        ConfigFileLock?.Close();
            SettingsStorage.LoadSettings();
            MainModelStatic.ConfigFileLock = File.Open(MainModelStatic.Cwd + MainModelStatic.SettingsConfFilePath, FileMode.Open);
        }

        /// <summary>
        /// Проверяет, первый ли это запуск. Если первый, проводит инициализацию. Если не первый, пропускает действия
        /// </summary>
        public static void CheckStartup()
        {
            LogService.LogService.Log("CheckStartup Running!", LogLevel.Debug);

            Directory.CreateDirectory(MainModelStatic.Cwd + @"\config");
            // Changelog
            if (File.Exists(MainModelStatic.Cwd + MainModelStatic.ChangelogPath))
            {
                MainModelStatic.ChangelogFound = true;
                MainModelStatic.UiDialogManager?.ShowMsg(MainModelStatic.ChangelogTitle ?? "Changelog", File.ReadAllText(MainModelStatic.Cwd + MainModelStatic.ChangelogPath));
                File.Delete(MainModelStatic.Cwd + MainModelStatic.ChangelogPath);
            }

            // python install
            if (File.Exists(MainModelStatic.Cwd + MainModelStatic.InitFilePath)) return;

            LogService.LogService.Log("Executing first install...", LogLevel.Debug);

            MainModelStatic.CurrentScriptCode = File.ReadAllText(MainModelStatic.Cwd + MainModelStatic.FirstInstallDilScriptPath);
            MainModelStatic.AutoExecuteCode?.Invoke();

            File.Create(MainModelStatic.Cwd + MainModelStatic.InitFilePath);
        }

        public static void CheckBlStatus() => DeviceInteractionLanguage.Execute("fastboot_check_bl");
    }
}