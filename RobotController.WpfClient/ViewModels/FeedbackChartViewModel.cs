using RobotController.WpfGui.Charts;

namespace RobotController.WpfGui.ViewModels
{
    public class FeedbackChartViewModel : ObservableEntity
    {
        private SpeedFeedbackChart _feedbackChart;

        public SpeedFeedbackChart SpeedFeedbackChart
        {
            get { return _feedbackChart; }
            set
            {
                _feedbackChart = value;
                OnPropertyChanged(nameof(SpeedFeedbackChart));
            }
        }
    }
}
