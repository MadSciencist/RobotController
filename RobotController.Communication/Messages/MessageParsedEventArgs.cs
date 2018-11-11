using RobotController.RobotModels;
using System;

namespace RobotController.Communication.Messages
{
    public class MessageParsedEventArgs : EventArgs
    {
        public SensorData LeftMotor { get; set; }
        public SensorData RightMotor { get; set; }
    }
}
