using RobotController.Communication.Configuration;
using RobotController.Communication.Interfaces;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RobotController.Communication.ReceivingTask
{
    public class ReceiverTask : IReceiverTask
    {
        public event EventHandler<RobotDataReceivedEventArgs> DataReceived;
        public event EventHandler<ReceiverErrorEventArgs> ErrorOccurred;

        private readonly IStreamResource _streamResource;
        private Task _task;
        private CancellationTokenSource _cancellation;
        private int _numberOfRestoreAttemps = 0;

        public ReceiverTask(IStreamResource streamResource)
        {
            _streamResource = streamResource;
        }

        public void Start()
        {
            StartReceivingTask(TaskRun);
            Debug.WriteLine("SRECEIVER starting...");
        }

        public void Stop()
        {
            Debug.WriteLine("RECEIVER Closing...");
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

        private void TaskRun()
        {
            try
            {
                while (true)
                {
                    TryReceiveData();
                    _cancellation.Token.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void TryReceiveData()
        {
            int numBytesRead = 0;
            byte[] data = new byte[Framing.FrameLength];

            if (_streamResource.BytesToRead() == 0)
            {
                Thread.Sleep(10);
            }

            while (numBytesRead != Framing.FrameLength)
            {
                numBytesRead += _streamResource.Read(data, numBytesRead, Framing.FrameLength - numBytesRead);
            }

            DataReceived?.Invoke(this, new RobotDataReceivedEventArgs { Data = data, Length = data.Length });
        }

        private void ExceptionHandler(Task task)
        {
            ErrorOccurred?.Invoke(this, new ReceiverErrorEventArgs(task.Exception) { NumberOfRestoreAttemps = _numberOfRestoreAttemps });
        }

        public void Dispose()
        {
            Debug.WriteLine("RECEIVER Dispose method public...");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Debug.WriteLine("RECEIVER Dispose method virtual...");
            if (disposing)
            {
                Debug.WriteLine("RECEIVER Disposing...");
                _cancellation.Cancel();
                _task.Wait();

                _cancellation.Dispose();
                _task.Dispose();
            }
        }
    }
}
