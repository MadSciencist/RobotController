using RobotController.Communication.Enums;

namespace RobotController.Communication.Interfaces
{
    public interface ISendMessage
    {
        ENode Node { get; set; }
        ESenderCommand CommandType { get; set; }
        object Payload { get; set; }
    }
}