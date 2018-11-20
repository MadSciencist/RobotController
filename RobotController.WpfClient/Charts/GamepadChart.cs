using LiveCharts;
using LiveCharts.Defaults;
using System.Collections.Generic;
using System.Windows;
using LiveCharts.Geared;

namespace RobotController.WpfGui.Charts
{
    public class GamepadChart
    {
        public  GearedValues<ObservablePoint> PointChartValues { get; set; }
        public GearedValues<ObservablePoint> LineChartValues { get; set; }
        public GearedValues<short> ExpoChartValues { get; set; }

        private readonly ObservablePoint _point;

        public GamepadChart()
        {
            var lineChartPoints = new List<ObservablePoint> {new ObservablePoint(0, 0), new ObservablePoint(255, 255)};
            LineChartValues = new GearedValues<ObservablePoint>();
            LineChartValues.AddRange(lineChartPoints);

            _point = new ObservablePoint(0, 0);
            PointChartValues = new GearedValues<ObservablePoint> { _point };

            ExpoChartValues = new GearedValues<short>();
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
