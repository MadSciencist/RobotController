using System;
using RobotController.Communication.Enums;
using RobotController.Communication.Interfaces;

namespace RobotController.Communication.Messages
{
    public class SendMessage : Message, ISendMessage
    {
        public ENode Node { get; set; }
        public ESenderCommand CommandType { get; set; }
        public override object Payload { get; set; }
    }
}
