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
        public event EventHandler<short> FilterSliderChanged;

        public ControlSettings()
        {
            InitializeComponent();
        }

        protected void OnFilterSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FilterSliderChanged?.Invoke(this, (short)e.NewValue);
        }
    }
}
