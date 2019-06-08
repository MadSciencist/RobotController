using RobotController.Communication;
using RobotController.Communication.Enums;
using RobotController.Communication.Messages;
using RobotController.RobotModels;
using System.Timers;

namespace RobotController.WpfGui.BusinessLogic
{
    public class ControlsSender : Sender
    {
        private ControlsModel _controls;
        private readonly Timer _timer;

        public ControlsSender(RobotConnectionService robotConnectionService, int interval) : base(robotConnectionService)
        {
            _controls = new ControlsModel(0, 0);
            _timer = new Timer(interval);   
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
        }

        public void UpdateAndSendControls(ControlsModel controls) => _controls = controls;

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            var message = new SendMessage
            {
                CommandType = ESenderCommand.Controls,
                Node = ENode.Master,
                Payload = _controls
            };

            base.SendMessage(message, EPriority.Low);
        }
    }
}
