using System;
using System.IO;
using Microsoft.Win32;

namespace RobotController.WpfGui.Infrastructure
{
    public class DataLogSaveDialog
    {
        public string SelectDatalogPath()
        {
            var fileName = $"log{DateTime.Now.ToString("yy_MM_dd_hh", System.Globalization.CultureInfo.InvariantCulture)}.csv";

            var dialog = new SaveFileDialog
            {
                FileName = fileName,
                DefaultExt = ".csv", // Default file extension
                Filter = @"Text documents (.csv)|*.csv" // Filter files by extension
            };

            var result = dialog.ShowDialog();

            if (result == true)
            {
                return dialog.FileName;
            }

            return Path.Combine(@".\", fileName);
        }
    }
}
