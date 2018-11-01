namespace RobotController.Gamepad.Models
{
    public class GamepadModel
    {
        public GamepadModel()
        {
            LeftThumbstick = new ThumbstickModel();
            RightThumbstick = new ThumbstickModel();
            ActionButtons = new ActionButtonsModel();
            Cross = new CrossModel();
        }

        public ThumbstickModel LeftThumbstick { get; set; }
        public ThumbstickModel RightThumbstick { get; set; }
        public ActionButtonsModel ActionButtons { get; set; }
        public CrossModel Cross { get; set; }
        public short LeftTrigger { get; set; }
        public short RightTrigger { get; set; }
        public bool IsLeftPressed { get; set; }
        public bool IsRightPressed { get; set; }
        public bool IsStartPressed { get; set; }
        public bool IsBackPressed { get; set; }
    }
}
