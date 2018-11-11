using NLog;
using RobotController.Communication.Configuration;
using RobotController.Communication.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RobotController.Communication.ReceivingTask
{
    public class ReceiverTask : IReceiverTask
    {
        public event EventHandler<RobotDataReceivedEventArgs> DataReceived;
        public event EventHandler<ReceiverErrorEventArgs> ErrorOccurred;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IStreamResource _streamResource;
        private Task _task;
        private CancellationTokenSource _source;

        public ReceiverTask(IStreamResource streamResource)
        {
            _streamResource = streamResource;
        }

        public void Start()
        {
            _source = new CancellationTokenSource();
            _task = Task.Factory.StartNew(TaskRun, _source.Token)
                .ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);

            _logger.Info("Receiver started");
        }

        public void Stop()
        {
            _logger.Info("Receiver stop request");
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

                    TryReceiveData();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.Info("Receiver task cancellation request");
            }
        }

        private void TryReceiveData()
        {
            var numBytesRead = 0;
            var data = new byte[Framing.FrameLength];

            try
            {
                if (_streamResource.BytesToRead() == 0)
                {
                    Thread.Sleep(10);
                }

                while (numBytesRead != Framing.FrameLength)
                {
                    numBytesRead += _streamResource.Read(data, numBytesRead, Framing.FrameLength - numBytesRead);
                }
            }
            catch (Exception e)
            {
                _logger.Error("Receiver task exception: " + e.Message);
                Stop(); // probably the serial port is not opened or not created, there is no reason to continue this task
            }


            DataReceived?.Invoke(this, new RobotDataReceivedEventArgs { Data = data, Length = data.Length });
        }

        private void ExceptionHandler(Task task)
        {
            _logger.Error("Receiver task exception: " + task.Exception?.Message);
            ErrorOccurred?.Invoke(this, new ReceiverErrorEventArgs(task.Exception));
        }
    }
}
