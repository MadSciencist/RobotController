namespace RobotController.RobotModels.Drive
{
    public class PidModel
    {
        public float Kp { get; set; }
        public float Ki { get; set; }
        public float Kd { get; set; }

        public float IntegralLimit { get; set; }
        public float OutputLimit { get; set; }
        public ushort Period { get; set; }
        public float Deadband { get; set; }
    }
}
