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

        public RobotSettings()
        {
            InitializeComponent();
        }

        private void OnTextBoxEnterPressed(object sender, KeyEventArgs e)
        {
            TextBoxEnterPressed?.Invoke(sender, e);
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked?.Invoke(sender, e);
        }
    }
}
