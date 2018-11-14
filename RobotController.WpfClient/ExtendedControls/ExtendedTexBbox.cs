﻿using RobotController.Communication.Enums;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotController.WpfGui.ExtendedControls
{
    public class ExtendedTexBbox : TextBox
    {
        public event EventHandler<KeyEventArgs> OnEnterPressed;
        public ESenderCommand ECommand { get; set; }
        public EPriority EPriority { get; set; }
        public ENode ENode { get; set; }
        public ESendingType EType { get; set; }

        public ExtendedTexBbox()
        {
            base.KeyUp += OnKeyUp;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnEnterPressed?.Invoke(this, e);
            }
        }
    }
}
