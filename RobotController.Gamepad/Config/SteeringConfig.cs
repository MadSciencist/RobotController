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

        /// <summary>
        /// It should be withing 20-100 range
        /// </summary>
        public short VelocityBoundPercentage
        {
            get => _velocityBoundPercentage ?? 75;
            set
            {
                if(value < 20 && value > 100) throw new ArgumentException("Boundary should be in 20-100 range");
                _velocityBoundPercentage = value;
            }
        }

        private short? _velocityBoundPercentage;

        public int LowPassCoefficient
        {
            get => _lowPassCoefficient ?? 92; //set default value
            set
            {
                if (value > 100) throw new ArgumentException("Coefficient should be in 0-99 range");
                _lowPassCoefficient = value;
            }
        } 

        private int? _lowPassCoefficient;
    }
}
