using System;
using RobotController.Communication.Enums;

namespace RobotController.Communication.Interfaces
{
    public interface ICommand
    {
        ESenderCommand CommandType { get; set; }
        object Payload { get; set; }
    }
}
