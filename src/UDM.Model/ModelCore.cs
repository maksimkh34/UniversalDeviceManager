using System.Net;
using UDM.Model.LogService;

namespace UDM.Model
{
    internal static class ModelCore
    {

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

        // move
        public static string GetBetween(string strSource, string strStart, string strEnd)
        {
            if (!strSource.Contains(strStart) || !strSource.Contains(strEnd)) return "";
            var start = strSource.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length;
            var end = strSource.IndexOf(strEnd, start, StringComparison.Ordinal);
            return strSource[start..end];

        }

        public static string ReplaceCodeWars(string code)
        {
            var result = code
                .Replace("%pyexecutable%", MainModel.PathToPython)
                .Replace("%cwd%", MainModel.Cwd)
                .Replace("%sid%", MainModelStatic.ModelDeviceManager.ActiveDevice.Id);

            result = MainModel.Vars.Keys.Aggregate(result, (current, varName) => current.Replace(varName, MainModel.Vars[varName]));

            while (result.Contains("askuser")) 
            {
                var msg = GetBetween(result, "%askuser: [", "]%");
                var userInput = MainModelStatic.UiDialogManager?.GetUserInput(msg);
                result = result.Replace($"%askuser: [{msg}]%", userInput);
            }

            while (result.Contains("getblock"))
            {
                var partition = GetBetween(result, "%getblock ", "%");
                var replaced = "";
                MainModelStatic.ModelDeviceManager.ActiveDevice.UpdatePartitions();
                foreach (var pair in MainModelStatic.ModelDeviceManager.ActiveDevice.Partitions)
                {
                    if (pair.Key == partition) replaced = pair.Value;
                }
                result = result.Replace($"%getblock {partition}%", replaced);
            }
            return result;
        }

        public static bool ValidateLogPath(object value)
        {
            var path = Path.GetDirectoryName((string)value)!;
            if (path == string.Empty) return false;
            Directory.CreateDirectory(path);
            File.Create(value.ToString()!).Close();
            return File.Exists((string)value);
        }
    }
}