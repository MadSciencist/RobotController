using NLog;
using RobotController.Communication.Enums;
using RobotController.Communication.Interfaces;
using RobotController.RobotModels;
using RobotController.RobotModels.PhysicalConverters;
using System;
using System.Diagnostics;

namespace RobotController.Communication.Messages
{
    public class MessageParser
    {
        // infrastructure events
        public EventHandler<MessageParsingErrorEventArgs> ParsingErrorOccured; //when got exception while parsing
        public EventHandler<MessageLostEventArgs> MessageLostOccured;//???

        // actual received & parsed messages
        public EventHandler KeepAliveReceived;
        public EventHandler<MessageParsedEventArgs> SpeedCurrentFeedbackReceived;
        public EventHandler<MessageParsedEventArgs> VoltageTemperatureFeedbackReceived;
        public event EventHandler<MessageParsedEventArgs> ParametersReceived;

        //singleton to keep all parameters in same object, so we can notify UI at once
        private readonly ParametersModel _parameters = ParametersModel.GetParameters();

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
                                    RawVelocity = leftRawVelocity,
                                    Current = CurrentConverter.GetPhysical(leftRawCurrent),
                                    RawCurrent = leftRawCurrent
                                },
                                RightMotor = new SpeedCurrentFeedbackModel
                                {
                                    Velocity = VelocityConverter.GetPhysical(rightRawVelocity),
                                    RawVelocity = rightRawVelocity,
                                    Current = CurrentConverter.GetPhysical(rightRawCurrent),
                                    RawCurrent = rightRawCurrent
                                }
                            };

                            SpeedCurrentFeedbackReceived?.Invoke(this, parsed);
                        }
                        break;

                    case EReceiverCommand.FeedbackVoltageTemperature:
                        {
                            var parsed = new MessageParsedEventArgs
                            {
                                VoltageTemperatureFeedbackModel = new VoltageTemperatureFeedbackModel
                                {
                                    Voltage = VoltageConverter.GetPhysical(BitConverter.ToUInt16(payload, 0)),
                                    Temperature = TemperatureConverter.GetPhysical(BitConverter.ToUInt16(payload, 2)),
                                }
                            };

                            VoltageTemperatureFeedbackReceived?.Invoke(this, parsed);
                        }
                        break;

                    #region Alarms
                    case EReceiverCommand.VoltageAlarm:
                        _parameters.Alarms.VoltageAlarm = VoltageConverter.GetPhysical(BitConverter.ToUInt16(payload, 0));
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.CriticalVoltageAlarm:
                        _parameters.Alarms.CriticalVoltageAlarm = VoltageConverter.GetPhysical(BitConverter.ToUInt16(payload, 0));
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.TemperatureAlarm:
                        _parameters.Alarms.TemperatureAlarm = TemperatureConverter.GetPhysical(BitConverter.ToUInt16(payload, 0));
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.CriticalTemperatureAlarm:
                        _parameters.Alarms.CriticalTemperatureAlarm = TemperatureConverter.GetPhysical(BitConverter.ToUInt16(payload, 0));
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.CurrentLeftAlarm:
                        _parameters.Alarms.LeftCurrentAlarm = CurrentConverter.GetPhysical(BitConverter.ToInt16(payload, 0));
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.CurrentRightAlarm:
                        _parameters.Alarms.RightCurrentAlarm = CurrentConverter.GetPhysical(BitConverter.ToInt16(payload, 0));
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;
                    #endregion

                    #region PID parameters
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
                    #endregion PID parameters

                    #region Fuzzy parameters
                    case EReceiverCommand.FuzzyKp_1:
                        _parameters.FuzzyLeft.Kp = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.FuzzyKi_1:
                        _parameters.FuzzyLeft.Ki = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.FuzzyKd_1:
                        _parameters.FuzzyLeft.Kd = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.FuzzyIntegralLimit_1:
                        _parameters.FuzzyLeft.IntegralLimit = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.FuzzyClamping_1:
                        _parameters.FuzzyLeft.OutputLimit = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.FuzzyDeadband_1:
                        _parameters.FuzzyLeft.Deadband = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.FuzzyPeriod_1:
                        _parameters.FuzzyLeft.Period = BitConverter.ToUInt16(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.FuzzyKp_2:
                        _parameters.FuzzyRight.Kp = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.FuzzyKi_2:
                        _parameters.FuzzyRight.Ki = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.FuzzyKd_2:
                        _parameters.FuzzyRight.Kd = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.FuzzyIntegralLimit_2:
                        _parameters.FuzzyRight.IntegralLimit = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.FuzzyClamping_2:
                        _parameters.FuzzyRight.OutputLimit = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.FuzzyDeadband_2:
                        _parameters.FuzzyRight.Deadband = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.FuzzyPeriod_2:
                        _parameters.FuzzyRight.Period = BitConverter.ToUInt16(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;
                    #endregion Fuzzy parameters

                    #region Encoder parameters
                    case EReceiverCommand.EncoderFilterCoef_1:
                        _parameters.EncoderLeft.LowPassFilterCoeff = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.EncoderFilterCoef_2:
                        _parameters.EncoderRight.LowPassFilterCoeff = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.EncoderScaleCoef_1:
                        _parameters.EncoderLeft.ScaleCoeff = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.EncoderScaleCoef_2:
                        _parameters.EncoderRight.ScaleCoeff = BitConverter.ToSingle(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.EncoderIsReversed_1:
                        _parameters.EncoderLeft.IsReversed = BitConverter.ToBoolean(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.EncoderIsReversed_2:
                        _parameters.EncoderRight.IsReversed = BitConverter.ToBoolean(payload, 0);
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;
                    #endregion Encoder parameters

                    case EReceiverCommand.ControlType:
                        _parameters.ControlType = payload[0];
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.RegenerativeBreaking:
                        _parameters.UseRegenerativeBreaking = payload[0] != 0;
                        ParametersReceived?.Invoke(this, new MessageParsedEventArgs { Parameters = _parameters });
                        break;

                    case EReceiverCommand.EepromSaved:
                        Debug.WriteLine("Saved");
                        break;

                    case EReceiverCommand.TX_MovementEnabled:
                        Debug.WriteLine("Enabled");
                        break;

                    case EReceiverCommand.TX_MovementDisabled:
                        Debug.WriteLine("Disabled");
                        break;


                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                ParsingErrorOccured?.Invoke(this, new MessageParsingErrorEventArgs(e));
            }
        }
    }
}
