using RobotController.RobotModels;
using System;

namespace RobotController.Communication.Messages
{
    public class MessageParsedEventArgs : EventArgs
    {
        public SpeedCurrentFeedbackModel LeftMotor { get; set; }
        public SpeedCurrentFeedbackModel RightMotor { get; set; }
        public VoltageTemperatureFeedbackModel VoltageTemperatureFeedbackModel { get; set; }
        public ParametersModel Parameters { get; set; }
    }
}
