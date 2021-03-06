﻿using System.Windows;
using System.Windows.Controls;
using RobotController.Communication.Enums;

namespace RobotController.WpfGui.ExtendedControls
{
    public class ExtendedButton : Button
    {
        public ESenderCommand ECommand { get; set; }
        public EPriority EPriority { get; set; }
        public ENode ENode { get; set; }

        public ExtendedButton()
        {
            this.Style = this.FindResource("MaterialDesignRaisedDarkButton") as Style;
        }
    }
}
