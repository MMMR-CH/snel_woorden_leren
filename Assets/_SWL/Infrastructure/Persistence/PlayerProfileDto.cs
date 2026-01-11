using System;

namespace SWL.Infrastructure.Persistence
{
    [Serializable]
    public sealed class PlayerProfileDto
    {
        public int Life;
        public int Coins;
        public int Gems;
        public long NextLifeRegenUnix;
        public int CurrentLevelIndex;
    }
}