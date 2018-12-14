using System;
using System.IO;

namespace RobotController.DataLogger.File
{
    public class FileWriter : IFileWriter
    {
        private readonly ILogConfig _config;

        public FileWriter(ILogConfig config)
        {
            _config = config;
        }

        public void WriteLine(string line)
        {
            using (var writer = new StreamWriter(_config.Path, append: true, encoding: System.Text.Encoding.UTF8))
            {
                writer.WriteLine(line);
            }
        }
    }
}
