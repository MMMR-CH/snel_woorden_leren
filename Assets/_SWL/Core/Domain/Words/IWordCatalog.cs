using System.Collections.Generic;

namespace SWL.Core.Domain.Words
{
    /// <summary>
    /// Read-only catalog of all words.
    /// Implementations are typically ScriptableObjects in the Content assembly.
    /// </summary>
    public interface IWordCatalog
    {
        int Count { get; }
        IEnumerable<WordEntry> All { get; }
        bool TryGet(string id, out WordEntry entry);
    }
}
