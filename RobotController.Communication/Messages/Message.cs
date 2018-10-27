using RobotController.Communication.Enums;
using RobotController.Communication.Interfaces;

namespace RobotController.Communication.Messages
{
    class Message : IMessage
    {
        public EReceiverCommand Command { get; set; }
        public byte DeviceAddress { get; set; }
        public object Payload { get; set; }
        public ushort Checksum { get; set; }
    }
}
