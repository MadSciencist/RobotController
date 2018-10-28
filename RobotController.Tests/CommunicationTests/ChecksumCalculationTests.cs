using RobotController.Communication.Utils;
using System;
using Xunit;

namespace RobotController.Tests
{
    public class ChecksumCalculationTests
    {
        [Fact]
        public void CrcUtilReturnProperValue()
        {
            var payload = new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            Assert.Equal(12, payload.Length);

            ushort crc = ChecksumUtils.CalculateCrc(payload);

            Assert.Equal(65120, crc);
        }

        [Fact]
        public void CrcUtilOverloadedMethodsReturnEqualValues()
        {
            var frame = new byte[] { 0x03, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x60, 0x61, 0x3E};
            Assert.Equal(16, frame.Length);

            var payload = new byte[12];
            Buffer.BlockCopy(frame, 1, payload, 0, 12);

            var crc = ChecksumUtils.CalculateCrc(payload);
            Assert.Equal(65120, crc);

            var crc2 = ChecksumUtils.CalculateCrc(frame, 1, 12);
            Assert.Equal(crc, crc2);
        }
    }
}
