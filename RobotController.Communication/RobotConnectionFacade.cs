using RobotController.Communication.Interfaces;
using RobotController.Communication.Messages;
using RobotController.Communication.ReceivingTask;
using System;
using System.Diagnostics;

namespace RobotController.Communication
{
    public class RobotConnectionFacade
    {
        private readonly IStreamResource _streamResource;
        private IReceiverTask _receiverTask;
        private readonly MessageExtractor _messageParser;

        public RobotConnectionFacade(IStreamResource streamResource)
        {
            _messageParser = new MessageExtractor();
            _streamResource = streamResource;
            _receiverTask = new ReceiverTask(_streamResource);
            _receiverTask.DataReceived += DataReceived;
            _receiverTask.Start();
        }

        private void DataReceived(object sender, RobotDataReceivedEventArgs args)
        {
            _messageParser.TryGetMessage(args.Data, args.Length);
        }

        public void Dispose()
        {
            Debug.WriteLine("ROBOT CONN Dispose method public...");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Debug.WriteLine("ROBOT CONN Dispose method virtual...");
            if (disposing)
            {
                Debug.WriteLine("ROBOT CONN Disposing...");
                if (_receiverTask != null)
                {
                    _receiverTask.Stop();
                }
            }
        }
    }
}
