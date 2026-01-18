using System.Collections.Generic;
using SWL.Core.Domain.Words;

namespace SWL.App.UseCases.Words
{
    public readonly struct WordsProgress
    {
        public readonly int Total;
        public readonly int Unlocked;
        public readonly int Learned;

        public WordsProgress(int total, int unlocked, int learned)
        {
            Total = total;
            Unlocked = unlocked;
            Learned = learned;
        }

        public float LearnedRatio => Total <= 0 ? 0f : (float)Learned / Total;
    }

    public sealed class GetWordsProgressUseCase
    {
        private readonly PlayerProfileStore _store;

        public GetWordsProgressUseCase(PlayerProfileStore store)
        {
            _store = store;
        }

        public WordsProgress GetSummary(IWordCatalog catalog)
        {
            var total = catalog?.Count ?? 0;
            var p = _store.Profile;
            var unlocked = p.UnlockedWordIds?.Count ?? 0;
            var learned = p.LearnedWordIds?.Count ?? 0;
            return new WordsProgress(total, unlocked, learned);
        }

        public List<WordProgressItem> GetItems(IWordCatalog catalog)
        {
            var list = new List<WordProgressItem>();
            if (catalog == null) return list;

            var p = _store.Profile;
            var unlockedSet = p.UnlockedWordIds ?? new System.Collections.Generic.HashSet<string>();
            var learnedSet = p.LearnedWordIds ?? new System.Collections.Generic.HashSet<string>();

            foreach (var w in catalog.All)
            {
                if (w == null) continue;
                var unlocked = unlockedSet.Contains(w.Id);
                var learned = learnedSet.Contains(w.Id);
                list.Add(new WordProgressItem(w.Id, unlocked, learned));
            }

            return list;
        }
    }
}
