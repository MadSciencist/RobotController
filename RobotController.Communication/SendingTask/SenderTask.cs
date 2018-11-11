using NLog;
using RobotController.Communication.Interfaces;
using RobotController.Communication.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;
using RobotController.Communication.Configuration;

namespace RobotController.Communication.SendingTask
{
    internal class SenderTask : ISenderTask
    {
        public event EventHandler<SenderErrorEventArgs> ErrorOccurred;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IStreamResource _streamResource;
        private readonly IQueueWrapper _queue;
        private readonly MessageGenerator _messageGenerator;
        private Task _task;
        private CancellationTokenSource _source;

        public SenderTask(IStreamResource streamResource, IQueueWrapper queue)
        {
            _messageGenerator = new MessageGenerator();
            _streamResource = streamResource;
            _queue = queue;
        }

        public void Start()
        {
            _source = new CancellationTokenSource();
            _task = Task.Factory.StartNew(TaskRun, _source.Token)
                .ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);

            _logger.Info("Sender started");
        }

        public void Stop()
        {
            _logger.Info("Sender stop request");
            _source.Cancel();
        }

        private void TaskRun()
        {
            try
            {
                while (true)
                {
                    if (_source.IsCancellationRequested)
                    {
                        _source.Token.ThrowIfCancellationRequested();
                    }

                    TrySendData();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.Info("Sender task cancellation request");
            }
        }

        private void TrySendData()
        {
            try
            {
                if(_queue.Count() == 0)
                {
                    Thread.Sleep(CommunicationTasks.TrasmitingTaskSleepTime);
                }
                else
                {
                    var message = _queue.Dequeue();
                    var packet = _messageGenerator.Generate(message);
                    _streamResource.Write(packet, 0, packet.Length);

                }
            }
            catch (Exception e)
            {
                _logger.Error("Sender task exception: " + e.Message);
                Stop(); // probably the serial port is not opened or not created, there is no reason to continue this task
            }

        }

        private void ExceptionHandler(Task task)
        {
            _logger.Error("Receiver task exception: " + task.Exception?.Message);
            ErrorOccurred?.Invoke(this, new SenderErrorEventArgs(task.Exception));
        }
    }
}
