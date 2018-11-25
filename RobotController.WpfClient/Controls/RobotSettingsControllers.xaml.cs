using System;
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

        public RobotSettingsControllers()
        {
            InitializeComponent();
        }

        protected void OnTextBoxEnterPressed(object sender, KeyEventArgs e)
        {
            TextBoxEnterPressed?.Invoke(sender, e);
        }
    }
}
