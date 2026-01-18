using System;
using UnityEngine;
using SWL.App.Ports;

namespace SWL.Infrastructure.Connectivity
{
    public sealed class UnityConnectivityService : IConnectivityService
    {
        public bool IsOnline { get; private set; }
        public event Action<bool> Changed;

        public UnityConnectivityService()
        {
            IsOnline = ReadOnline();
        }

        public void Poll()
        {
            var now = ReadOnline();
            if (now == IsOnline) return;

            IsOnline = now;
            Changed?.Invoke(IsOnline);
        }

        private static bool ReadOnline()
        {
            // Note: This does not guarantee internet access, only network reachability.
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }
}
