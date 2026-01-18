namespace SWL.Core.Domain.League
{
    public readonly struct LeaderboardEntry
    {
        public readonly int Rank;
        public readonly string PlayerId;
        public readonly string DisplayName;
        public readonly int Score;

        public LeaderboardEntry(int rank, string playerId, string displayName, int score)
        {
            Rank = rank;
            PlayerId = playerId;
            DisplayName = displayName;
            Score = score;
        }
    }
}
