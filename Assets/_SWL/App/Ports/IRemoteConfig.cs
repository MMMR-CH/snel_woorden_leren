using System;

namespace SWL.App.Ports
{
    public interface IRemoteConfig
    {
        void FetchAndActivate(Action<bool> onCompleted = null);

        bool GetBool(string key, bool defaultValue = false);
        int GetInt(string key, int defaultValue = 0);
        float GetFloat(string key, float defaultValue = 0f);
        string GetString(string key, string defaultValue = null);
    }
}
