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
        public EventHandler<MessageParsedEventArgs> SpeedCurrentFeedbackReceived;
        public EventHandler<MessageParsedEventArgs> VoltageTemperatureFeedbackReceived;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Parse(IReceiveMessage message)
        {
            try
            {
                switch (message.Command)
                {
                    case EReceiverCommand.KeepAlive:
                        KeepAliveReceived?.Invoke(this, new MessageParsedEventArgs());
                        Console.WriteLine("Keep alive recieved");
                        break;

                    case EReceiverCommand.FeedbackSpeedCurrent:
                        {
                            var parsed = new MessageParsedEventArgs
                            {
                                LeftMotor = new SpeedCurrentFeedbackModel
                                {
                                    RawSpeed = BitConverter.ToInt16(message.Payload as byte[], 0),
                                    RawCurrent = BitConverter.ToInt16(message.Payload as byte[], 2),
                                },
                                RightMotor = new SpeedCurrentFeedbackModel
                                {
                                    RawSpeed = BitConverter.ToInt16(message.Payload as byte[], 4),
                                    RawCurrent = BitConverter.ToInt16(message.Payload as byte[], 6),
                                }
                            };

                            SpeedCurrentFeedbackReceived?.Invoke(this, parsed);
                        }
                        break;

                    case EReceiverCommand.FeedbackVoltageTemperature:
                        {
                            var parsed = new MessageParsedEventArgs
                            {
                                VoltageTemperatureFeedbackModel = new VoltageTemperatureFeedbackModel()
                                {
                                    RawVoltage = BitConverter.ToInt16(message.Payload as byte[], 0),
                                    RawTemperature = BitConverter.ToInt16(message.Payload as byte[], 2),
                                }
                            };

                            VoltageTemperatureFeedbackReceived?.Invoke(this, parsed);
                        }
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
