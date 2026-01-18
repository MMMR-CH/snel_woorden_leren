using System;
using System.Collections.Generic;

namespace SWL.Core.Domain.Player
{
    /// <summary>
    /// Pure persistence model (POCO). No logic; rules are applied in UseCases.
    /// </summary>
    [Serializable]
    public sealed class PlayerProfile
    {
        // Identity
        public string PlayerId;

        // HUD currencies + life
        public int Life;
        public int Coins;
        public int Gems;
        public long NextLifeRegenUnix; // Unix timestamp (seconds)

        // Progression
        public int CurrentLevelIndex; // acts as "next main level id" for roadmap
        public HashSet<int> CompletedLevels = new();

        // Words
        public HashSet<string> UnlockedWordIds = new();
        public HashSet<string> LearnedWordIds = new();

        // Settings
        public string LanguageCode = "EN";
        public float MusicVolume = 1f;
        public float SfxVolume = 1f;
        public bool VibrationEnabled = true;

        // Shop entitlements
        public bool RemoveAdsOwned;
        public bool VipOwned;

        // Daily gift
        public long DailyGiftLastClaimUnix;
        public int DailyGiftStreakDays;

        // Quests (skeleton)
        public long QuestsLastResetUnix;
        public Dictionary<string, int> QuestProgress = new();
        public HashSet<string> ClaimedQuestIds = new();
    }
}
