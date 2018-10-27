using RobotController.Communication.Configuration;
using RobotController.Communication.Enums;
using RobotController.Communication.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RobotController.Communication.ReceivingTask
{
    public class ReceiverTask : IDisposable, IReceiverTask
    {
        public event EventHandler<DataReceivedEventArgs> DataReceived;
        public event EventHandler<ReceiverErrorEventArgs> ErrorOccurred;

        public EReceiverStatus Status { get; private set; }

        private readonly IStreamResource _streamResource;
        private Task _task;
        private CancellationTokenSource _cancellation;
        private int _numberOfRestoreAttemps = 0;

        public ReceiverTask(IStreamResource streamResource)
        {
            _streamResource = streamResource;
            Status = EReceiverStatus.Waiting;
        }

        public void Start()
        {
            if (Status == EReceiverStatus.Waiting)
            {
                StartReceivingTask(TaskRun);
                Status = EReceiverStatus.Receiving;
            }
            else if (Status != EReceiverStatus.Receiving)
            {
                RestartReceivingTask(TaskRun);
                Status = EReceiverStatus.Receiving;
            }
            else
            {
                throw new InvalidOperationException("Already started");
            }
        }

        public void Stop()
        {
            Status = EReceiverStatus.Stopped;
        }

        public void Cancel()
        {
            _cancellation.Cancel();
        }

        public void Close()
        {
            _cancellation.Cancel();
            _task.Wait();
        }

        private void StartReceivingTask(Action action)
        {
            _cancellation = new CancellationTokenSource();
            _task = new Task(action, _cancellation.Token);
            _task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            _task.Start();

            _numberOfRestoreAttemps = 0;
        }

        private void RestartReceivingTask(Action action)
        {
            _cancellation.Dispose();
            _task.Dispose();

            StartReceivingTask(action);
        }

        private void TaskRun()
        {
            try
            {
                while (Status == EReceiverStatus.Receiving)
                {
                    TryReceiveData();
                    _cancellation.Token.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                Status = EReceiverStatus.Canceled;
            }
        }

        private void TryReceiveData()
        {
            int numBytesRead = 0;
            byte[] data = new byte[Framing.FrameLength];

            if (_streamResource.BytesToRead() == 0)
            {
                Thread.Sleep(Framing.ReceivingTaskSleepTime);
            }
                                                
            while (numBytesRead != Framing.FrameLength)
            {
                numBytesRead += _streamResource.Read(data, numBytesRead, Framing.FrameLength - numBytesRead);
            }

            DataReceived?.Invoke(this, new DataReceivedEventArgs { Data = data, Length = data.Length });
        }

        private void ExceptionHandler(Task task)
        {
            Status = EReceiverStatus.Error;
            ErrorOccurred?.Invoke(this, new ReceiverErrorEventArgs(task.Exception) { NumberOfRestoreAttemps = _numberOfRestoreAttemps });
            ++_numberOfRestoreAttemps;
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Close();
                _cancellation.Dispose();
                _task.Dispose();

                disposedValue = true;
            }
        }

        public void Dispose() => Dispose(true);
    }
}
