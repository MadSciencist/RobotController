using System;
using RobotController.Communication.Configuration;
using RobotController.Communication.Interfaces;
using RobotController.Communication.Utils;
using RobotController.RobotModels;

namespace RobotController.Communication.Messages
{
    public class MessageGenerator
    {
        public byte[] Generate(ISendMessage command)
        {
            var payload = GetBytes(command.Payload);

            var buffer = new byte[14];

            buffer[0] = (byte)ReceiverFraming.FrameStart;
            buffer[1] = (byte)command.Node;
            buffer[2] = (byte)command.CommandType;

            Buffer.BlockCopy(payload, 0, buffer, 3, payload.Length);

            var crcBytes = CalculateCrc(buffer);

            buffer[11] = crcBytes[0];
            buffer[12] = crcBytes[1];
            buffer[13] = (byte)ReceiverFraming.FrameEnd;

            return buffer;
        }

        private static byte[] GetBytes(object payload)
        {
            var bytes = new byte[8];

            switch (payload)
            {
                case ControlsModel data when payload is ControlsModel:
                    var left = BitConverter.GetBytes(data.LeftSpeed);
                    var right = BitConverter.GetBytes(data.RightSpeed);
                    Buffer.BlockCopy(left, 0, bytes, 0, 2);
                    Buffer.BlockCopy(right, 0, bytes, 2, 2);
                    break;

                case float data when payload is float:
                    bytes = BitConverter.GetBytes(data);
                    break;

                case double data when payload is double:
                    bytes = BitConverter.GetBytes(data);
                    break;

                case byte data when payload is byte:
                    bytes = BitConverter.GetBytes(data);
                    break;

                case sbyte data when payload is sbyte:
                    bytes = BitConverter.GetBytes(data);
                    break;

                case short data when payload is short:
                    bytes = BitConverter.GetBytes(data);
                    break;

                case ushort data when payload is ushort:
                    bytes = BitConverter.GetBytes(data);
                    break;

                case int data when payload is int:
                    bytes = BitConverter.GetBytes(data);
                    break;

                case uint data when payload is uint:
                    bytes = BitConverter.GetBytes(data);
                    break;

                case long data when payload is long:
                    bytes = BitConverter.GetBytes(data);
                    break;

                case ulong data when payload is ulong:
                    bytes = BitConverter.GetBytes(data);
                    break;

                case byte[] data when (payload is byte[]):
                    Buffer.BlockCopy(data, 0, bytes, 0, 8);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return bytes;
        }

        private static byte[] CalculateCrc(byte[] buffer)
        {
            var crc = ChecksumUtils.CalculateCrc(buffer, 1, 10);
            var crcBytes = BitConverter.GetBytes(crc);
            return crcBytes;
        }
    }
}
