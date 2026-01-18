namespace SWL.App.UseCases.Words
{
    public sealed class UnlockWordsUseCase
    {
        private readonly PlayerProfileStore _store;

        public UnlockWordsUseCase(PlayerProfileStore store)
        {
            _store = store;
        }

        public void Unlock(params string[] wordIds)
        {
            if (wordIds == null || wordIds.Length == 0) return;

            var p = _store.Profile;
            p.UnlockedWordIds ??= new System.Collections.Generic.HashSet<string>();

            bool changed = false;
            for (int i = 0; i < wordIds.Length; i++)
            {
                var id = wordIds[i];
                if (string.IsNullOrWhiteSpace(id)) continue;
                changed |= p.UnlockedWordIds.Add(id);
            }

            if (changed)
                _store.NotifyChanged();
        }
    }
}
