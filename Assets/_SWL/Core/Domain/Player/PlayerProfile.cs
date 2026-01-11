using System.Collections.Generic;

namespace SWL.Core.Domain.Player
{
    /// <summary>
    /// Data Model / Persistence Model / POCO
    /// No rules, no validation, no state change control.
    /// Only the player's current photo is here.
    /// Easily serialized, easy save-load, easy access, minimum dependency with frameworks
    /// NO need to encapsulate. State does not change, use case can.
    /// 
    /// 
    /// UI  --> can not reach profile directly
    /// ↓
    /// UseCase(ConsumeLife, GrantReward, TickLife)    --> applies the rules
    /// ↓
    /// PlayerProfileStore     --> only publishes events
    /// ↓
    /// PlayerProfile   --> only pure data with fields
    ///  
    /// </summary>
    [System.Serializable]
    public sealed class PlayerProfile
    {
        public int Life;
        public int Coins;
        public int Gems;        
        public long NextLifeRegenUnix; // Unix timestamp (seconds)
        public int CurrentLevelIndex;
        public HashSet<int> CompletedLevels = new();
    }
}
