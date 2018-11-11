using NLog;
using RobotController.Communication.Enums;
using RobotController.Communication.Interfaces;
using RobotController.RobotModels;
using System;

namespace RobotController.Communication.Messages
{
    internal class MessageParser
    {
        public EventHandler<MessageParsingErrorEventArgs> ParsingErrorOccured;
        public EventHandler<MessageLostEventArgs> MessageLostOccured;
        public EventHandler<MessageParsedEventArgs> KeepAliveReceived;
        public EventHandler<MessageParsedEventArgs> FeedbackReceived;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Parse(IMessage message)
        {
            try
            {
                switch (message.Command)
                {
                    case EReceiverCommand.KeepAlive:
                        KeepAliveReceived?.Invoke(this, new MessageParsedEventArgs());
                        break;

                    case EReceiverCommand.Feedback:
                        var parsed = new MessageParsedEventArgs
                        {
                            LeftMotor = new SensorData
                            {
                                RawSpeed = BitConverter.ToInt16(message.Payload as byte[], 0),
                                RawCurrent = BitConverter.ToInt16(message.Payload as byte[], 2),
                                RawVoltage = BitConverter.ToInt16(message.Payload as byte[], 4),
                                RawTemperature = BitConverter.ToInt16(message.Payload as byte[], 6)
                            },
                            RightMotor = new SensorData
                            {
                                RawSpeed = BitConverter.ToInt16(message.Payload as byte[], 8),
                                RawCurrent = BitConverter.ToInt16(message.Payload as byte[], 10),
                                RawVoltage = BitConverter.ToInt16(message.Payload as byte[], 12),
                                RawTemperature = BitConverter.ToInt16(message.Payload as byte[], 14)
                            }
                        };

                        FeedbackReceived?.Invoke(this, parsed);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ParsingErrorOccured?.Invoke(this, new MessageParsingErrorEventArgs(e));
            }
        }
    }
}
