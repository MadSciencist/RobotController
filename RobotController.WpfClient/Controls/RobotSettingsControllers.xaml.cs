using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RobotController.WpfGui.BusinessLogic;
using RobotController.WpfGui.ExtendedControls;

namespace RobotController.WpfGui.Controls
{
    /// <summary>
    /// Interaction logic for RobotSettingsControllers.xaml
    /// </summary>
    public partial class RobotSettingsControllers : UserControl
    {
        public event EventHandler<SendingTextBoxEventArgs> TextBoxEnterPressed;
        public event EventHandler<RoutedEventArgs> RadioButtonChecked;
        public event EventHandler<RoutedEventArgs> CheckboxChecked; 

        public RobotSettingsControllers()
        {
            InitializeComponent();
        }

        protected void TextBox_OnEnterPressed(object sender, KeyEventArgs e)
        {
            var textBox = (ExtendedTexBbox)sender;
            TextBoxEnterPressed?.Invoke(sender, new SendingTextBoxEventArgs {Value = textBox.Text});
        }

        private void RadioButton_OnChecked(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked?.Invoke(sender, e);
        }

        private void CheckBox_OnCheckedChanged(object sender, RoutedEventArgs e)
        {
            CheckboxChecked?.Invoke(sender, e);
        }
    }
}
