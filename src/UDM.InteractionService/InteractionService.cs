using System.Diagnostics;

namespace UDM.InteractionService
{
    public class InteractionService
    {
        readonly Process _proc = new();
        private bool running = false;

        public InteractionService(string pathToScript)
        {
            _proc.StartInfo.WorkingDirectory = @"C:\Users\maksi\AppData\Local\Programs\Python\Python312";
            _proc.StartInfo.FileName = "python.exe";
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
            running = true;
        }

        public string Read()
        {
            if (!running) throw new Exception("Interaction session is not running");
            var buffer = new char[1000];
            _proc.StandardOutput.Read(buffer, 0, buffer.Length);
            return string.Join("", buffer);
        }

        public void EndWait()
        {
            Write("$$end_wait$$");
        }

        public void Write(string msg)
        {
            if (!running) throw new Exception("Interaction session is not running");
            _proc.StandardInput.WriteLine(msg);
            _proc.StandardInput.FlushAsync();
        }

    }
}
