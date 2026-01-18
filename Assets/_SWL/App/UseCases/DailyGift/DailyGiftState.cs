namespace SWL.App.UseCases.DailyGift
{
    public readonly struct DailyGiftState
    {
        public readonly bool CanClaim;
        public readonly int StreakDay;
        public readonly long SecondsUntilClaim;
        public readonly int NextRewardCoins;
        public readonly int NextRewardGems;

        public DailyGiftState(bool canClaim, int streakDay, long secondsUntilClaim, int nextRewardCoins, int nextRewardGems)
        {
            CanClaim = canClaim;
            StreakDay = streakDay;
            SecondsUntilClaim = secondsUntilClaim;
            NextRewardCoins = nextRewardCoins;
            NextRewardGems = nextRewardGems;
        }
    }
}
