using System.IO;
using UnityEngine;
using SWL.App.Ports;
using SWL.Core.Domain.Player;

namespace SWL.Infrastructure.Save
{
    public sealed class LocalJsonPlayerSave : IPlayerSave
    {
        private string FilePath =>
            Path.Combine(Application.persistentDataPath, "player_profile.json");

        public PlayerProfile Load()
        {
            if (!File.Exists(FilePath))
                return CreateNewProfile();

            var json = File.ReadAllText(FilePath);
            return JsonUtility.FromJson<PlayerProfile>(json);
        }

        public void Save(PlayerProfile profile)
        {
            var json = JsonUtility.ToJson(profile, true);
            File.WriteAllText(FilePath, json);
        }

        private PlayerProfile CreateNewProfile()
        {
            var now = GetUnixTime();

            return new PlayerProfile
            {
                Life = LifeRules.InitialLife,
                Coins = 0,
                Gems = 0,
                CurrentLevelIndex = 1,
                NextLifeRegenUnix = now + LifeRules.RegenSeconds
            };
        }

        private long GetUnixTime()
        {
            return System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}
