using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UDM.Model.LogService;

namespace UDM.Model
{
    public static class SysCalls
    {
        public static OsType OsType = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? OsType.Linux : OsType.Win;

        public static string Exec(string path, string filename, string args)
        {
            LogService.LogService.Log("Executing " + path + "\\" +filename + " " + args, LogLevel.Debug);

            string output;
            switch (OsType)
            {
                case OsType.Win:
                    var p = new Process();
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.CreateNoWindow = true;

                    p.StartInfo.Arguments = args;
                    p.StartInfo.FileName = filename;
                    p.StartInfo.WorkingDirectory = path;
                    
                    p.Start();
                    output = p.StandardOutput.ReadToEnd();

                    p.WaitForExit();
                    break;

                case OsType.Linux:
                    throw new NotImplementedException();
                default:
                    throw new InvalidEnumArgumentException();
            }
            return output;
        }
    }

    public enum OsType
    {
        Win,
        Linux
    }
}
