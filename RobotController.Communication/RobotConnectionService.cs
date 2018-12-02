﻿using NLog;
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
        public event EventHandler<EventArgs> TimeoutOccured; 

        private readonly IStreamResource _streamResource;
        private readonly IReceiverTask _receiverTask;
        private readonly ISenderTask _senderTask;
        private readonly MessageExtractor _messageExtractor;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IWatchdog _watchdog;
        private readonly ISendQueueWrapper _senderQueue;

        public RobotConnectionService(IStreamResource streamResource)
        {
            _streamResource = streamResource;
            _watchdog = new Watchdog(550);
            _watchdog.TimeoutOccured += (sender, args) => TimeoutOccured?.Invoke(sender, args);
            

            //TODO unifiy the error event args, so we can use one common event handler
            _messageExtractor = new MessageExtractor();
            _messageExtractor.KeepAliveReceived += (sender, args) => _watchdog.ResetWatchdog();
            _messageExtractor.MessageLostOccured +=
                (sender, args) => _logger.Fatal($"Lost message, total count: {args.TotalLostCount}");
            _messageExtractor.SpeedCurrentFeedbackReceived += (sender, args) => SpeedCurrentFeedbackReceived?.Invoke(sender, args);
            _messageExtractor.VoltageTemperatureFeedbackReceived += (sender, args) => VoltageTemperatureFeedbackReceived?.Invoke(sender, args);

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
