using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HardwareEmulatorGui
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            Log("Connecting...");
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            Log("Stopped!");
        }

        private void Log(string messege)
        {
            var nl = Environment.NewLine;
            var line = $"{DateTime.Now}: {messege} {nl}";
            AppendLineToLog(line);
        }
        private void AppendLineToLog(string line)
        {
            var run = new Run { Text = line };
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(run);
            RichTextBlock.Blocks.Add(paragraph);
        }

        private void ComboBox_OnDropDownOpened(object sender, object e)
        {
            

        }

        private void LoadPortNames()
        {
            var ports = System.IO.Ports;
            if (ports.Length <= 0) return;
            var observable = new ObservableCollection<string>(ports);
            PortNameComboBox.ItemsSource = observable;
            PortNameComboBox.PlaceholderText = observable[0];
        }
    }
}
