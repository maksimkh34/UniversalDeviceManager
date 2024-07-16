using System.IO.Compression;
using UDM.Model.DIL;
using UDM.Model.LogService;
using UDM.Model.SettingsService;

namespace UDM.Model
{
    public static class MainModel
    {
        public static SettingsStorage SettingsStorage = new(Cwd + SettingsConfFilePath);
        public static SettingChanged? LangChanged;

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

        public static string PathToFastboot
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

        public const string NoCodeExecutedDefaultMsg = "No code is being executed.";
        public const string ImageNotSelected = "Image not selected";

        public const string ChangelogPath = @"\changelog";
        public const string InitFilePath = @"\config\init";
        public const string PythonWd = @"\python";
        public const string SettingsConfFilePath = @"\config\settings_storage.conf";
        public const string FirstInstallScriptPath = @"\python\install.py";
        public const string PythonEmbedTempPath = @"\python.embed";

        public const string PathToEmbed = @"\py_embed";

        public const string PythonUrl = "https://www.python.org/ftp/python/3.12.4/python-3.12.4-embed-amd64.zip";

        public delegate void MsgDialog(string titleText, string textboxText);
        public delegate void WaitForInputDialog();
        public delegate bool? ExecuteCode();
        public delegate string GetStrAction();
        public delegate string GetStrActionMsg(string msg);

        public delegate void MsgWindowAction();

        public static bool ChangelogFound;

        public static MsgDialog? UiMsgDialog;
        public static ExecuteCode? ModelExecuteCode;
        public static MsgWindowAction? PythonDownloadMsgShow;
        public static MsgWindowAction? PythonDownloadMsgClose;
        public static WaitForInputDialog? UiWaitForInputDialog;
        public static GetStrAction? GetImagePath;
        public static GetStrActionMsg? GetUserInput;
        public static string? ChangelogTitle;

        public static void RegisterMainModel(MsgDialog msgDialog, ExecuteCode executeCode, GetStrAction getImageStrAction, 
            string changelogTitle, MsgWindowAction pythonDownloadMsgShow, MsgWindowAction pythonDownloadMsgClose,
            WaitForInputDialog waitForInputDialog, GetStrActionMsg getUserInput)
        {
            UiMsgDialog = msgDialog;
            ChangelogTitle = changelogTitle;
            ModelExecuteCode = executeCode;
            GetImagePath = getImageStrAction;
            PythonDownloadMsgClose = pythonDownloadMsgClose;
            PythonDownloadMsgShow = pythonDownloadMsgShow;
            UiWaitForInputDialog = waitForInputDialog;
            GetUserInput = getUserInput;

            // Do not forget to update SettingsViewModel! 

            MainModel.SettingsStorage.Register(new Setting(SnForceDebugLogs,
                false, typeof(bool),
                null, null, null,
                "StForceDebugLogs", false, null));

            MainModel.SettingsStorage.Register(new Setting(SnCurrentLanguage,
                Languages[0], typeof(string),
                null, null, MainModel.LangChanged,
                "StCurrentLanguage", true, Languages));

            MainModel.SettingsStorage.Register(new Setting(SnLogPath,
                Cwd + @"\Logs.log", typeof(string),
                ValidateLogPath, null, null,
                "StLogPath", false, null));

            MainModel.
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

            // Changelog
            if (File.Exists(Cwd + ChangelogPath))
            {
                ChangelogFound = true;
                UiMsgDialog?.Invoke(ChangelogTitle ?? "Changelog", File.ReadAllText(Cwd + ChangelogPath));
                File.Delete(Cwd + ChangelogPath);
            }

            // python install
            if (File.Exists(Cwd + InitFilePath)) return;
            if (!Directory.Exists(Cwd + PathToEmbed))
            {
                LogService.LogService.Log("Downloading python embed...", LogLevel.Info);
                PythonDownloadMsgShow?.Invoke();

                Task.Run(async () => await DownloadFile(Cwd + PythonEmbedTempPath, PythonUrl)).Wait();
                ZipFile.ExtractToDirectory(Cwd + PythonEmbedTempPath, Cwd + PathToEmbed);
                File.Delete(Cwd + PythonEmbedTempPath);
                PythonDownloadMsgClose?.Invoke();
                LogService.LogService.Log("Downloaded!", LogLevel.Info);
            }

            LogService.LogService.Log("Execution python install...", LogLevel.Debug);

            InteractionService.InteractionService service = new(Cwd + FirstInstallScriptPath, Cwd + PathToEmbed);
            service.Run();
            LogService.LogService.Log(service.Read(), LogLevel.OuterServices);
            Directory.CreateDirectory(Cwd + @"\config");
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
            var result = code.Replace("%cwd%", Cwd)
                .Replace("%sid%", ModelDeviceManager.SelectedDevice.Id);

            while (result.Contains("askuser"))
            {
                var msg = GetBetween(result, "%askuser: [", "]%");
                var userInput = GetUserInput?.Invoke(msg);
                result = result.Replace($"%askuser: [{msg}]%", userInput);
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

        public static async Task DownloadFile(string path, string url)
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(path, response);

            }
            catch (Exception ex)
            {
                LogService.LogService.Log($"Error downloading from {url}: {ex.Message}", LogLevel.Error);
            }
        }
    }
}