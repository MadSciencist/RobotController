namespace RobotController.Gamepad.Models
{
    public class RobotControlModel
    {
        public RobotControlModel(short left, short right)
        {
            LeftSpeed = left;
            RightSpeed = right;
        }
        public short LeftSpeed { get; set; }
        public short RightSpeed { get; set; }
    }
}
