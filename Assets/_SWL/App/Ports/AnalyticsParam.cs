namespace SWL.App.Ports
{
    public readonly struct AnalyticsParam
    {
        public readonly string Key;
        public readonly string Value;

        public AnalyticsParam(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
