namespace SWL.App.UseCases
{
    public sealed class ConsumeLifeUseCase
    {
        private readonly PlayerProfileStore _store;

        public ConsumeLifeUseCase(PlayerProfileStore store) => _store = store;

        public bool CanStart() => _store.Profile.Life > 0;

        public void ConsumeOnFail()
        {
            if (_store.Profile.Life <= 0) return;
            _store.Profile.Life--;
            _store.NotifyChanged();
        }
    }
}
