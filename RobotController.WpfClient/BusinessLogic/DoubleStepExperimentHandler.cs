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

        private Task HandleAsync()
        {
            return Task.Factory.StartNew(Action, CancellationToken.None);
        }

        private void Action()
        {
            try
            {
                Sender.UpdateAndSendControls(new ControlsModel(Params.FirstStepVelocity, Params.FirstStepVelocity));
                Thread.Sleep(Params.FirstStepLength);
                Sender.UpdateAndSendControls(new ControlsModel(Params.SecondStepVelocity, Params.SecondStepVelocity));
                Thread.Sleep(Params.SecondStepLength);
                Sender.UpdateAndSendControls(new ControlsModel(0, 0));
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
