namespace RobotController.Communication.Enums
{
    public enum ESenderCommand : byte
    {
        KeepAlive = 0,
        Controls = 1,
        EepromRead = 5,
        EepromWrite = 6,
        AllowMovement = 10,
        StopMovement = 11
    }
}
