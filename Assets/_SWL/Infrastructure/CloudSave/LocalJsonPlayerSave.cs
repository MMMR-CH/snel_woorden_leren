using System;
using System.IO;
using UnityEngine;
using SWL.App.Ports;
using SWL.Core.Domain.Player;

namespace SWL.Infrastructure.Save
{
    public sealed class LocalJsonPlayerSave : IPlayerSave
    {
        private string FilePath => Path.Combine(Application.persistentDataPath, "player_profile.json");

        public PlayerProfile Load()
        {
            if (!File.Exists(FilePath))
                return CreateNewProfile();

            string json;
            try
            {
                json = File.ReadAllText(FilePath);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to read save file: {e.Message}");
                return CreateNewProfile();
            }

            PlayerProfile profile;
            try
            {
                profile = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerProfile>(json);
            }
            catch (Exception e)
            {
                BackupCorruptSave(json);
                Debug.LogWarning($"Save file is corrupt. Creating new profile. Error: {e.Message}");
                return CreateNewProfile();
            }

            profile ??= CreateNewProfile();
            Normalize(profile);
            return profile;
        }

        public void Save(PlayerProfile profile)
        {
            if (profile == null) return;
            Normalize(profile);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(profile, Settings);

            try
            {
                var tmp = FilePath + ".tmp";
                File.WriteAllText(tmp, json);
                if (File.Exists(FilePath)) File.Delete(FilePath);
                File.Move(tmp, FilePath);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to write save file: {e.Message}");
            }
        }

        private PlayerProfile CreateNewProfile()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            return new PlayerProfile
            {
                PlayerId = Guid.NewGuid().ToString("N"),
                Life = LifeRules.InitialLife,
                Coins = 0,
                Gems = 0,
                CurrentLevelIndex = 1,
                NextLifeRegenUnix = now + LifeRules.RegenSeconds
            };
        }

        private void Normalize(PlayerProfile p)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (string.IsNullOrWhiteSpace(p.PlayerId))
                p.PlayerId = Guid.NewGuid().ToString("N");

            p.CompletedLevels ??= new System.Collections.Generic.HashSet<int>();
            p.UnlockedWordIds ??= new System.Collections.Generic.HashSet<string>();
            p.LearnedWordIds ??= new System.Collections.Generic.HashSet<string>();
            p.QuestProgress ??= new System.Collections.Generic.Dictionary<string, int>();
            p.ClaimedQuestIds ??= new System.Collections.Generic.HashSet<string>();

            if (p.CurrentLevelIndex <= 0) p.CurrentLevelIndex = 1;

            p.Life = Mathf.Clamp(p.Life, 0, LifeRules.MaxLife);
            if (p.MusicVolume < 0f) p.MusicVolume = 0f;
            if (p.MusicVolume > 1f) p.MusicVolume = 1f;
            if (p.SfxVolume < 0f) p.SfxVolume = 0f;
            if (p.SfxVolume > 1f) p.SfxVolume = 1f;

            // Regen timer rules
            if (p.Life >= LifeRules.MaxLife)
            {
                p.Life = LifeRules.MaxLife;
                p.NextLifeRegenUnix = 0;
            }
            else
            {
                if (p.NextLifeRegenUnix <= 0)
                    p.NextLifeRegenUnix = now + LifeRules.RegenSeconds;
            }

            // Language default
            if (string.IsNullOrWhiteSpace(p.LanguageCode))
                p.LanguageCode = "EN";
        }

        private void BackupCorruptSave(string rawJson)
        {
            try
            {
                var dir = Path.GetDirectoryName(FilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var stamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var backup = FilePath + $".corrupt.{stamp}";
                File.WriteAllText(backup, rawJson);
            }
            catch
            {
                // ignore
            }
        }

        private static readonly Newtonsoft.Json.JsonSerializerSettings Settings =
            new()
            {
                Formatting = Newtonsoft.Json.Formatting.None,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            };
    }
}
