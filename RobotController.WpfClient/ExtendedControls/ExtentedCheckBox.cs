using RobotController.Communication.Enums;
using System.Windows.Controls;

namespace RobotController.WpfGui.ExtendedControls
{
    public class ExtentedCheckBox : CheckBox
    {
        public ESenderCommand ESenderCommand { get; set; }
        public EPriority EPriority { get; set; }
        public ENode ENode { get; set; }
    }
}
