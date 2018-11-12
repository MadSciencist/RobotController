namespace RobotController.Communication.Messages
{
    public abstract class Message
    {
        public abstract object Payload { get; set; }
    }
}
