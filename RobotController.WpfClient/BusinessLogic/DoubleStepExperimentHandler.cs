using NLog;
using RobotController.RobotModels;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RobotController.WpfGui.BusinessLogic
{
    public class DoubleStepExperimentHandler : ExperimentHandler
    {
        public DoubleStepExperimentParams Params { get; set; }
        public event EventHandler<ControlsModel> ComputedNewControls;

        private static Logger Logger;
        private string _result;

        public DoubleStepExperimentHandler()
        {
            _result = string.Empty;
            Logger = LogManager.GetCurrentClassLogger();
        }

        public override async void Handle()
        {
            if (!IsValid()) return;
            Logger.Info("Experiment started");
            await HandleAsync();
            Logger.Info("Experiment performed succesffully");
            base.OnFinished(_result);
        }

        protected override void Action()
        {
            try
            {
                var controls = new ControlsModel(Params.FirstStepVelocity, Params.FirstStepVelocity);
                ComputedNewControls?.Invoke(this, controls);
                Sender.UpdateAndSendControls(controls);
                Thread.Sleep(Params.FirstStepLength);

                var controls2 = new ControlsModel(Params.SecondStepVelocity, Params.SecondStepVelocity);
                ComputedNewControls?.Invoke(this, controls2);
                Sender.UpdateAndSendControls(controls2);
                Thread.Sleep(Params.SecondStepLength);

                var controls3 = new ControlsModel(0, 0);
                ComputedNewControls?.Invoke(this, controls3);
                Sender.UpdateAndSendControls(controls3);
                _result = "success";
            }
            catch (Exception ex)
            {
                Logger.Error("Error while executing experiment");
                Logger.Error(ex);
                _result = "error";
            }
        }

        private bool IsValid()
        {
            try
            {
                if (Sender == null)
                    throw new ArgumentNullException(nameof(Sender));
                if (Params == null)
                    throw new ArgumentNullException(nameof(Params));
                if (Params.FirstStepVelocity == 0)
                    throw new ArgumentException(nameof(Params.FirstStepVelocity));
                if (Params.FirstStepLength == TimeSpan.Zero)
                    throw new ArgumentException(nameof(Params.FirstStepLength));
                if (Params.SecondStepLength == TimeSpan.Zero)
                    throw new ArgumentException(nameof(Params.SecondStepLength));

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Experiment validation error:");
                Logger.Error(ex);
                //throw;
                return false;
            }
        }
    }
}
