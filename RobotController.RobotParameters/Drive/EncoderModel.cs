namespace RobotController.RobotModels.Drive
{
    public class EncoderModel
    {
        public bool IsReversed { get; set; }
        public double LowPassFilterCoeff { get; set; }
        public double ScaleCoeff { get; set; }
    }
}
