namespace SWL.App.UseCases.DailyGift
{
    public static class DailyGiftRules
    {
        public const int ClaimCooldownSeconds = 24 * 60 * 60;
        public const int StreakBreakSeconds = 48 * 60 * 60;

        // 7-day loop
        private static readonly int[] CoinsByDay = { 50, 75, 100, 125, 150, 200, 250 };
        private static readonly int[] GemsByDay = { 0, 0, 0, 0, 0, 0, 5 };

        public static int CoinsForDay(int streakDay)
        {
            var i = ((streakDay - 1) % 7);
            if (i < 0) i = 0;
            return CoinsByDay[i];
        }

        public static int GemsForDay(int streakDay)
        {
            var i = ((streakDay - 1) % 7);
            if (i < 0) i = 0;
            return GemsByDay[i];
        }
    }
}
