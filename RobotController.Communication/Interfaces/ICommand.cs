using System;
using RobotController.Communication.Enums;

namespace RobotController.Communication.Interfaces
{
    /// <summary>
    /// Sending message
    /// </summary>
    public interface ICommand
    {
        ENode Node { get; set; }
        ESenderCommand CommandType { get; set; }
        object Payload { get; set; }
    }
}
