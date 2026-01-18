using SWL.Core.Domain.Levels;
using SWL.Core.Domain.Player;

namespace SWL.App.UseCases.Levels
{
    public sealed class ResolveLevelResultUseCase
    {
        private readonly PlayerProfileStore _store;
        private readonly ILevelCatalog _catalog;

        public ResolveLevelResultUseCase(PlayerProfileStore store, ILevelCatalog catalog)
        {
            _store = store;
            _catalog = catalog;
        }

        public LevelResolution Apply(LevelSpec spec, LevelResult result, long nowUnix)
        {
            var p = _store.Profile;

            if (result.Success)
            {
                p.CompletedLevels ??= new System.Collections.Generic.HashSet<int>();
                p.CompletedLevels.Add(spec.LevelId);

                p.Coins += result.CoinsReward;
                p.Gems += result.GemsReward;

                // Progression: next level by catalog ordering.
                // Fallback: if catalog is missing, advance sequentially.
                if (_catalog != null && _catalog.TryGetNextAfter(spec.LevelId, out var next))
                    p.CurrentLevelIndex = next.LevelId;
                else if (p.CurrentLevelIndex == spec.LevelId)
                    p.CurrentLevelIndex = spec.LevelId + 1;

                _store.NotifyChanged();

                return new LevelResolution(true, false, result.CoinsReward, result.GemsReward, p.Life, p.CurrentLevelIndex);
            }

            // Failure consumes life
            if (p.Life > 0)
            {
                var wasMax = p.Life >= LifeRules.MaxLife;
                p.Life -= 1;
                if (p.Life < 0) p.Life = 0;

                // Start regen timer when dropping from max
                if (wasMax && p.Life < LifeRules.MaxLife)
                {
                    if (p.NextLifeRegenUnix <= 0)
                        p.NextLifeRegenUnix = nowUnix + LifeRules.RegenSeconds;
                }

                _store.NotifyChanged();
                return new LevelResolution(false, true, 0, 0, p.Life, p.CurrentLevelIndex);
            }

            return new LevelResolution(false, false, 0, 0, p.Life, p.CurrentLevelIndex);
        }
    }
}
