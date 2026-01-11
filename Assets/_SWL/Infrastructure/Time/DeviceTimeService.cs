using System;
using SWL.App.Ports;

namespace SWL.Infrastructure.Time
{
    public sealed class DeviceTimeService : ITimeService
    {
        public long UtcNowUnixSeconds => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}
