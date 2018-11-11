using NLog;
using RobotController.Communication.Configuration;
using RobotController.Communication.Enums;
using RobotController.Communication.Interfaces;
using RobotController.Communication.Utils;
using System;

namespace RobotController.Communication.Messages
{
    internal class MessageExtractor : MessageParser
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static int _lostDataCount;
        private static int _previousCounterValue;

        public void TryGetMessage(byte[] data)
        {
            if (CheckLengthAndMarkers(data))
            {
                if (CheckChecksumMatching(data))
                {
                    GetMessage(data);
                }
                else
                {
                    _logger.Fatal("Checksum mismatch, message dropped");
                }
            }
            else
            {
                _logger.Fatal("Framing mismatch, message dropped");
            }
        }

        private void GetMessage(byte[] data)
        {
            var payload = new byte[Framing.PayloadLength];
            Buffer.BlockCopy(data, Framing.CommandPosition+1, payload, 0, Framing.PayloadLength);

            var message = new Message
            {
                Counter = data[Framing.AddressPosition],
                Command = (EReceiverCommand)data[Framing.CommandPosition],
                Payload = payload,
                Checksum = GetFrameChecksum(data)
            };

            VerifyCounter(message);

            base.Parse(message);
        }

        private static bool CheckLengthAndMarkers(byte[] data) => (data[0] == Framing.FrameStart && data[Framing.FrameLength - 1] == Framing.FrameEnd);
        private static bool CheckChecksumMatching(byte[] data) => GetFrameChecksum(data) == CalculateFrameChecksum(data);
        private static ushort GetFrameChecksum(byte[] data) => BitConverter.ToUInt16(data, Framing.CrcStartByte);
        private static ushort CalculateFrameChecksum(byte[] data) => ChecksumUtils.CalculateCrc(data, 1, Framing.NumOfBytesToCrcCalculation);

        private void VerifyCounter(IMessage message)
        {
            //TODO
            //this might need a little more logic, now I'm just checking if current counter-1 = previous counter
            //this generates onMessageLost after runing if for first time
            if (message.Counter - _previousCounterValue > 1)
            {
                if (_lostDataCount < int.MaxValue)
                    _lostDataCount++;
                base.MessageLostOccured?.Invoke(this, new MessageLostEventArgs() { TotalLostCount = _lostDataCount });
            }

            _previousCounterValue = message.Counter;
        }
    }
}
