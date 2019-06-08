using System;

namespace RobotController.WpfGui.BusinessLogic
{
    public class DoubleStepExperimentParams
    {
        public short FirstStepVelocity { get; set; }
        public short SecondStepVelocity { get; set; }
        public TimeSpan FirstStepLength { get; set; }
        public TimeSpan SecondStepLength { get; set; }
    }
}