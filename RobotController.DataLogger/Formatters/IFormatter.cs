namespace RobotController.DataLogger.Formatters
{
    public interface IFormatter
    {
        string Format(DatalogModel log);
        string GetHeader();
    }
}
