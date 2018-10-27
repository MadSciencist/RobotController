namespace RobotController.Communication
{
    public class Connection : BaseConnection, IConnection
    {

        public string[] GetAvailablePorts()
        {
            return base.GetPortNames();
        }

        public void ConnectToPort(string portName)
        {
            base.TryToOpenPort(portName);
        }

        public void Disconnect()
        {
            base.ClosePort();
        }
    }
}
