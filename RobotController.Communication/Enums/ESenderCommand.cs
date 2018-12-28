namespace RobotController.Communication.Enums
{
    public enum ESenderCommand : byte
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
        PidPeriod = 106,

        VoltageAlarm = 130,
        CriticalVoltageAlarm = 131,
        TemperatureAlarm = 132,
        CriticalTemperatureAlarm = 133,
        CurrentLeftAlarm = 134,
        CurrentRightAlarm = 135,

        EncoderLeftFilterCoef = 140,
        EncoderRightFilterCoef = 141,
        EncoderLeftIsReversed = 142,
        EncoderRightIsReversed = 143,
        EncoderLeftScaleCoef = 144,
        EncoderRightScaleCoef = 145,

        Hello = 255
    }
}
