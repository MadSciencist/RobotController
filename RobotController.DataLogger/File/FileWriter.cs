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
            var fileName = $"log{DateTime.Now.ToString("yy_MM_dd_hh", System.Globalization.CultureInfo.InvariantCulture)}.csv";
            var path = Path.Combine(_config.Path, fileName);

            using (var writer = new StreamWriter(path, append: true, encoding: System.Text.Encoding.UTF8))
            {
                writer.WriteLine(line);
            }
        }
    }
}
