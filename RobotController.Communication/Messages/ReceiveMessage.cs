using RobotController.Communication.Enums;
using RobotController.Communication.Interfaces;

namespace RobotController.Communication.Messages
{
    public class ReceiveMessage : Message, IReceiveMessage
    {
        public EReceiverCommand Command { get; set; }
        public byte Counter { get; set; }
        public override object Payload { get; set; }
        public ushort Checksum { get; set; }
    }
}
