using Priority_Queue;
using RobotController.Communication.Enums;
using RobotController.Communication.Interfaces;

namespace RobotController.Communication.SendingTask
{

    public class SendQueueWrapper : ISendQueueWrapper
    {
        private readonly SimplePriorityQueue<ISendMessage> _queue;

        public SendQueueWrapper()
        {
            _queue = new SimplePriorityQueue<ISendMessage>();
        }

        public void Enqueue(ISendMessage message, EPriority priority)
        {
            _queue.Enqueue(message, (int)priority);
        }

        public ISendMessage Dequeue()
        {
            return _queue.Dequeue();
        }

        public int Count()
        {
            return _queue.Count;
        }
    }
}
