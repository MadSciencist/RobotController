using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using RobotController.Communication;
using RobotController.Communication.Interfaces;
using RobotController.Communication.ReceivingTask;

namespace RobotController.Tests.CommunicationTests
{
    public class ReceiverTaskTests
    {
        [Fact]
        void Ad()
        {
            var data = new byte[] {60, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10, 10, 62};

            var streamResource = new Mock<IStreamResource>().Object;
            var receiverTask = new ReceiverTask(streamResource);

            Assert.IsNotType<ReceiverTask>(receiverTask);

        }
    }
}
