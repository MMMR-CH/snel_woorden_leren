namespace SWL.App.UseCases.Levels
{
    /// <summary>
    /// Result of applying level completion rules to the PlayerProfile.
    /// Pure data container (no Unity dependencies).
    /// </summary>
    public readonly struct LevelResolution
    {
        public readonly bool Success;
        public readonly bool ConsumedLife;
        public readonly int CoinsGained;
        public readonly int GemsGained;
        public readonly int RemainingLife;
        public readonly int NextMainLevelId;

        public LevelResolution(
            bool success,
            bool consumedLife,
            int coinsGained,
            int gemsGained,
            int remainingLife,
            int nextMainLevelId)
        {
            Success = success;
            ConsumedLife = consumedLife;
            CoinsGained = coinsGained;
            GemsGained = gemsGained;
            RemainingLife = remainingLife;
            NextMainLevelId = nextMainLevelId;
        }

        public bool OutOfLives => RemainingLife <= 0;
    }
}
