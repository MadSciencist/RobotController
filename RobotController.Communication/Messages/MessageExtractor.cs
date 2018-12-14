using NLog;
using RobotController.Communication.Configuration;
using RobotController.Communication.Enums;
using RobotController.Communication.Utils;
using System;
using RobotController.Communication.Interfaces;

namespace RobotController.Communication.Messages
{
    internal class MessageExtractor : MessageParser
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
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
                    Logger.Fatal("Checksum mismatch, message dropped");
                }
            }
            else
            {
                Logger.Fatal("Framing mismatch, message dropped");
            }
        }

        private void GetMessage(byte[] data)
        {
            var payload = new byte[ReceiverFraming.PayloadLength];
            Buffer.BlockCopy(data, ReceiverFraming.CommandPosition+1, payload, 0, ReceiverFraming.PayloadLength);

            var message = new ReceiveMessage
            {
                Counter = data[ReceiverFraming.AddressPosition],
                Command = (EReceiverCommand)data[ReceiverFraming.CommandPosition],
                Payload = payload,
                Checksum = GetFrameChecksum(data)
            };

            VerifyCounter(message);

            base.Parse(message);
        }

        private static bool CheckLengthAndMarkers(byte[] data) => (data[0] == ReceiverFraming.FrameStart && data[ReceiverFraming.FrameLength - 1] == ReceiverFraming.FrameEnd);
        private static bool CheckChecksumMatching(byte[] data) => GetFrameChecksum(data) == CalculateFrameChecksum(data);
        private static ushort GetFrameChecksum(byte[] data) => BitConverter.ToUInt16(data, ReceiverFraming.CrcStartByte);
        private static ushort CalculateFrameChecksum(byte[] data) => ChecksumUtils.CalculateCrc(data, 1, ReceiverFraming.NumOfBytesToCrcCalculation);

        private void VerifyCounter(IReceiveMessage message)
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
