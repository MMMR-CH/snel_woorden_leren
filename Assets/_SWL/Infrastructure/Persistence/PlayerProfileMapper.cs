using System.Collections.Generic;
using SWL.Core.Domain.Player;

namespace SWL.Infrastructure.Persistence
{
    public static class PlayerProfileMapper
    {
        public static PlayerProfile FromDto(PlayerProfileDto dto)
        {
            var p = new PlayerProfile
            {
                PlayerId = dto.PlayerId,
                Life = dto.Life,
                Coins = dto.Coins,
                Gems = dto.Gems,
                NextLifeRegenUnix = dto.NextLifeRegenUnix,
                CurrentLevelIndex = dto.CurrentLevelIndex,
                LanguageCode = dto.LanguageCode,
                MusicVolume = dto.MusicVolume,
                SfxVolume = dto.SfxVolume,
                VibrationEnabled = dto.VibrationEnabled,
                RemoveAdsOwned = dto.RemoveAdsOwned,
                VipOwned = dto.VipOwned,
                DailyGiftLastClaimUnix = dto.DailyGiftLastClaimUnix,
                DailyGiftStreakDays = dto.DailyGiftStreakDays,
                QuestsLastResetUnix = dto.QuestsLastResetUnix
            };

            if (dto.CompletedLevels != null)
                p.CompletedLevels = new HashSet<int>(dto.CompletedLevels);

            if (dto.UnlockedWordIds != null)
                p.UnlockedWordIds = new HashSet<string>(dto.UnlockedWordIds);

            if (dto.LearnedWordIds != null)
                p.LearnedWordIds = new HashSet<string>(dto.LearnedWordIds);

            if (dto.ClaimedQuestIds != null)
                p.ClaimedQuestIds = new HashSet<string>(dto.ClaimedQuestIds);

            if (dto.QuestProgress != null)
            {
                p.QuestProgress = new Dictionary<string, int>();
                foreach (var e in dto.QuestProgress)
                {
                    if (e == null || string.IsNullOrWhiteSpace(e.QuestId)) continue;
                    p.QuestProgress[e.QuestId] = e.Value;
                }
            }

            return p;
        }

        public static PlayerProfileDto ToDto(PlayerProfile profile)
        {
            var dto = new PlayerProfileDto
            {
                PlayerId = profile.PlayerId,
                Life = profile.Life,
                Coins = profile.Coins,
                Gems = profile.Gems,
                NextLifeRegenUnix = profile.NextLifeRegenUnix,
                CurrentLevelIndex = profile.CurrentLevelIndex,
                CompletedLevels = profile.CompletedLevels != null ? new List<int>(profile.CompletedLevels).ToArray() : null,
                UnlockedWordIds = profile.UnlockedWordIds != null ? new List<string>(profile.UnlockedWordIds).ToArray() : null,
                LearnedWordIds = profile.LearnedWordIds != null ? new List<string>(profile.LearnedWordIds).ToArray() : null,
                LanguageCode = profile.LanguageCode,
                MusicVolume = profile.MusicVolume,
                SfxVolume = profile.SfxVolume,
                VibrationEnabled = profile.VibrationEnabled,
                RemoveAdsOwned = profile.RemoveAdsOwned,
                VipOwned = profile.VipOwned,
                DailyGiftLastClaimUnix = profile.DailyGiftLastClaimUnix,
                DailyGiftStreakDays = profile.DailyGiftStreakDays,
                QuestsLastResetUnix = profile.QuestsLastResetUnix,
                ClaimedQuestIds = profile.ClaimedQuestIds != null ? new List<string>(profile.ClaimedQuestIds).ToArray() : null,
            };

            if (profile.QuestProgress != null)
            {
                var list = new List<PlayerProfileDto.QuestProgressEntryDto>();
                foreach (var kv in profile.QuestProgress)
                {
                    if (string.IsNullOrWhiteSpace(kv.Key)) continue;
                    list.Add(new PlayerProfileDto.QuestProgressEntryDto { QuestId = kv.Key, Value = kv.Value });
                }
                dto.QuestProgress = list.ToArray();
            }

            return dto;
        }
    }
}
