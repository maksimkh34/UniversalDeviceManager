﻿using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UDM.Model.LogService;

namespace UDM.Model
{
    public static class SysCalls
    {
        public static OsType OsType = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? OsType.Linux : OsType.Win;

        public static ExecutionResult Exec(string wd, string filename, string args)
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
                    p.WaitForExit();

                    var stdOutput = p.StandardError.ReadToEnd();
                    var errOutput = p.StandardOutput.ReadToEnd();

                    return new ExecutionResult(new[] { stdOutput, errOutput });

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
        public string StdOutput => data[0];
        public string ErrOutput => data[1];
    }
}