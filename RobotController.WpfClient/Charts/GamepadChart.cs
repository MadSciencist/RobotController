using System.Collections;
using System.Collections.Generic;
using LiveCharts;
using LiveCharts.Wpf;

namespace RobotController.WpfGui.Charts
{
    public class GamepadChart
    {
        //handle for the chart 
        public SeriesCollection SeriesCollection { get; set; }
        public ICollection<int> ExponentialCurveValues { get; set; }

        public void UpdateExponentialCurveChart(ICollection<int> newValues)
        {
            ExponentialCurveValues = newValues;
        }

        public GamepadChart()
        {
            ExponentialCurveValues = new List<int>(256);

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<short> { 3, 5, 7, 4 }
                },
                new LineSeries
                {
                    Values = new ChartValues<int>()
                }
            };
        }
    }
}
