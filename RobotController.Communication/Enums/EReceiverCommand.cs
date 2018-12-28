namespace RobotController.Communication.Enums
{
    public enum EReceiverCommand : byte
    {
        KeepAlive = 1,
        FeedbackSpeedCurrent = 10,
        FeedbackVoltageTemperature = 11,
        ControlType = 12,
        RegenerativeBreaking = 13,

        VoltageAlarm = 20,
        CriticalVoltageAlarm = 21,
        TemperatureAlarm = 22,
        CriticalTemperatureAlarm = 23,
        CurrentLeftAlarm = 24,
        CurrentRightAlarm = 25,

        EepromSaved = 30,

        PidKp_1 = 100,
        PidKi_1 = 101,
        PidKd_1 = 102,
        PidIntegralLimit_1 = 103,
        PidClamping_1 = 104,
        PidDeadband_1 = 105,
        PidPeriod_1 = 106,

        EncoderFilterCoef_1 = 130,
        EncoderIsReversed_1 = 131,
        EncoderScaleCoef_1 = 132,

        PidKp_2 = 200,
        PidKi_2 = 201,
        PidKd_2 = 202,
        PidIntegralLimit_2 = 203,
        PidClamping_2 = 204,
        PidDeadband_2 = 205,
        PidPeriod_2 = 206,

        EncoderFilterCoef_2 = 230,
        EncoderIsReversed_2 = 231,
        EncoderScaleCoef_2 = 232
    }
}
