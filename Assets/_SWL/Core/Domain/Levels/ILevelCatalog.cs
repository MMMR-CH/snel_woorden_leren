namespace SWL.Core.Domain.Levels
{
    /// <summary>
    /// Read-only access to the game's level ordering.
    /// Implementations are typically ScriptableObjects in the Content assembly.
    /// </summary>
    public interface ILevelCatalog
    {
        int Count { get; }

        bool TryGetById(int levelId, out LevelSpec spec);
        bool TryGetByIndex(int index, out LevelSpec spec);

        /// <summary>Returns the 0-based index for the levelId, or -1 if not found.</summary>
        int IndexOf(int levelId);

        /// <summary>Returns the next level after <paramref name="levelId"/> according to catalog order.</summary>
        bool TryGetNextAfter(int levelId, out LevelSpec next);
    }
}
