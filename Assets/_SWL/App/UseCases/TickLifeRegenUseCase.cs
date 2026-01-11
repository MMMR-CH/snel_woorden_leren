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
            var p = _store.Profile;

            if (p.Life >= LifeRules.MaxLife)
            {
                // stop timer if life full
                if (p.NextLifeRegenUnix != 0)
                {
                    p.NextLifeRegenUnix = 0;
                    _store.NotifyChanged();
                }
                return;
            }

            // start timer if not started
            if (p.NextLifeRegenUnix <= 0)
            {
                p.NextLifeRegenUnix = nowUnix + LifeRules.RegenSeconds;
                _store.NotifyChanged();
                return;
            }

            if (nowUnix < p.NextLifeRegenUnix)
                return;

            int interval = LifeRules.RegenSeconds;
            long extraSecondsPast = nowUnix - p.NextLifeRegenUnix;

            // 1 life is earned already, and then based on the interval
            var gained = 1 + (int)(extraSecondsPast / interval);

            p.Life = p.Life + gained;
            if (p.Life >= LifeRules.MaxLife)
            {
                p.Life = LifeRules.MaxLife;
                p.NextLifeRegenUnix = 0;
            }
            else
            {
                p.NextLifeRegenUnix = p.NextLifeRegenUnix + gained * interval;
            }

            _store.NotifyChanged();
        }
    }
}
