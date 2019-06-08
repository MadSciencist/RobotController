using System;

namespace RobotController.WpfGui.BusinessLogic
{
    public abstract class ExperimentHandler
    {
        public virtual ControlsSender Sender { get; set; }
        public event EventHandler<string> Finished;
        public abstract void Handle();

        protected virtual void OnFinished(string result)
        {
            Finished?.Invoke(this, result);
        }
    }
}
