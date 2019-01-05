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
    public class RobotConnectionService : IDisposable
    {
        public EventHandler<MessageParsedEventArgs> SpeedCurrentFeedbackReceived;
        public EventHandler<MessageParsedEventArgs> VoltageTemperatureFeedbackReceived;
        public EventHandler<MessageParsedEventArgs> ParametersReceived;
        public event EventHandler<EventArgs> TimeoutOccured; 

        private readonly IStreamResource _streamResource;
        private readonly IReceiverTask _receiverTask;
        private readonly ISenderTask _senderTask;
        private readonly IWatchdog _watchdog;
        private readonly ISendQueueWrapper _senderQueue;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public RobotConnectionService(IStreamResource streamResource)
        {
            _streamResource = streamResource;
            _watchdog = new Watchdog(1100); //this should be minimum 2x keepAlive sending period
            _watchdog.TimeoutOccured += (sender, args) => TimeoutOccured?.Invoke(sender, args);
            

            //TODO unifiy the error event args, so we can use one common event handler
            var messageExtractor = new MessageExtractor();
            messageExtractor.KeepAliveReceived += (sender, args) => _watchdog.ResetWatchdog();
            messageExtractor.MessageLostOccured +=
                (sender, args) => Logger.Fatal($"Lost message, total count: {args.TotalLostCount}");
            messageExtractor.SpeedCurrentFeedbackReceived += (sender, args) => SpeedCurrentFeedbackReceived?.Invoke(sender, args);
            messageExtractor.VoltageTemperatureFeedbackReceived += (sender, args) => VoltageTemperatureFeedbackReceived?.Invoke(sender, args);
            messageExtractor.ParametersReceived += (sender, args) => ParametersReceived?.Invoke(sender, args);

            _receiverTask = new ReceiverTask(_streamResource);
            _receiverTask.ErrorOccurred += (sender, args) => Logger.Error($"Receiver task error: {args.GetException().Message}");
            _receiverTask.DataReceived += (sender, args) => messageExtractor.TryGetMessage(args.Data);
            _receiverTask.Start();

            _senderQueue = new SendQueueWrapper();
            _senderTask = new SenderTask(_streamResource, _senderQueue);
            _senderTask.ErrorOccurred += (sender, args) => Logger.Error($"Sender task error: {args.GetException().Message}");
            _senderTask.Start();

            _watchdog.Start();
        }

        public void SendCommand(ISendMessage command, EPriority priority)
        {
            _senderQueue.Enqueue(command, priority);
        }

        public void Dispose()
        {
            Logger.Info("Disposing robot connection...");
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
