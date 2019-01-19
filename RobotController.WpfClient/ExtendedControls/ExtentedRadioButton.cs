using System.Windows;
using RobotController.Communication.Enums;
using System.Windows.Controls;

namespace RobotController.WpfGui.ExtendedControls
{
    public class ExtentedRadioButton : RadioButton
    {
        public ESenderCommand ESenderCommand { get; set; }
        public EPriority EPriority { get; set; }
        public ENode ENode { get; set; }
        public byte State { get; set; }

        public ExtentedRadioButton()
        {
            this.Style = this.FindResource("MaterialDesignRadioButton") as Style;;
        }
    }
}