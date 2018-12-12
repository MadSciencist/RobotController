using Moq;
using RobotController.Communication;
using RobotController.Communication.ReceivingTask;
using Xunit;

namespace RobotController.Tests.CommunicationTests
{
    public class ReceiverTaskTests
    {
        [Fact]
        void ReceiverTask_RisedEventSetsCorrectData_ArraysAreEqual()
        {
            var data = new byte[] {60, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10, 10, 62};
            var receivedData = new byte[22];

            var streamResource = new Mock<IStreamResource>();
            streamResource.Setup(x => x.Read(data, 0, 22)).Returns(22);
            streamResource.Setup(x => x.BytesToRead()).Returns(22);

            var receiverTask = new ReceiverTask(streamResource.Object);
            receiverTask.DataReceived += (sender, args) => receivedData = args.Data;
            receiverTask.Start();

            Assert.Equal(receivedData.Length, data.Length);
        }
    }
}
