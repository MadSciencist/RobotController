namespace RobotController.Gamepad.Interfaces
{
    public interface ISteeringConfig
    {
        short Deadband { get; set; }
        short Centervalue { get; set; }
        bool IsReversed { get; set; }
        bool IsLeftRightReverse { get; set; }
    }
}