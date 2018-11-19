namespace RobotController.Gamepad.Interfaces
{
    public interface ISteeringConfig
    {
        short Deadband { get; set; }
        short Centervalue { get; set; }
        bool IsReversed { get; set; }
        bool IsLeftRightReverse { get; set; }
        bool UseExponentialCurve { get; set; }
        short ExponentialCurveCoefficient { get; set; }
        bool UseLowPassFilter { get; set; }
        int LowPassCoefficient { get; set; }
    }
}