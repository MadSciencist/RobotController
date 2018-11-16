using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiveCharts;
using LiveCharts.Configurations;
using RobotController.WpfGui.ViewModels;

namespace RobotController.WpfGui.Charts
{
    public class SpeedFeedbackChart : ObservableEntity
    {
        //public bind models to be consumed by XAML
        public ChartValues<MeasurementModel> MotorLeftValues { get; set; }
        public ChartValues<MeasurementModel> MotorRightValues { get; set; }
        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }

        private double _axisMax;
        private double _axisMin;

        public SpeedFeedbackChart()
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

            //AxisStep forces the distance between each separator in the X axis
            AxisStep = TimeSpan.FromSeconds(2).Ticks;
            //AxisUnit forces lets the axis know that we are plotting seconds
            //this is not always necessary, but it can prevent wrong labeling
            AxisUnit = TimeSpan.TicksPerSecond;

            SetAxisLimits(DateTime.Now);
        }

        public void AddNewPoint(short left, short right)
        {
            var now = DateTime.Now;

            MotorLeftValues.Add(new MeasurementModel()
            {
                DateTime = now,
                Value = left
            });

            MotorRightValues.Add(new MeasurementModel()
            {
                DateTime = now,
                Value = right
            });

            SetAxisLimits(now);

            //keep only last X values
            if (MotorLeftValues.Count > 200) MotorLeftValues.RemoveAt(0);
            if (MotorRightValues.Count > 200) MotorRightValues.RemoveAt(0);
        }

        public void AddNewPoints(IList<MeasurementModel> left, IList<MeasurementModel> right)
        {
            MotorLeftValues.AddRange(left);
            MotorRightValues.AddRange(right);

            SetAxisLimits(DateTime.Now);

            //keep only last X values
            //TODO PITTY!!! Dispatcher timer does nothing, cause charts dont have remove range method :( waiting for next release to fix this
            while (MotorLeftValues.Count > 200) MotorLeftValues.RemoveAt(0);
            while (MotorRightValues.Count > 200) MotorRightValues.RemoveAt(0);
        }

        public double AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                OnPropertyChanged("AxisMax");
            }
        }
        public double AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                OnPropertyChanged("AxisMin");
            }
        }

        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 1 second ahead
            AxisMin = now.Ticks - TimeSpan.FromSeconds(20).Ticks; // and 20 seconds behind
        }
    }
}
