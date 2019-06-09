using System;
using System.Threading;
using System.Threading.Tasks;

namespace RobotController.WpfGui.BusinessLogic
{
    public abstract class ExperimentHandler
    {
        public virtual ControlsSender Sender { get; set; }
        public event EventHandler<string> Finished;
        public abstract void Handle();

        protected Task HandleAsync()
        {
            return Task.Factory.StartNew(Action, CancellationToken.None);
        }

        protected abstract void Action();

        protected virtual void OnFinished(string result)
        {
            Finished?.Invoke(this, result);
        }
    }
}
