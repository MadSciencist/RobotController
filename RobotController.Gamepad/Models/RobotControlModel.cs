namespace RobotController.Gamepad.Models
{
    public struct RobotControlModel
    {
        public RobotControlModel(short left, short right) : this()
        {
            LeftSpeed = left;
            RightSpeed = right;
        }
        public short LeftSpeed { get; set; }
        public short RightSpeed { get; set; }
    }
}
