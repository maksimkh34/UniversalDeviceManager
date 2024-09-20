using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UDM.Model.LogService;

namespace UDM.Model
{
    public static class SysCalls
    {
        public static OsType OsType = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? OsType.Linux : OsType.Win;

        private static string? _wd;
        private static string? _filename;
        private static string? _args;

        private static ExecutionResult _result = new(["", ""]);

        public static ExecutionResult ExecLegacy(string wd, string filename, string args)
        {
            _wd = wd;
            _filename = filename;
            _args = args;
            var result = ExecPseudoAsync();
            _wd = null;
            _filename = null;
            _args = null;
            return result;
        }

        public static async Task<ExecutionResult> Exec(string wd, string filename, string args)
        {
            _wd = wd;
            _filename = filename;
            _args = args;
            Task.Run(ExecAsync).Wait();
            _wd = null;
            _filename = null;
            _args = null;
            return _result;
        }

        private static ExecutionResult ExecPseudoAsync()
        {
            if (!File.Exists(_filename)) _filename = _wd + "\\" + _filename;
            LogService.LogService.Log("Executing " + _filename + " " + _args, LogLevel.Debug);

            switch (OsType)
            {
                case OsType.Win:
                    var p = new Process();
                    p.StartInfo.UseShellExecute = false;

                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.RedirectStandardOutput = true;

                    p.StartInfo.CreateNoWindow = true;

                    p.StartInfo.Arguments = _args;
                    p.StartInfo.FileName = _filename;
                    p.StartInfo.WorkingDirectory = _wd;

                    p.Start();
                    p.WaitForExit();

                    var stdOutput = p.StandardOutput.ReadToEnd();
                    var errOutput = p.StandardError.ReadToEnd();

                    return new ExecutionResult([errOutput, stdOutput]);

                case OsType.Linux:
                    throw new NotImplementedException();
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        private static async Task ExecAsync()
        {
            if (!File.Exists(_filename)) _filename = _wd + "\\" + _filename;
            LogService.LogService.Log("Executing " + _filename + " " + _args, LogLevel.Debug);

            switch (OsType)
            {
                case OsType.Win:
                    var p = new Process();
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.Arguments = _args;
                    p.StartInfo.FileName = _filename;
                    p.StartInfo.WorkingDirectory = _wd;

                    p.Start();
                    await p.WaitForExitAsync();

                    var stdOutput = await p.StandardOutput.ReadToEndAsync();
                    var errOutput = await p.StandardError.ReadToEndAsync();

                    _result = new ExecutionResult([errOutput, stdOutput]); 
                    break;

                case OsType.Linux:
                    throw new NotImplementedException();
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        public static string ExecAndRead(string wd, string filename, string args)
        {
            LogService.LogService.Log("Executing " + filename + " " + args, LogLevel.Debug);

            switch (OsType)
            {
                case OsType.Win:
                    var p = new Process();
                    p.StartInfo.UseShellExecute = false;

                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.RedirectStandardOutput = true;

                    p.StartInfo.CreateNoWindow = true;

                    p.StartInfo.Arguments = args;
                    p.StartInfo.FileName = filename;
                    p.StartInfo.WorkingDirectory = wd;

                    p.Start();
                    return p.StandardOutput.ReadToEnd();

                case OsType.Linux:
                    throw new NotImplementedException();
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }

    public enum OsType
    {
        Win,
        Linux
    }

    public class ExecutionResult(IReadOnlyList<string> data)
    {
        public string ErrOutput => data[0];
        public string StdOutput => data[1];
    }
}