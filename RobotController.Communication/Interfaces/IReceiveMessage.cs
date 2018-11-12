using RobotController.Communication.Enums;

namespace RobotController.Communication.Interfaces
{
    public interface IReceiveMessage
    {
        EReceiverCommand Command { get; set; }
        byte Counter { get; set; }
        object Payload { get; set; }
        ushort Checksum { get; set; }
    }
}