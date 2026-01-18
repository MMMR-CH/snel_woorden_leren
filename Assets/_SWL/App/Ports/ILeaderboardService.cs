using System;
using System.Collections.Generic;
using SWL.Core.Domain.League;

namespace SWL.App.Ports
{
    public interface ILeaderboardService
    {
        void GetRankWindow(string leaderboardId, string playerId, int radius, Action<IReadOnlyList<LeaderboardEntry>> callback);
    }
}
