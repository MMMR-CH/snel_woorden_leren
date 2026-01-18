namespace SWL.App.UseCases.DailyGift
{
    public readonly struct DailyGiftClaimResult
    {
        public readonly bool Success;
        public readonly int StreakDay;
        public readonly int Coins;
        public readonly int Gems;

        public DailyGiftClaimResult(bool success, int streakDay, int coins, int gems)
        {
            Success = success;
            StreakDay = streakDay;
            Coins = coins;
            Gems = gems;
        }
    }

    public sealed class ClaimDailyGiftUseCase
    {
        private readonly PlayerProfileStore _store;
        private readonly GetDailyGiftStateUseCase _state;

        public ClaimDailyGiftUseCase(PlayerProfileStore store)
        {
            _store = store;
            _state = new GetDailyGiftStateUseCase(store);
        }

        public DailyGiftClaimResult Claim(long nowUnix)
        {
            var state = _state.Get(nowUnix);
            if (!state.CanClaim)
                return new DailyGiftClaimResult(false, state.StreakDay, 0, 0);

            var p = _store.Profile;

            p.DailyGiftStreakDays = state.StreakDay;
            p.DailyGiftLastClaimUnix = nowUnix;

            p.Coins += state.NextRewardCoins;
            p.Gems += state.NextRewardGems;

            _store.NotifyChanged();

            return new DailyGiftClaimResult(true, state.StreakDay, state.NextRewardCoins, state.NextRewardGems);
        }
    }
}
