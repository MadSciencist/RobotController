using System;
using NLog;
using RobotController.Communication;
using RobotController.Communication.Messages;
using RobotController.DataLogger.File;
using RobotController.DataLogger.Formatters;
using RobotController.Gamepad.EventArguments;
using RobotController.Gamepad.Interfaces;

namespace RobotController.DataLogger
{
    public class DataLoggerService
    {
        public event EventHandler LoggingStarted;
        public event EventHandler LoggingStopped;

        private RobotConnectionService _robot;
        private IGamepadService _gamepad;
        private readonly ILogConfig _config;
        private readonly IFormatter _formatter;
        private IFileWriter _writer;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public DataLoggerService(ILogConfig config)
        {
            _config = config;
            _formatter = new CsvFormatter();
        }

        public void SubscribeAndStart(RobotConnectionService robot, IGamepadService gamepad)
        {
            _robot = robot;
            _gamepad = gamepad;

            _writer = new FileWriter(_config);
            _writer.WriteLine(_formatter.GetHeader());
            _log = new DatalogModel();

            try
            {
                _gamepad.RobotControlChanged += GamepadSerrvice_RobotControlChanged;
                _robot.SpeedCurrentFeedbackReceived += RobotConnection_SpeedCurrentReceived;
                _robot.VoltageTemperatureFeedbackReceived += RobotConnection_VoltageTemperatureReceived;
                _logger.Info($"Started on path {_config.Path}");
                LoggingStarted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                _logger.Error($"Cannot subscribe: {e.Message}. Please connect first.");
            }
        }

        public void UnSubscribeAndStop()
        {
            try
            {
                _gamepad.RobotControlChanged -= GamepadSerrvice_RobotControlChanged;
                _robot.SpeedCurrentFeedbackReceived -= RobotConnection_SpeedCurrentReceived;
                _robot.VoltageTemperatureFeedbackReceived -= RobotConnection_VoltageTemperatureReceived;
                _logger.Info("Stopped");
                LoggingStopped?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                _logger.Error($"Cannot un-subscribe: {e.Message}. Please connect first.");
            }
        }

        private DatalogModel _log;
        /// <summary>
        /// We are goind to log when speed and current appear
        /// because it is updated with highest frequency
        /// </summary>
        private void RobotConnection_SpeedCurrentReceived(object sender, MessageParsedEventArgs e)
        {
            _log.LeftSpeed = e.LeftMotor.Velocity;
            _log.RawLeftSpeed = e.LeftMotor.RawVelocity;
            _log.RightSpeed = e.RightMotor.Velocity;
            _log.RawRightSpeed = e.RightMotor.RawVelocity;
            _log.LeftCurrent = e.LeftMotor.Current;
            _log.RawLeftCurrent = e.LeftMotor.RawCurrent;
            _log.RightCurrent = e.RightMotor.Current;
            _log.RawRightCurrent = e.RightMotor.RawCurrent;

            Log();
        }

        private void RobotConnection_VoltageTemperatureReceived(object sender, MessageParsedEventArgs e)
        {
            _log.Voltage = e.VoltageTemperatureFeedbackModel.Voltage;
            _log.Temperature = e.VoltageTemperatureFeedbackModel.Temperature;
        }

        private void GamepadSerrvice_RobotControlChanged(object sender, RobotControlEventArgs e)
        {
            _log.LeftSetpoint = e.RobotControl.LeftSpeed;
            _log.RightSetpoint = e.RobotControl.RightSpeed;
        }

        private void Log()
        {
            var line = _formatter.Format(_log);
            _writer.WriteLine(line);
        }
    }
}
