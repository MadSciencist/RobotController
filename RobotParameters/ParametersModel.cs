using RobotController.RobotModels.Drive;

namespace RobotController.RobotModels
{
    /// <summary>
    /// We keep this class as singleton, to simplify UI notification
    /// New parameters are put here once they are recieved from robot
    /// The UI is using this istance for binding
    /// </summary>
    public class ParametersModel
    {
        private static ParametersModel _parameters; //singleton instance
        private ParametersModel()
        {
            PidLeft = new PidModel();
            PidRight = new PidModel();
        }

        public static ParametersModel GetParameters()
        {
            if (_parameters == null) _parameters = new ParametersModel();
            return _parameters;
        }

        public PidModel PidLeft { get; set; }
        public PidModel PidRight { get; set; }
        public byte ControlType { get; set; }
        public bool UseRegenerativeBreaking { get; set; }
    }
}
