using System.Collections.Generic;
using SWL.Core.Domain.Words;
using UnityEngine;

namespace SWL.Content.Words
{
    [CreateAssetMenu(menuName = "SWL/Words/Word Catalog", fileName = "WordCatalog")]
    public sealed class WordCatalogSO : ScriptableObject, IWordCatalog
    {
        public List<WordEntry> Words = new();

        public int Count => Words?.Count ?? 0;

        public IEnumerable<WordEntry> All => Words;

        public bool TryGet(string id, out WordEntry entry)
        {
            if (Words != null)
            {
                for (int i = 0; i < Words.Count; i++)
                {
                    var w = Words[i];
                    if (w != null && w.Id == id)
                    {
                        entry = w;
                        return true;
                    }
                }
            }

            entry = null;
            return false;
        }
    }
}
