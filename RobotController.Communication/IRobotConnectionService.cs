using System;
using RobotController.Communication.Enums;
using RobotController.Communication.Interfaces;
using RobotController.Communication.Messages;

namespace RobotController.Communication
{
    public interface IRobotConnectionService
    {
        event EventHandler<MessageParsedEventArgs> SpeedCurrentFeedbackReceived;
        event EventHandler<MessageParsedEventArgs> VoltageTemperatureFeedbackReceived;
        event EventHandler<MessageParsedEventArgs> ParametersReceived;
        event EventHandler<EventArgs> TimeoutOccured;
        void SendCommand(ISendMessage command, EPriority priority);
        void Dispose();
    }
}