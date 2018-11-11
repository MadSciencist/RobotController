using RobotController.Communication.Enums;
using RobotController.Communication.Interfaces;

namespace RobotController.Communication.Messages
{
    public class Command : ICommand
    {
        public ESenderCommand CommandType { get; set; }
        public object Payload { get; set; }
    }
}
