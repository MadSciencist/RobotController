using System;
using System.Windows;
using System.Windows.Controls;

namespace RobotController.WpfGui.Controls
{
    /// <summary>
    /// Interaction logic for ControlSettings.xaml
    /// </summary>
    public partial class ControlSettings : UserControl
    {
        public event EventHandler<short> ExpoSliderChanged;
        public event EventHandler<short> LowPassFilterSliderChanged;

        public ControlSettings()
        {
            InitializeComponent();
        }

        protected void OnExpoSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ExpoSliderChanged?.Invoke(this, (short)e.NewValue);
        }

        protected void OnFilterSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LowPassFilterSliderChanged?.Invoke(this, (short)e.NewValue);
        }
    }
}
