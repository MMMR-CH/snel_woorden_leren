using System;
using System.Collections.Generic;
using SWL.App.Ports;
using SWL.Core.Domain.League;

namespace SWL.App.UseCases.League
{
    public sealed class GetLeagueWindowUseCase
    {
        private readonly PlayerProfileStore _store;
        private readonly ILeaderboardService _leaderboards;

        public GetLeagueWindowUseCase(PlayerProfileStore store, ILeaderboardService leaderboards)
        {
            _store = store;
            _leaderboards = leaderboards;
        }

        public void Fetch(string leaderboardId, int radius, Action<IReadOnlyList<LeaderboardEntry>> callback)
        {
            if (_leaderboards == null)
            {
                callback?.Invoke(new List<LeaderboardEntry>());
                return;
            }

            var playerId = _store.Profile.PlayerId;
            _leaderboards.GetRankWindow(leaderboardId, playerId, radius, callback);
        }
    }
}
