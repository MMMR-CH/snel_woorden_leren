using System;
using System.Collections.Generic;
using SWL.App.Ports;

namespace SWL.Infrastructure.RemoteConfig
{
    /// <summary>
    /// Local in-memory remote config. Useful for play mode.
    /// Replace with Firebase Remote Config later.
    /// </summary>
    public sealed class StubRemoteConfig : IRemoteConfig
    {
        private readonly Dictionary<string, object> _values = new();

        public void Set(string key, object value) => _values[key] = value;

        public void FetchAndActivate(Action<bool> onCompleted = null)
        {
            // nothing to fetch in stub
            onCompleted?.Invoke(true);
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            if (_values.TryGetValue(key, out var v) && v is bool b) return b;
            return defaultValue;
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            if (_values.TryGetValue(key, out var v))
            {
                if (v is int i) return i;
                if (v is long l) return (int)l;
                if (v is float f) return (int)f;
                if (v is double d) return (int)d;
            }
            return defaultValue;
        }

        public float GetFloat(string key, float defaultValue = 0f)
        {
            if (_values.TryGetValue(key, out var v))
            {
                if (v is float f) return f;
                if (v is double d) return (float)d;
                if (v is int i) return i;
                if (v is long l) return l;
            }
            return defaultValue;
        }

        public string GetString(string key, string defaultValue = null)
        {
            if (_values.TryGetValue(key, out var v) && v != null) return v.ToString();
            return defaultValue;
        }
    }
}
