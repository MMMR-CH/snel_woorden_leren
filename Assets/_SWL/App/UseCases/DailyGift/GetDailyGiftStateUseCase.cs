namespace SWL.App.UseCases.DailyGift
{
    public sealed class GetDailyGiftStateUseCase
    {
        private readonly PlayerProfileStore _store;

        public GetDailyGiftStateUseCase(PlayerProfileStore store)
        {
            _store = store;
        }

        public DailyGiftState Get(long nowUnix)
        {
            var p = _store.Profile;
            var last = p.DailyGiftLastClaimUnix;

            if (last <= 0)
            {
                var streak = 1;
                return new DailyGiftState(true, streak, 0, DailyGiftRules.CoinsForDay(streak), DailyGiftRules.GemsForDay(streak));
            }

            var since = nowUnix - last;
            var can = since >= DailyGiftRules.ClaimCooldownSeconds;
            var secondsLeft = can ? 0 : (DailyGiftRules.ClaimCooldownSeconds - since);

            var nextStreak = p.DailyGiftStreakDays;

            // if streak is broken, next claim will reset to 1
            if (since >= DailyGiftRules.StreakBreakSeconds)
                nextStreak = 0;

            var proposed = nextStreak + 1;
            if (proposed < 1) proposed = 1;

            return new DailyGiftState(can, proposed, secondsLeft, DailyGiftRules.CoinsForDay(proposed), DailyGiftRules.GemsForDay(proposed));
        }
    }
}
