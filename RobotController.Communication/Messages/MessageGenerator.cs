using System;
using RobotController.Communication.Interfaces;

namespace RobotController.Communication.Messages
{
    public class MessageGenerator
    {
        public byte[] Generate(ICommand command)
        {
            return new byte[50];
        }
    }
}
