using System;
using System.Diagnostics;

namespace AspNetCore.Testing.Processes
{
    public class SimpleProcess : IDisposable 
    {
        private Process _process;
        private string _filename;
        private string _args;
        private bool _hideWindow;

        public SimpleProcess(string filename, string args, bool hideWindow)
        {
            _filename = filename;
            _args = args;
            _hideWindow = hideWindow;
        }

        public void StartProcess()
        {
            _process = new Process
            {
                StartInfo =
                {
                    FileName = _filename,
                    Arguments = _args
                },
                EnableRaisingEvents = true //Kills child processes
            };

            if (_hideWindow)
            {
                _process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                _process.StartInfo.CreateNoWindow = true;
                _process.StartInfo.UseShellExecute = false;
                _process.StartInfo.RedirectStandardOutput = true;
                _process.StartInfo.RedirectStandardError = true;
                _process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                _process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
            }

            _process.Start();

            if (_hideWindow)
            {
                _process.BeginOutputReadLine();
                _process.BeginErrorReadLine();
            }

            System.Threading.Thread.Sleep(10000);

            if (_process.HasExited)
            {
                throw new Exception(_filename + " failed to launch");
            }
        }

        static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            Debug.WriteLine(outLine.Data);
        }

        public bool IsRunning
        {
            get
            {
                return  !_process.HasExited;
            }
        }

        public void Dispose()
        {
            if (_process != null)
            {
                KillProcess();
            }
        }

        public void KillProcess()
        {
            if (_process != null)
            {
                try
                {
                    _process.KillProcessAndChildren();
                }
                catch
                {
                }
            }
        }
    }
}
