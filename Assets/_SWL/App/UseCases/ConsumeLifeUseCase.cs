using System;
using SWL.Core.Domain.Player;

namespace SWL.App.UseCases
{
    public sealed class ConsumeLifeUseCase
    {
        private readonly PlayerProfileStore _store;

        public ConsumeLifeUseCase(PlayerProfileStore store) => _store = store;

        public bool CanStart() => _store.Profile.Life > 0;

        public void ConsumeOnFail(long nowUnix = 0)
        {
            var p = _store.Profile;
            if (p.Life <= 0) return;

            var wasMax = p.Life >= LifeRules.MaxLife;
            p.Life--;
            if (p.Life < 0) p.Life = 0;

            if (wasMax && p.Life < LifeRules.MaxLife)
            {
                if (nowUnix <= 0)
                    nowUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                if (p.NextLifeRegenUnix <= 0)
                    p.NextLifeRegenUnix = nowUnix + LifeRules.RegenSeconds;
            }

            _store.NotifyChanged();
        }
    }
}
