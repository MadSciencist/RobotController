using RobotController.RobotModels.PhysicalConverters;
using Xunit;

namespace RobotController.Tests.ConvertersTest
{

    public class PhysicalUnitsConvertersTests
    {
        [Fact]
        public void VoltageConverter_CanConvertToPhysical_ReturnProperDouble()
        {
            // Arrange & Act
            var result = VoltageConverter.GetPhysical(1000);

            //Assert
            Assert.IsType<double>(result);
            Assert.Equal(53.8d, result, 1);
        }

        [Fact]
        public void VoltageConverter_CanConvertToBit_ReturnProperShort()
        {
            // Arrange & Act
            var result = VoltageConverter.GetBit(53.8d);

            //Assert
            Assert.IsType<short>(result);
            Assert.Equal(1000, result);
        }

        [Fact]
        public void CurrentConverter_CanConvertToPhysical_ReturnProperDouble()
        {
            // Arrange & Act
            var result = CurrentConverter.GetPhysical(1000);

            //Assert
            Assert.IsType<double>(result);
            Assert.Equal(36.2d, result, 1);
        }

        [Fact]
        public void CurrentConverter_CanConvertToBit_ReturnProperShort()
        {
            // Arrange & Act
            var result = CurrentConverter.GetBit(36.2);

            //Assert
            Assert.IsType<short>(result);
            Assert.Equal(1000, result);
        }
    }
}
