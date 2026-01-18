using System;

namespace SWL.App.Ports
{
    public interface IConnectivityService
    {
        bool IsOnline { get; }
        event Action<bool> Changed;

        /// <summary>Optional polling entry for platforms where connectivity callbacks are unavailable.</summary>
        void Poll();
    }
}
