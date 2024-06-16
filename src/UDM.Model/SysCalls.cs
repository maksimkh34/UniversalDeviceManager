using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace UDM.Model
{
    public static class SysCalls
    {
        public static OsType OsType = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? OsType.Linux : OsType.Win;

        public static string Exec(string path, string filename, string args)
        {
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
