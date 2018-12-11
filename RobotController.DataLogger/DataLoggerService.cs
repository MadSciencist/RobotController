using RobotController.Communication;
using RobotController.Communication.Messages;
using RobotController.DataLogger.Formatters;
using RobotController.Gamepad.EventArguments;
using RobotController.Gamepad.Interfaces;

namespace RobotController.DataLogger
{
    public class DataLoggerService
    {
        private readonly ILogConfig _config;
        private readonly IFormatter _formatter;
        private readonly DatalogModel _log;

        public DataLoggerService(RobotConnectionService robot, IGamepadService gamepad, ILogConfig config)
        {
            _config = config;
            _formatter = new CsvFormatter();

            gamepad.RobotControlChanged += GamepadSerrvice_RobotControlChanged;
            robot.SpeedCurrentFeedbackReceived += RobotConnection_SpeedCurrentReceived;
            robot.VoltageTemperatureFeedbackReceived += RobotConnection_VoltageTemperatureReceived;
        }

        /// <summary>
        /// We are goind to log when speed and current appear
        /// because it is updated with highest frequency
        /// </summary>
        private void RobotConnection_SpeedCurrentReceived(object sender, MessageParsedEventArgs e)
        {
            _log.LeftSpeed = e.LeftMotor.RawSpeed;
            _log.RightSpeed = e.RightMotor.RawSpeed;
            _log.LeftCurrent = e.LeftMotor.RawCurrent;
            _log.RightCurrent = e.RightMotor.RawCurrent;

            Log();
        }

        private void RobotConnection_VoltageTemperatureReceived(object sender, MessageParsedEventArgs e)
        {
            _log.Voltage = e.VoltageTemperatureFeedbackModel.RawVoltage;
            _log.Temperature = e.VoltageTemperatureFeedbackModel.RawTemperature;
        }

        private void GamepadSerrvice_RobotControlChanged(object sender, RobotControlEventArgs e)
        {
            _log.LeftSetpoint = e.RobotControl.LeftSpeed;
            _log.RightSetpoint = e.RobotControl.RightSpeed;
        }

        private void Log()
        {
            var line = _formatter.Format(_log);
        }
    }
}
