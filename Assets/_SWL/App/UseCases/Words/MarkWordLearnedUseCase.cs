namespace SWL.App.UseCases.Words
{
    public sealed class MarkWordLearnedUseCase
    {
        private readonly PlayerProfileStore _store;

        public MarkWordLearnedUseCase(PlayerProfileStore store)
        {
            _store = store;
        }

        public void MarkLearned(string wordId)
        {
            if (string.IsNullOrWhiteSpace(wordId)) return;

            var p = _store.Profile;
            p.UnlockedWordIds ??= new System.Collections.Generic.HashSet<string>();
            p.LearnedWordIds ??= new System.Collections.Generic.HashSet<string>();

            if (!p.UnlockedWordIds.Contains(wordId))
                p.UnlockedWordIds.Add(wordId); // auto-unlock

            if (p.LearnedWordIds.Add(wordId))
                _store.NotifyChanged();
        }
    }
}
