using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotController.WpfGui.Controls
{
    /// <summary>
    /// Interaction logic for RobotSettingsControllers.xaml
    /// </summary>
    public partial class RobotSettingsControllers : UserControl
    {
        public event EventHandler<KeyEventArgs> TextBoxEnterPressed;
        public event EventHandler<RoutedEventArgs> RadioButtonChecked;
        public event EventHandler<RoutedEventArgs> CheckboxChecked; 

        public RobotSettingsControllers()
        {
            InitializeComponent();
        }

        protected void TextBox_OnEnterPressed(object sender, KeyEventArgs e)
        {
            TextBoxEnterPressed?.Invoke(sender, e);
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
