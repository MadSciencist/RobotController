using NLog;
using RobotController.Communication.Interfaces;
using RobotController.Communication.Messages;
using RobotController.Communication.ReceivingTask;
using System;
using System.Runtime.Remoting.Channels;
using RobotController.Communication.Utils;


namespace RobotController.Communication
{
    public class RobotConnectionFacade : IDisposable
    {
        public event EventHandler<MessageParsedEventArgs> FeedbackReceived;

        private readonly IStreamResource _streamResource;
        private readonly IReceiverTask _receiverTask;
        private readonly MessageExtractor _messageExtractor;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IWatchdog _watchdog;

        public RobotConnectionFacade(IStreamResource streamResource)
        {
            _streamResource = streamResource;

            _messageExtractor = new MessageExtractor();
            _messageExtractor.KeepAliveReceived += (sender, args) => _watchdog.ResetWatchdog();
            _messageExtractor.MessageLostOccured +=
                (sender, args) => _logger.Fatal($"Lost message, total count: {args.TotalLostCount}");
            _messageExtractor.FeedbackReceived += (sender, args) => FeedbackReceived?.Invoke(sender, args);

            _receiverTask = new ReceiverTask(_streamResource);
            _receiverTask.DataReceived += (sender, args) => _messageExtractor.TryGetMessage(args.Data);
            _receiverTask.Start();
            _logger.Info("Starting receiver task");

            _watchdog = new Watchdog(250);
            _watchdog.TimeoutOccured += (sender, args) => _logger.Fatal("Communication timeout");
            _watchdog.Start();
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
                _watchdog.Stop();
                _receiverTask?.Stop();
            }
        }
    }
}
