namespace RobotController.Communication.Enums
{
    public enum EReceiverCommand : byte
    {
        KeepAlive = 1,
        FeedbackSpeedCurrent = 10,
        FeedbackVoltageTemperature = 11
    }
}
