using SWL.Core.Domain.Player;

namespace SWL.App.UseCases
{
    public sealed class ConsumeLifeUseCase
    {
        private readonly PlayerProfileStore _store;

        public ConsumeLifeUseCase(PlayerProfileStore store)
        {
            _store = store;
        }

        public bool TryConsume()
        {
            if (_store.Profile.Life <= 0)
                return false;

            _store.Profile.Life--;
            _store.NotifyChanged();
            return true;
        }
    }
}
