﻿using RobotController.Communication.Configuration;
using RobotController.Communication.Enums;
using RobotController.Communication.Utils;
using System;
using System.Diagnostics;

namespace RobotController.Communication.Messages
{
    public class MessageExtractor : MessageParser
    {
        static int messageCount = 0;

        public void TryGetMessage(byte[] data, int lentgh)
        {
            if (CheckLengthAndMarkers(data))
            {
                if (CheckChecksumMatching(data))
                {
                    GetMessage(data);
                    Debug.WriteLine($"Message parsed, count: {++messageCount}");
                }
                else
                {
                    Debug.WriteLine("Checksum mismatch, message dropped");
                }
            }
            else
            {
                Debug.WriteLine("Framing mismatch, message dropped");
            }
        }

        private void GetMessage(byte[] data)
        {
            byte[] payload = new byte[Framing.PayloadLength];
            Buffer.BlockCopy(data, Framing.CommandPosition+1, payload, 0, Framing.PayloadLength);

            var message = new Message
            {
                DeviceAddress = data[Framing.AddressPosition],
                Command = (EReceiverCommand)data[Framing.CommandPosition],
                Payload = payload,
                Checksum = GetFrameChecksum(data)
            };

            base.Parse(message);
        }

        private bool CheckLengthAndMarkers(byte[] data) => (data[0] == Framing.FrameStart && data[Framing.FrameLength - 1] == Framing.FrameEnd);
        private bool CheckChecksumMatching(byte[] data) => GetFrameChecksum(data) == CalculateFrameChecksum(data);
        private ushort GetFrameChecksum(byte[] data) => BitConverter.ToUInt16(data, Framing.CrcStartByte);
        private ushort CalculateFrameChecksum(byte[] data) => ChecksumUtils.CalculateCrc(data, 1, Framing.NumOfBytesToCrcCalculation);
    }
}