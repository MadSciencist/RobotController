using RobotController.Communication.Enums;
using RobotController.Communication.Interfaces;

namespace RobotController.Communication.Messages
{
    internal class Message : IMessage
    {
        public EReceiverCommand Command { get; set; }
        public byte Counter { get; set; }
        public object Payload { get; set; }
        public ushort Checksum { get; set; }
    }
}
