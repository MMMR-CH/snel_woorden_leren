using System;

namespace SWL.Infrastructure.Persistence
{
    [Serializable]
    public sealed class PlayerProfileDto
    {
        public string PlayerId;

        public int Life;
        public int Coins;
        public int Gems;
        public long NextLifeRegenUnix;

        public int CurrentLevelIndex;
        public int[] CompletedLevels;

        public string[] UnlockedWordIds;
        public string[] LearnedWordIds;

        public string LanguageCode;
        public float MusicVolume;
        public float SfxVolume;
        public bool VibrationEnabled;

        public bool RemoveAdsOwned;
        public bool VipOwned;

        public long DailyGiftLastClaimUnix;
        public int DailyGiftStreakDays;

        public long QuestsLastResetUnix;
        public string[] ClaimedQuestIds;
        public QuestProgressEntryDto[] QuestProgress;

        [Serializable]
        public sealed class QuestProgressEntryDto
        {
            public string QuestId;
            public int Value;
        }
    }
}
