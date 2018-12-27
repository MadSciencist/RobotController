using LiveCharts;
using LiveCharts.Configurations;
using RobotController.WpfGui.ViewModels;
using System;
using System.Collections.Generic;

namespace RobotController.WpfGui.Charts
{
    public class FeedbackChart : ObservableEntity
    {
        //public bind models to be consumed by XAML
        public ChartValues<MeasurementModel> MotorLeftValues { get; set; }
        public ChartValues<MeasurementModel> MotorRightValues { get; set; }
        public Func<double, string> DateTimeFormatter { get; set; }
        public Func<double, string> YAxisFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }

        private double _axisMax;
        private double _axisMin;
        private static List<MeasurementModel> historyLeft, historyRight;

        public FeedbackChart()
        {
            //initialize series storage
            MotorLeftValues = new ChartValues<MeasurementModel>();
            MotorRightValues = new ChartValues<MeasurementModel>();

            //register and setup current chart model
            var mapper = Mappers.Xy<MeasurementModel>()
                .X(model => model.DateTime.Ticks)
                .Y(model => model.Value);

            Charting.For<MeasurementModel>(mapper);

            //initialize formatter
            DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss");
            YAxisFormatter = value => $"{value:#.##}";

            //AxisStep forces the distance between each separator in the X axis
            AxisStep = TimeSpan.FromSeconds(1).Ticks;
            //AxisUnit forces lets the axis know that we are plotting seconds
            //this is not always necessary, but it can prevent wrong labeling
            AxisUnit = TimeSpan.TicksPerSecond;

            SetAxisLimits(DateTime.Now);

            historyLeft = new List<MeasurementModel>();
            historyRight = new List<MeasurementModel>();
        }

        public void AddNewPoint(MeasurementModel left, MeasurementModel right)
        {
            MotorLeftValues.Add(left);
            MotorRightValues.Add(right);

            if (MotorLeftValues.Count > 120) MotorLeftValues.RemoveAt(0);
            if (MotorRightValues.Count > 120) MotorRightValues.RemoveAt(0);

            SetAxisLimits(DateTime.Now);
        }
        public void AddNewPoints(IList<MeasurementModel> left, IList<MeasurementModel> right)
        {
            const int historicalSamples = 130;

            historyLeft.AddRange(left);
            historyRight.AddRange(right);

            if (historyLeft.Count > historicalSamples) historyLeft.RemoveRange(0, historyLeft.Count - historicalSamples);
            if (historyRight.Count > historicalSamples) historyRight.RemoveRange(0, historyRight.Count - historicalSamples);

            MotorLeftValues.Clear();
            MotorRightValues.Clear();

            MotorLeftValues.AddRange(historyLeft);
            MotorRightValues.AddRange(historyRight);

            SetAxisLimits(DateTime.Now);
        }

        public double AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                OnPropertyChanged(nameof(AxisMax));
            }
        }
        public double AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                OnPropertyChanged(nameof(AxisMin));
            }
        }

        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 1 second ahead
            AxisMin = now.Ticks - TimeSpan.FromSeconds(10).Ticks; // and 20 seconds behind
        }
    }
}
