using System;
using System.Windows;
using System.Windows.Controls;

namespace RobotController.WpfGui.Controls
{
    /// <summary>
    /// Interaction logic for Status.xaml
    /// </summary>
    public partial class Status : UserControl
    {
        public Status()
        {
            InitializeComponent();
        }

        public event EventHandler<RoutedEventArgs> ExportClicked;
        private void ExportClick(object sender, RoutedEventArgs e) => ExportClicked?.Invoke(sender, e);

        public event EventHandler<RoutedEventArgs> ImportClicked;
        private void ImportClick(object sender, RoutedEventArgs e) => ImportClicked?.Invoke(sender, e);
    }
}
