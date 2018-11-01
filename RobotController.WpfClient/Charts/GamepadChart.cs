using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Wpf;

namespace RobotController.WpfGui.Charts
{
    public class GamepadChart
    {
        public SeriesCollection SeriesCollection { get; set; }
        public GamepadChart()
        {
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<short> { 3, 5, 7, 4 },
                },
            };
        }
    }
}
