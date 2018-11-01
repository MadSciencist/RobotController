using RobotController.Gamepad.Interfaces;

namespace RobotController.Gamepad.Config
{
    public class SteeringConfig : ISteeringConfig
    {
        public short Deadband { get; set; } = 10;
        public short Centervalue { get; set; } = 255;
        public bool IsReversed { get; set; } = false;
        public bool IsLeftRightReverse { get; set; } = false;
    }
}
