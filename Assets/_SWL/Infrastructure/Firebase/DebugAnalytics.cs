using UnityEngine;
using SWL.App.Ports;

namespace SWL.Infrastructure.Firebase
{
    /// <summary>
    /// Development analytics implementation: logs to console.
    /// Replace with Firebase Analytics later.
    /// </summary>
    public sealed class DebugAnalytics : IAnalytics
    {
        private string _userId;

        public void SetUserId(string userId)
        {
            _userId = userId;
            Debug.Log($"[Analytics] SetUserId: {_userId}");
        }

        public void SetUserProperty(string key, string value)
        {
            Debug.Log($"[Analytics] UserProp {key}={value}");
        }

        public void LogEvent(string eventName)
        {
            Debug.Log($"[Analytics] {eventName}");
        }

        public void LogEvent(string eventName, params AnalyticsParam[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                LogEvent(eventName);
                return;
            }

            var msg = $"[Analytics] {eventName} |";
            for (int i = 0; i < parameters.Length; i++)
            {
                msg += $" {parameters[i].Key}={parameters[i].Value}";
            }
            Debug.Log(msg);
        }
    }
}
