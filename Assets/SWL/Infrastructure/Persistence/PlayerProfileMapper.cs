using SWL.Core.Domain.Player;

namespace SWL.Infrastructure.Persistence
{
    public static class PlayerProfileMapper
    {
        public static PlayerProfile FromDto(PlayerProfileDto dto)
            => new PlayerProfile
            {
                Life = dto.Life,
                Coins = dto.Coins,
                Gems = dto.Gems,
                NextLifeRegenUnix = dto.NextLifeRegenUnix,
                CurrentLevelIndex = dto.CurrentLevelIndex
            };

        public static PlayerProfileDto ToDto(PlayerProfile profile)
            => new PlayerProfileDto
            {
                Life = profile.Life,
                Coins = profile.Coins,
                Gems = profile.Gems,
                NextLifeRegenUnix = profile.NextLifeRegenUnix,
                CurrentLevelIndex = profile.CurrentLevelIndex
            };
    }
}
