﻿using System;
using RobotController.Communication.Configuration;
using RobotController.Communication.Interfaces;
using RobotController.Communication.Utils;
using RobotModels;

namespace RobotController.Communication.Messages
{
    public class MessageGenerator
    {
        public byte[] Generate(ICommand command)
        {
            var payload = GetBytes(command);

            var buffer = new byte[14];

            buffer[0] = (byte)Framing.FrameStart;
            buffer[1] = (byte)command.Node;
            buffer[2] = (byte)command.CommandType;

            Buffer.BlockCopy(payload, 0, buffer, 3, payload.Length);

            var crcBytes = CalculateCrc(buffer);

            buffer[11] = crcBytes[0];
            buffer[12] = crcBytes[1];
            buffer[13] = (byte)Framing.FrameEnd;

            return buffer;
        }

        private static byte[] GetBytes(ICommand command)
        {
            var payload = new byte[8];

            switch (command.Payload)
            {
                case double data when (command.Payload is double):
                    payload = BitConverter.GetBytes(data);
                    break;

                case short data when (command.Payload is short):
                    payload = BitConverter.GetBytes(data);
                    break;

                case ushort data when (command.Payload is ushort):
                    payload = BitConverter.GetBytes(data);
                    break;

                case int data when (command.Payload is int):
                    payload = BitConverter.GetBytes(data);
                    break;

                case ControlsModel data when (command.Payload is ControlsModel):
                    var left = BitConverter.GetBytes(data.LeftSpeed);
                    var right = BitConverter.GetBytes(data.RightSpeed);
                    Buffer.BlockCopy(left, 0, payload, 0, 2);
                    Buffer.BlockCopy(right, 0, payload, 2, 2);
                    break;
                    
                default:
                    throw new NotImplementedException();
            }

            return payload;
        }

        private static byte[] CalculateCrc(byte[] buffer)
        {
            var crc = ChecksumUtils.CalculateCrc(buffer, 1, 10);
            var crcBytes = BitConverter.GetBytes(crc);
            return crcBytes;
        }
    }
}