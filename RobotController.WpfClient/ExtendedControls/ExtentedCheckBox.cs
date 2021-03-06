﻿using System.Windows;
using RobotController.Communication.Enums;
using System.Windows.Controls;

namespace RobotController.WpfGui.ExtendedControls
{
    public class ExtentedCheckBox : CheckBox
    {
        public ESenderCommand ECommand { get; set; }
        public EPriority EPriority { get; set; }
        public ENode ENode { get; set; }

        public ExtentedCheckBox()
        {
            this.Style = this.FindResource("MaterialDesignCheckBox") as Style;
        }
    }
}
