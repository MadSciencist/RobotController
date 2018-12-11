using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotController.WpfGui.Controls
{
    /// <summary>
    /// Interaction logic for RobotSettings.xaml
    /// </summary>
    public partial class RobotSettings : UserControl
    {
        public event EventHandler<KeyEventArgs> TextBoxEnterPressed;
        public event EventHandler<RoutedEventArgs> RadioButtonChecked;
        public event EventHandler<RoutedEventArgs> CheckboxChanged; 

        public RobotSettings()
        {
            InitializeComponent();
        }

        private void RobotSettingsControllers_OnTextBoxEnterPressed(object sender, KeyEventArgs e)
        {
            TextBoxEnterPressed?.Invoke(sender, e);
        }

        private void RobotSettingsControllers_OnRadioChecked(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked?.Invoke(sender, e);
        }

        private void RobotSettingsControllers_OnCheckboxChecked(object sender, RoutedEventArgs e)
        {
            CheckboxChanged?.Invoke(sender, e);
        }
    }
}
