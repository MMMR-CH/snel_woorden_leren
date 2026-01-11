namespace SWL.App.Ports
{
    public interface ITimeService
    {
        long UtcNowUnixSeconds { get; }
    }
}
