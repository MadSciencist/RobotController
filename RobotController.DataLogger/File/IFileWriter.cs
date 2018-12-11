namespace RobotController.DataLogger.File
{
    public interface IFileWriter
    {
        void WriteLine(string line);
        void Dispose();
    }
}
