using RobotController.Communication.Configuration;
using RobotController.Communication.Enums;
using RobotController.Communication.Interfaces;
using RobotController.Communication.Utils;
using System;

namespace RobotController.Communication.Messages
{
    public class MessageParser
    {
        public EventHandler<MessageParsedEventArgs> MessageParsed;

        public void TryGetMessage(byte[] data, int lentgh)
        {
            if (CheckLengthAndMarkers(data))
            {
                if (CheckChecksum(data))
                {
                    GetMessage(data);
                }
                else
                {
                    throw new NotImplementedException(); ;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void GetMessage(byte[] data)
        {
            byte[] payload = new byte[Framing.PayloadLength];
            Buffer.BlockCopy(data, 3, payload, 0, Framing.PayloadLength);

            var message = new Message
            {
                DeviceAddress = data[1],
                Command = (EReceiverCommand)data[2],
                Payload = payload,
                Checksum = BitConverter.ToUInt16(data, 10)
            };

            OnMessageParsed(message);
        }

        protected void OnMessageParsed(IMessage message)
        {
            MessageParsed?.Invoke(this, new MessageParsedEventArgs { Message = message });
        }

        private bool CheckChecksum(byte[] data)
        {
            return GetFrameChecksum(data) == CalculateFrameChecksum(data);
        }


        private ushort GetFrameChecksum(byte[] data)
        {
            return BitConverter.ToUInt16(data, 10);
        }

        private ushort CalculateFrameChecksum(byte[] data)
        {
            //verify index
            return BitConverter.ToUInt16(ChecksumUtils.CalculateCrc(data), 0);
        }

        private bool CheckLengthAndMarkers(byte[] data)
        {
            if (data[0] == Framing.FrameStart && data[Framing.FrameLength] == Framing.FrameEnd)
                return true;
            else return false;
        }
    }
}
