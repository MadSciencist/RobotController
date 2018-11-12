using RobotController.Communication.Enums;

namespace RobotController.Communication.Interfaces
{
    public interface ISendQueueWrapper
    {
        void Enqueue(ISendMessage message, EPriority priority);
        ISendMessage Dequeue();
        int Count();
    }
}