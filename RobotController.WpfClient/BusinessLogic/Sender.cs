using RobotController.Communication;
using RobotController.Communication.Enums;
using RobotController.Communication.Interfaces;

namespace RobotController.WpfGui.BusinessLogic
{
    public class Sender
    {

        private readonly RobotConnectionService _robotConnectionService;
        public Sender(RobotConnectionService robotConnectionService)
        {
            _robotConnectionService = robotConnectionService;
        }

        public void SendMessage(ISendMessage message, EPriority priority)
        {
            _robotConnectionService?.SendCommand(message, priority);
        }
    }
}
