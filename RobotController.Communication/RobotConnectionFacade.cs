using NLog;
using RobotController.Communication.Interfaces;
using RobotController.Communication.Messages;
using RobotController.Communication.ReceivingTask;
using System;

namespace RobotController.Communication
{
    public class RobotConnectionFacade : IDisposable
    {
        public event EventHandler<MessageParsedEventArgs> FeedbackReceived;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IStreamResource _streamResource;
        private IReceiverTask _receiverTask;
        private readonly MessageExtractor _messageExtractor;

        public RobotConnectionFacade(IStreamResource streamResource)
        {
            _streamResource = streamResource;

            _messageExtractor = new MessageExtractor();
            _messageExtractor.FeedbackReceived += (sender, args) => FeedbackReceived?.Invoke(sender, args);
            
            _receiverTask = new ReceiverTask(_streamResource);
            _receiverTask.DataReceived += DataReceived;
            _receiverTask.Start();
            _logger.Info("Starting receiver task");
        }

        private void DataReceived(object sender, RobotDataReceivedEventArgs args)
        {
            _messageExtractor.TryGetMessage(args.Data, args.Length);
        }

        public void Dispose()
        {
            _logger.Info("Disposing robot connection...");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _receiverTask?.Stop();
            }
        }
    }
}
