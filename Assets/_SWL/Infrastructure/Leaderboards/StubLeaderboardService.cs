using System;
using System.Collections.Generic;
using SWL.App.Ports;
using SWL.Core.Domain.League;

namespace SWL.Infrastructure.Leaderboards
{
    /// <summary>
    /// Deterministic in-memory leaderboard for development.
    /// </summary>
    public sealed class StubLeaderboardService : ILeaderboardService
    {
        private readonly List<LeaderboardEntry> _entries = new();

        public StubLeaderboardService()
        {
            // generate static list
            for (int i = 1; i <= 250; i++)
            {
                var playerId = $"player_{i:000}";
                var name = $"Player {i:000}";
                // higher ranks have higher score
                var score = (251 - i) * 10;
                _entries.Add(new LeaderboardEntry(i, playerId, name, score));
            }
        }

        public void GetRankWindow(string leaderboardId, string playerId, int radius, Action<IReadOnlyList<LeaderboardEntry>> callback)
        {
            if (radius < 0) radius = 0;

            int idx = -1;
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].PlayerId == playerId)
                {
                    idx = i;
                    break;
                }
            }

            // If player not found, put them at the end
            if (idx < 0)
                idx = _entries.Count - 1;

            int start = Math.Max(0, idx - radius);
            int end = Math.Min(_entries.Count - 1, idx + radius);

            var list = new List<LeaderboardEntry>(end - start + 1);
            for (int i = start; i <= end; i++)
                list.Add(_entries[i]);

            callback?.Invoke(list);
        }
    }
}
