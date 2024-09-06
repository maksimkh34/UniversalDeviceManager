namespace UDM.Model.MainModelSpace
{
    public static class MainModelStatic
    {
        public static string? CurrentUserInputFromUserInputWindow;
        public static string? ChangelogTitle;

        public static bool ChangelogFound;

        public static MainModel.ExecuteCode? ModelExecuteCode;

        public static MainModel.ExecuteCode? AutoExecuteCode;

        public static UiDialogManager? UiDialogManager;

        public static TranslationService? TranslationService;

        public static FileStream? ConfigFileLock;

        public static FileStream? InitFileLock;

        public static string[] Languages = ["en-US", "ru-RU"];

        public static DeviceManager ModelDeviceManager = new();

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
                    return LocalEnvVars.LocalPlatformtoolsPath;
                }
                return Directory.GetCurrentDirectory() + @"\fastboot\";
            }
        }

        public static int MaxPathLength { get; set; } = 35;
        public const string SnForceDebugLogs = nameof(SnForceDebugLogs); // Sn - SettingName
        public const string SnLogPath = nameof(SnLogPath);
        public const string SnCurrentLanguage = nameof(SnCurrentLanguage);
        public const string NoCodeExecutedDefaultMsg = "No code is being executed.";
        public const string FileNotSelected = "File not selected";
        public const string ChangelogPath = @"\changelog";
        public const string InitFilePath = @"\config\init";
        public const string PythonWd = @"\python";
        public const string SettingsConfFilePath = @"\config\settings_storage.conf";
        public const string FirstInstallScriptPath = @"\python\install.py";
        public const string FirstInstallDilScriptPath = @"\script\install.dil";
        public const string InstallPythonScriptPath = @"\script\py_installer.dil";
        public static readonly string PathToPython = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Programs\Python\Python312-32\python.exe";

        public static string CurrentScriptCode
        {
            get;
            set;
        } = NoCodeExecutedDefaultMsg;
    }
}