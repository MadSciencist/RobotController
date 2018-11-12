using NLog;
using RobotController.Communication.Interfaces;
using RobotController.Communication.Messages;
using RobotController.Communication.ReceivingTask;
using RobotController.Communication.SendingTask;
using RobotController.Communication.Utils;
using System;
using RobotController.Communication.Enums;


namespace RobotController.Communication
{
    public class RobotConnectionFacade : IDisposable
    {
        public event EventHandler<MessageParsedEventArgs> FeedbackReceived;

        private readonly IStreamResource _streamResource;
        private readonly IReceiverTask _receiverTask;
        private readonly ISenderTask _senderTask;
        private readonly MessageExtractor _messageExtractor;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IWatchdog _watchdog;
        private readonly ISendQueueWrapper _senderQueue;

        public RobotConnectionFacade(IStreamResource streamResource)
        {
            _streamResource = streamResource;
            _watchdog = new Watchdog(250);
            _watchdog.TimeoutOccured += (sender, args) => _logger.Fatal("Communication timeout");
            

            _messageExtractor = new MessageExtractor();
            _messageExtractor.KeepAliveReceived += (sender, args) => _watchdog.ResetWatchdog();
            _messageExtractor.MessageLostOccured +=
                (sender, args) => _logger.Fatal($"Lost message, total count: {args.TotalLostCount}");
            _messageExtractor.FeedbackReceived += (sender, args) => FeedbackReceived?.Invoke(sender, args);

            _receiverTask = new ReceiverTask(_streamResource);
            _receiverTask.ErrorOccurred += (sender, args) => _logger.Error($"Receiver task error: {args.GetException().Message}");
            _receiverTask.DataReceived += (sender, args) => _messageExtractor.TryGetMessage(args.Data);
            _receiverTask.Start();

            _senderQueue = new SendQueueWrapper();
            _senderTask = new SenderTask(_streamResource, _senderQueue);
            _senderTask.ErrorOccurred += (sender, args) => _logger.Error($"Sender task error: {args.GetException().Message}");
            _senderTask.Start();

            _watchdog.Start();
        }

        public void SendCommand(ISendMessage command, EPriority priority)
        {
            _senderQueue.Enqueue(command, priority);
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
                _watchdog?.Stop();
                _receiverTask?.Stop();
                _senderTask?.Stop();
            }
        }
    }
}
