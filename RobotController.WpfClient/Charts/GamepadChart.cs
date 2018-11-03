using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Charts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Definitions.Series;
using LiveCharts.Dtos;
using LiveCharts.Helpers;
using LiveCharts.Wpf;

namespace RobotController.WpfGui.Charts
{
    public class GamepadChart
    {
        //observable for the chart 
        public SeriesCollection SeriesCollection { get; set; }


        public Func<double, string> DateTimeFormatter { get; set; }

        //for live point
        private readonly ChartValues<ObservablePoint> _pointSeries;
        private ObservablePoint _point;


        private ChartValues<MeasurementModel> robotChart;
        public GamepadChart()
        {
            var line = new List<ObservablePoint>();
            line.Add(new ObservablePoint(0, 0));
            line.Add(new ObservablePoint(255, 255));


            //  var lineSeries = new LineSeries(){ Title = "Line series", Values = new ChartValues<int>(line)};

            _point = new ObservablePoint(0, 0);
            _pointSeries = new ChartValues<ObservablePoint> { _point };

            var mapper = Mappers.Xy<MeasurementModel>()
                .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y

            //lets save the mapper globally.
            Charting.For<MeasurementModel>(mapper);
            robotChart = new ChartValues<MeasurementModel>();
            //lets set how to display the X Labels
            DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss");


            SeriesCollection = new SeriesCollection
            {
                new LineSeries() { Title = "Single point", Values = _pointSeries, PointGeometrySize = 20},
                new LineSeries() { Values = new ChartValues<ObservablePoint>(line)}
            };
        }


        public void UpdateLivePointChart(double x, double y)
        {
            _point.X = x;
            _point.Y = y;
        }

        public void AddValue(short val)
        {
            robotChart.Add(new MeasurementModel
            {
                DateTime = DateTime.Now,
                Value = val
            });
        }
    }
}
