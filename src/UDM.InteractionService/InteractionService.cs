﻿using System.Diagnostics;

namespace UDM.InteractionService
{
    public class InteractionService
    {
        public static string Cwd
        {
            get
            {
#if DEBUG
                return @"D:\Work\C#\UniversalDeviceManager\cwd";
#else
                return Directory.GetCurrentDirectory();
#endif
            }
        }

        private readonly Process _proc = new();
        private bool _running;

        public InteractionService(string pathToScript, string cwd)
        {
            _proc.StartInfo.WorkingDirectory = cwd;
            _proc.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Programs\Python\Python312-32\python\python.exe";
            _proc.StartInfo.Arguments = pathToScript;
            _proc.StartInfo.RedirectStandardInput = true;
            _proc.StartInfo.RedirectStandardOutput = true;
            _proc.StartInfo.RedirectStandardError = true;
            _proc.StartInfo.UseShellExecute = false;
            _proc.StartInfo.CreateNoWindow = true;
        }

        public void Run()
        {
            _proc.Start();
            _running = true;
        }

        public string Read()
        {
            if (!_running) throw new Exception("Interaction session is not running");
            var buffer = new char[1000];
            _proc.StandardOutput.Read(buffer, 0, buffer.Length);
            if (buffer[0] == buffer[1] &&
                buffer[1] == buffer[2] &&
                buffer[2] == buffer[3] &&
                buffer[3] == buffer[4] &&
                buffer[4] == buffer[5] &&
                buffer[5] == '\0')
            {
                _proc.StandardError.Read(buffer, 0, buffer.Length);
            }

            return string.Join("", buffer);
        }

        // ReSharper disable once InconsistentNaming
        public string GetDILCode()
        {
            var code = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\UniversalDeviceManager\python_pipe.dil");
            File.Delete("python_pipe.dil");
            return code;
        }

        public string ReadErr()
        {
            if (!_running) throw new Exception("Interaction session is not running");
            var buffer = new char[1000];
            _proc.StandardError.Read(buffer, 0, buffer.Length);
            if (buffer[0] == buffer[1] &&
                buffer[1] == buffer[2] &&
                buffer[2] == buffer[3] &&
                buffer[3] == buffer[4] &&
                buffer[4] == buffer[5] &&
                buffer[5] == '\0')
            {
                _proc.StandardOutput.Read(buffer, 0, buffer.Length);
            }

            return string.Join("", buffer);
        }

        public void EndWait()
        {
            Write("$$end_wait$$");
        }

        public void Write(string msg)
        {
            if (!_running) throw new Exception("Interaction session is not running");
            _proc.StandardInput.WriteLine(msg);
            _proc.StandardInput.FlushAsync();
        }

        public void WaitProc()
        {
            if (Read() != "$$start_proc$$") return;
            while (Read() != "$$end_proc$$")
            {
                Thread.Sleep(1000);
            }
        }
    }
}