namespace RobotController.RobotModels
{
    public class SpeedCurrentFeedbackModel
    {
        public double Velocity { get; set; }
        public short RawVelocity { get; set; }
        public double Current { get; set; }
        public short RawCurrent { get; set; }
    }
}
