namespace RobotController.RobotModels
{
    public class ControlsModel
    {
        public ControlsModel(short left, short right)
        {
            LeftSpeed = left;
            RightSpeed = right;
        }

        public short LeftSpeed { get; set; }
        public short RightSpeed { get; set; }
    }
}
