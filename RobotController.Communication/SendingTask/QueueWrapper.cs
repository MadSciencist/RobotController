using Priority_Queue;
using RobotController.Communication.Enums;
using RobotController.Communication.Interfaces;

namespace RobotController.Communication.SendingTask
{

    public class QueueWrapper : IQueueWrapper
    {
        private readonly SimplePriorityQueue<ICommand> _queue;

        public QueueWrapper()
        {
            _queue = new SimplePriorityQueue<ICommand>();
        }

        public void Enqueue(ICommand message, EPriority priority)
        {
            _queue.Enqueue(message, (int)priority);
        }

        public ICommand Dequeue()
        {
            return _queue.Dequeue();
        }

        public int Count()
        {
            return _queue.Count;
        }
    }
}
