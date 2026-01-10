using SWL.Core.Domain.Player;

namespace SWL.App.UseCases
{
    public sealed class TickLifeRegenUseCase
    {
        private readonly PlayerProfileStore _store;

        public TickLifeRegenUseCase(PlayerProfileStore store)
        {
            _store = store;
        }

        public void Tick(long nowUnix)
        {
            var profile = _store.Profile;

            if (profile.Life >= LifeRules.MaxLife)
                return;

            if (nowUnix < profile.NextLifeRegenUnix)
                return;

            profile.Life++;

            if (profile.Life < LifeRules.MaxLife)
                profile.NextLifeRegenUnix = nowUnix + LifeRules.RegenSeconds;

            _store.NotifyChanged();
        }
    }
}
