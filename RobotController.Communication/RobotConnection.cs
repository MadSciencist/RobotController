using RobotController.Communication.ReceivingTask;
using System;

namespace RobotController.Communication
{
    public class RobotConnection : IDisposable
    {
        private IStreamResource _streamResource;
        private ReceiverTask _receiverTask;

        public RobotConnection(IStreamResource streamResource)
        {
            _streamResource = streamResource;
            _receiverTask = new ReceiverTask(streamResource);

            _receiverTask.Start();
        }

        public void StopConnection() => _receiverTask.Close();

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _streamResource.Dispose();
                _receiverTask.Dispose();

                disposedValue = true;
            }
        }

        public void Dispose() => Dispose(true);
    }
}
