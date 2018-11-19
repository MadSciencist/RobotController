using RobotController.Gamepad.Interfaces;
using System;

namespace RobotController.Gamepad.Config
{
    public class SteeringConfig : ISteeringConfig
    {
        public short Deadband { get; set; } = 10;
        public short Centervalue { get; set; } = 255;
        public bool IsReversed { get; set; } = false;
        public bool IsLeftRightReverse { get; set; } = false;
        public bool UseExponentialCurve { get; set; } = true;
        public short ExponentialCurveCoefficient { get; set; } = 5;
        public bool UseLowPassFilter { get; set; } = true;

        public int LowPassCoefficient
        {
            get { return _lowPassCoefficient ?? 92 ; } //set default value
            set
            {
                if (value > 100) throw new ArgumentException("Coefficient should be in 0-99 range");
                _lowPassCoefficient = value;
            }
        } 

        private int? _lowPassCoefficient;
    }
}
