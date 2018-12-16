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
        public event EventHandler<SendingTextBoxEventArgs> TextBoxEnterPressedNew;



        public event EventHandler<RoutedEventArgs> RadioButtonChecked;
        public event EventHandler<RoutedEventArgs> CheckboxChanged; 

        public RobotSettings()
        {
            InitializeComponent();
        }

        private void OnTextBoxEnterPressed(object sender, KeyEventArgs e)
        {
            TextBoxEnterPressed?.Invoke(sender, e);
        }
        private void OnTextBoxEnterPressedNew(object sender, SendingTextBoxEventArgs e)
        {
            TextBoxEnterPressedNew?.Invoke(sender, e);
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
