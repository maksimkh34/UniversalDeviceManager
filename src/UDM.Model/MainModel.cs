using System.IO.Compression;
using System.Net;
using UDM.Model.DIL;
using UDM.Model.LogService;
using UDM.Model.SettingsService;

namespace UDM.Model
{
    public static class MainModel
    {
        public static SettingsStorage SettingsStorage = new(Cwd + SettingsConfFilePath);
        public static SettingChanged? LangChanged;

        public static Dictionary<string, string> Vars = new();

        public static bool IsDebugRelease
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public static string Cwd
        {
            get
            {
#if DEBUG
                return LocalEnvVars.LocalCwd;
#else
                return Directory.GetCurrentDirectory();
#endif
            }
        }

        public static string PathToPlatformtools
        {
            get
            {
                if (IsDebugRelease)
                {
                    return LocalEnvVars.LocalFastbootPath;
                }
                return Directory.GetCurrentDirectory() + @"\fastboot\";
            }
        }

        public static string CurrentScriptCode
        {
            get;
            set;
        } = NoCodeExecutedDefaultMsg;

        public static string? CurrentUserInputFromUserInputWindow;

        public const string SnForceDebugLogs = nameof(SnForceDebugLogs); // Sn - SettingName
        public const string SnLogPath = nameof(SnLogPath);
        public const string SnCurrentLanguage = nameof(SnCurrentLanguage);

        // In GUI, there are shorters (if path more than this value, it will start
        // with "..." and last (MaxPathLength-3) symbols of provided path
        public static int MaxPathLength = 35;

        public const string NoCodeExecutedDefaultMsg = "No code is being executed.";
        public const string FileNotSelected = "File not selected";

        public const string ChangelogPath = @"\changelog";
        public const string InitFilePath = @"\config\init";
        public const string PythonWd = @"\python";
        public const string SettingsConfFilePath = @"\config\settings_storage.conf";
        public const string FirstInstallScriptPath = @"\python\install.py";
        // ReSharper disable once InconsistentNaming
        public const string FirstInstallDILScriptPath = @"\script\install.dil";
        public const string InstallPythonScriptPath = @"\script\py_installer.dil";
        public static readonly string PathToPython = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Programs\Python\Python312-32\python.exe";

        public delegate void MsgDialog(string titleText, string textboxText);
        public delegate void WaitForInputDialog();
        public delegate bool? ExecuteCode();
        public delegate string GetStrAction();
        public delegate string? GetStrActionMsg(string msg);
        public delegate string[] GetFilesDelegate();

        public delegate void MsgWindowAction();

        public static bool ChangelogFound;

        public static MsgDialog? UiMsgDialog;
        public static ExecuteCode? ModelExecuteCode;
        public static ExecuteCode? AutoExecuteCode;
        public static MsgWindowAction? PythonDownloadMsgShow;
        public static MsgWindowAction? PythonDownloadMsgClose;
        public static WaitForInputDialog? UiWaitForInputDialog;
        public static GetStrAction? GetImagePath;
        public static GetStrAction? GetArchivePath;
        public static GetStrAction? GetFolderPath;
        public static GetStrActionMsg? GetUserInput;
        public static GetFilesDelegate? GetFiles;
        public static string? ChangelogTitle;

        public static void RegisterMainModel(MsgDialog msgDialog, ExecuteCode executeCode, ExecuteCode autoExecuteCode,
            GetStrAction getImageStrAction, GetStrAction getArchiveStrAction, GetStrAction getFolderStrAction, GetFilesDelegate getFilesAction,
            string changelogTitle, MsgWindowAction pythonDownloadMsgShow, MsgWindowAction pythonDownloadMsgClose,
            WaitForInputDialog waitForInputDialog, GetStrActionMsg getUserInput)
        {
            UiMsgDialog = msgDialog;
            ChangelogTitle = changelogTitle;
            ModelExecuteCode = executeCode;
            AutoExecuteCode = autoExecuteCode;
            GetImagePath = getImageStrAction;
            GetArchivePath = getArchiveStrAction;
            GetFolderPath = getFolderStrAction;
            GetFiles = getFilesAction;
            PythonDownloadMsgClose = pythonDownloadMsgClose;
            PythonDownloadMsgShow = pythonDownloadMsgShow;
            UiWaitForInputDialog = waitForInputDialog;
            GetUserInput = getUserInput;

            // Do not forget to update SettingsViewModel! 

            SettingsStorage.Register(new Setting(SnForceDebugLogs,
                false, typeof(bool),
                null, null, null,
                "StForceDebugLogs", false, null));

            SettingsStorage.Register(new Setting(SnCurrentLanguage,
                Languages[0], typeof(string),
                null, null, LangChanged,
                "StCurrentLanguage", true, Languages));

            SettingsStorage.Register(new Setting(SnLogPath,
                Cwd + @"\Logs.log", typeof(string),
                ValidateLogPath, null, null,
                "StLogPath", false, null));

            SettingsStorage.LoadSettings();
        }

        public static string[] Languages = { "en-US", "ru-RU" };

        public static DeviceManager ModelDeviceManager = new();

        /// <summary>
        /// Проверяет, первый ли это запуск. Если первый, проводит инициализацию. Если не первый, пропускает действия
        /// </summary>
        public static void CheckStartup()
        {
            LogService.LogService.Log("CheckStartup Running!", LogLevel.Debug);

            Directory.CreateDirectory(Cwd + @"\config");
            // Changelog
            if (File.Exists(Cwd + ChangelogPath))
            {
                ChangelogFound = true;
                UiMsgDialog?.Invoke(ChangelogTitle ?? "Changelog", File.ReadAllText(Cwd + ChangelogPath));
                File.Delete(Cwd + ChangelogPath);
            }

            // python install
            if (File.Exists(Cwd + InitFilePath)) return;

            LogService.LogService.Log("Executing first install...", LogLevel.Debug);

            CurrentScriptCode = File.ReadAllText(Cwd + FirstInstallDILScriptPath);
            AutoExecuteCode?.Invoke();

            File.Create(Cwd + InitFilePath);
        }

        public static bool ValidateLogPath(object value)
        {
            var path = Path.GetDirectoryName((string)value)!;
            if (path == string.Empty) return false;
            Directory.CreateDirectory(path);
            File.Create(value.ToString()!).Close();
            return File.Exists((string)value);
        }

        public static void CheckBlStatus()
        {
            DeviceInteractionLanguage.Execute("fastboot_check_bl");
        }

        public static string ReplaceCodeWars(string code)
        {
            var result = code
                .Replace("%pyexecutable%", PathToPython)
                .Replace("%cwd%", Cwd)
                .Replace("%sid%", ModelDeviceManager.ActiveDevice.Id);

            result = Vars.Keys.Aggregate(result, (current, varName) => current.Replace(varName, Vars[varName]));

            while (result.Contains("askuser"))
            {
                var msg = GetBetween(result, "%askuser: [", "]%");
                var userInput = GetUserInput?.Invoke(msg);
                result = result.Replace($"%askuser: [{msg}]%", userInput);
            }

            while (result.Contains("getblock"))
            {
                var partition = GetBetween(result, "%getblock ", "%");
                var replaced = "";
                ModelDeviceManager.ActiveDevice.UpdatePartitions();
                foreach (var pair in ModelDeviceManager.ActiveDevice.Partitions)
                {
                    if(pair.Key == partition) replaced = pair.Value;
                }
                result = result.Replace($"%getblock {partition}%", replaced);
            }
            return result;
        }

        public static string GetBetween(string strSource, string strStart, string strEnd)
        {
            if (!strSource.Contains(strStart) || !strSource.Contains(strEnd)) return "";
            var start = strSource.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length;
            var end = strSource.IndexOf(strEnd, start, StringComparison.Ordinal);
            return strSource.Substring(start, end - start);

        }

        public static void DownloadFile(string path, string url)
        {
            try
            {
#pragma warning disable SYSLIB0014
                var webClient = new WebClient();
#pragma warning restore SYSLIB0014
                webClient.DownloadFile(new Uri(url), path);
            }
            catch (Exception ex)
            {
                LogService.LogService.Log($"Error downloading from {url}: {ex.Message}", LogLevel.Error);
            }
        }
    }
}