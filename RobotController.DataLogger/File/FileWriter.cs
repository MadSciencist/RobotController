using System;
using System.Diagnostics;
using System.IO;

namespace RobotController.DataLogger.File
{
    public class FileWriter : IDisposable, IFileWriter
    {
        private readonly ILogConfig _config;
        private readonly StreamWriter _writer;

        public FileWriter(ILogConfig config)
        {
            _config = config;
            _writer = new StreamWriter(_config.Path);

            WriteHeader();
        }

        public void WriteLine(string line)
        {
            TryWriteLine(line);
        }

        private void TryWriteLine(string line)
        {
            try
            {
                _writer.WriteLine(line);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);

            }
            finally
            {
                _writer.Flush();
                _writer.Dispose();
            }
        }

        private void WriteHeader()
        {
            TryWriteLine($"Date: {DateTime.Now.ToString()}");
        }

        public void Dispose()
        {
            _writer?.Dispose();
        }
    }
}
