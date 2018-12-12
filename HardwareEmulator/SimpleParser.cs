using HardwareEmulator.Enums;
using System;

namespace HardwareEmulator
{
    public class SimpleParser
    {
        public event EventHandler<string> Received;

        public void Parse(byte[] data)
        {
            var payload = new byte[8];
            var node = (ENode)data[1];
            var command = (EReceiver)data[2];
            Buffer.BlockCopy(data, 3, payload, 0, 8);

            switch (command)
            {
                case EReceiver.EepromRead:
                    Received?.Invoke(this, Format(node, command, ""));
                    break;

                case EReceiver.EepromWrite:
                    Received?.Invoke(this, Format(node, command, ""));
                    break;

                case EReceiver.AllowMovement:
                    Received?.Invoke(this, Format(node, command, ""));
                    break;

                case EReceiver.StopMovement:
                    Received?.Invoke(this, Format(node, command, ""));
                    break;

                case EReceiver.KeepAlive:
                    Received?.Invoke(this, Format(node, command, ""));
                    break;

                case EReceiver.PidKp:
                    var kp = BitConverter.ToDouble(payload, 0);
                    Received?.Invoke(this, Format(node, command, kp));
                    break;

                case EReceiver.PidKi:
                    var ki = BitConverter.ToDouble(payload, 0);
                    Received?.Invoke(this, Format(node, command, ki));
                    break;

                case EReceiver.PidKd:
                    var kd = BitConverter.ToDouble(payload, 0);
                    Received?.Invoke(this, Format(node, command, kd));
                    break;

                default:
                    break;
            }
        }

        private string Format(ENode node, EReceiver command, object data)
        {
            return $"[RECEIVED] To node: {node.ToString()}  Cmd:{command.ToString()} Data: {data.ToString()}";
        }
    }
}
