using LiveCharts;
using LiveCharts.Defaults;
using System.Collections.Generic;
using System.Windows;

namespace RobotController.WpfGui.Charts
{
    public class GamepadChart
    {
        public ChartValues<ObservablePoint> PointChartValues { get; set; }
        public ChartValues<ObservablePoint> LineChartValues { get; set; }
        public ChartValues<short> ExpoChartValues { get; set; }

        private readonly ObservablePoint _point;

        public GamepadChart()
        {
            var lineChartPoints = new List<ObservablePoint> { new ObservablePoint(0, 0), new ObservablePoint(255, 255) };
            LineChartValues = new ChartValues<ObservablePoint>();
            LineChartValues.AddRange(lineChartPoints);

            _point = new ObservablePoint(0, 0);
            PointChartValues = new ChartValues<ObservablePoint> { _point };

            ExpoChartValues = new ChartValues<short>();
        }

        public void UpdateExpoChart(IList<short> values)
        {
            ExpoChartValues.Clear();
            ExpoChartValues.AddRange(values);
        }

        public void UpdateLivePointChart(double x, double y)
        {
            _point.X = x;
            _point.Y = y;
        }

        public void UpdateLivePointChart(Point point)
        {
            _point.X = point.X;
            _point.Y = point.Y;
        }
    }
}
