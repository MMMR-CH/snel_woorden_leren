namespace SWL.Core.Domain.Levels
{
    public readonly struct LevelResult
    {
        public readonly bool Success;
        public readonly int CoinsReward;
        public readonly int GemsReward;

        public LevelResult(bool success, int coinsReward, int gemsReward)
        {
            Success = success;
            CoinsReward = coinsReward;
            GemsReward = gemsReward;
        }

        public static LevelResult Fail() => new(false, 0, 0);
    }
}
