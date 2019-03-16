using RobotController.RobotModels.Drive;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace RobotController.RobotModels
{
    /// <summary>
    /// We keep this class as singleton, to simplify UI notification
    /// New parameters are put here once they are recieved from robot
    /// The UI is using this istance for binding
    /// </summary>
    [Serializable]
    public class ParametersModel
    {
        private static ParametersModel _parameters; //singleton instance
        private static readonly object _threadLock = new object();

        private ParametersModel()
        {
            PidLeft = new PidModel();
            PidRight = new PidModel();
            FuzzyLeft = new FuzzyModel();
            FuzzyRight = new FuzzyModel();
            EncoderLeft = new EncoderModel();
            EncoderRight = new EncoderModel();
            Alarms = new AlarmModel();
        }

        public static ParametersModel GetParameters()
        {
            lock (_threadLock)
            {
                return _parameters ?? (_parameters = new ParametersModel());
            }
        }

        public PidModel PidLeft { get; set; }
        public PidModel PidRight { get; set; }
        public FuzzyModel FuzzyLeft { get; set; }
        public FuzzyModel FuzzyRight { get; set; }
        public EncoderModel EncoderLeft { get; set; }
        public EncoderModel EncoderRight { get; set; }
        public AlarmModel Alarms { get; set; }
        public byte ControlType { get; set; }
        public bool UseRegenerativeBreaking { get; set; }
    }
}
