namespace HardwareEmulator.Enums
{
    public enum EReceiver : byte
    {
        KeepAlive = 0,
        Controls = 1,
        EepromRead = 5,
        EepromWrite = 6,
        AllowMovement = 10,
        StopMovement = 11,
        ControlType = 12,
        RegenerativeBreaking = 13,

        PidKp = 100,
        PidKi = 101,
        PidKd = 102,
        PidIntegralLimit = 103,
        PidClamping = 104,
        PidDeadband = 105,
        PidPeriod = 106
    }
}
