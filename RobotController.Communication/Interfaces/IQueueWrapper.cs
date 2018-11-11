using RobotController.Communication.Enums;

namespace RobotController.Communication.Interfaces
{
    public interface IQueueWrapper
    {
        void Enqueue(ICommand message, EPriority priority);
        ICommand Dequeue();
        int Count();
    }
}