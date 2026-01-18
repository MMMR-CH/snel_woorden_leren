namespace SWL.App.Ports
{
    public interface IAnalytics
    {
        void SetUserId(string userId);
        void SetUserProperty(string key, string value);

        void LogEvent(string eventName);
        void LogEvent(string eventName, params AnalyticsParam[] parameters);
    }
}
