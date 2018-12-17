using RobotController.WpfGui.BusinessLogic;
using System;
using System.Windows;
using System.Windows.Controls;

namespace RobotController.WpfGui.Controls
{
    /// <summary>
    /// Interaction logic for RobotSettings.xaml
    /// </summary>
    public partial class RobotSettings : UserControl
    {
        public event EventHandler<SendingTextBoxEventArgs> TextBoxEnterPressed;
        public event EventHandler<RoutedEventArgs> RadioButtonChecked;
        public event EventHandler<RoutedEventArgs> CheckboxChanged; 

        public RobotSettings()
        {
            InitializeComponent();
        }

        private void OnTextBoxEnterPressed(object sender, SendingTextBoxEventArgs e)
        {
            TextBoxEnterPressed?.Invoke(sender, e);
        }

        private void OnRadioChecked(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked?.Invoke(sender, e);
        }

        private void OnCheckboxChecked(object sender, RoutedEventArgs e)
        {
            CheckboxChanged?.Invoke(sender, e);
        }
    }
}
