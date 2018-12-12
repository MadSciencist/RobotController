using NLog;
using RobotController.Communication.Enums;
using RobotController.Communication.Interfaces;
using RobotController.RobotModels;
using System;
using System.Diagnostics;
using RobotController.RobotModels.PhysicalConverters;

namespace RobotController.Communication.Messages
{
    internal class MessageParser
    {
        public EventHandler<MessageParsingErrorEventArgs> ParsingErrorOccured;
        public EventHandler<MessageLostEventArgs> MessageLostOccured;//???
        public EventHandler KeepAliveReceived;
        public EventHandler<MessageParsedEventArgs> SpeedCurrentFeedbackReceived;
        public EventHandler<MessageParsedEventArgs> VoltageTemperatureFeedbackReceived;
        public event EventHandler<MessageParsedEventArgs> ParametersReceived;

        private readonly ParametersModel _parameters = ParametersModel.GetParameters();
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Parse(IReceiveMessage message)
        {
            try
            {
                var payload = (byte[])message.Payload;

                switch (message.Command)
                {
                    case EReceiverCommand.KeepAlive:
                        KeepAliveReceived?.Invoke(this, EventArgs.Empty);
                        Debug.WriteLine("Keep alive recieved");
                        break;

                    case EReceiverCommand.FeedbackSpeedCurrent:
                        {
                            var leftRawCurrent = BitConverter.ToInt16(payload, 2);
                            var leftRawVelocity = BitConverter.ToInt16(payload, 0);
                            var rightRawCurrent = BitConverter.ToInt16(payload, 6);
                            var rightRawVelocity = BitConverter.ToInt16(payload, 4);

                            var parsed = new MessageParsedEventArgs
                            {
                                LeftMotor = new SpeedCurrentFeedbackModel
                                {
                                    Velocity = VelocityConverter.GetPhysical(leftRawVelocity),
                                    Current = CurrentConverter.GetPhysical(leftRawCurrent),
                                    RawCurrent = leftRawCurrent
                                },
                                RightMotor = new SpeedCurrentFeedbackModel
                                {
                                    Velocity = VelocityConverter.GetPhysical(rightRawVelocity),
                                    Current = CurrentConverter.GetPhysical(rightRawCurrent),
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
                                    Voltage = VoltageConverter.GetPhysical(BitConverter.ToInt16(payload, 0)),
                                    Temperature = TemperatureConverter.GetPhysical(BitConverter.ToInt16(payload, 2)),
                                }
                            };

                            VoltageTemperatureFeedbackReceived?.Invoke(this, parsed);
                        }
                        break;

                    case EReceiverCommand.ControlType:
                        _parameters.ControlType = payload[0];
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.RegenerativeBreaking:
                        _parameters.UseRegenerativeBreaking = payload[0] != 0;
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.PidKp_1:
                        _parameters.PidLeft.Kp = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.PidKi_1:
                        _parameters.PidLeft.Ki = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.PidKd_1:
                        _parameters.PidLeft.Kd = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.PidIntegralLimit_1:
                        _parameters.PidLeft.IntegralLimit = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.PidClamping_1:
                        _parameters.PidLeft.OutputLimit = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.PidDeadband_1:
                        _parameters.PidLeft.Deadband = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.PidPeriod_1:
                        _parameters.PidLeft.Period = BitConverter.ToUInt16(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.PidKp_2:
                        _parameters.PidRight.Kp = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.PidKi_2:
                        _parameters.PidRight.Ki = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.PidKd_2:
                        _parameters.PidRight.Kd = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.PidIntegralLimit_2:
                        _parameters.PidRight.IntegralLimit = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.PidClamping_2:
                        _parameters.PidRight.OutputLimit = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.PidDeadband_2:
                        _parameters.PidRight.Deadband = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.PidPeriod_2:
                        _parameters.PidRight.Period = BitConverter.ToUInt16(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
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
