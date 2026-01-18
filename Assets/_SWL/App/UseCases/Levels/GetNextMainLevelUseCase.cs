using SWL.Core.Domain.Levels;

namespace SWL.App.UseCases.Levels
{
    public sealed class GetNextMainLevelUseCase
    {
        private readonly PlayerProfileStore _store;
        private readonly ILevelCatalog _catalog;

        public GetNextMainLevelUseCase(PlayerProfileStore store, ILevelCatalog catalog)
        {
            _store = store;
            _catalog = catalog;
        }

        public bool TryGetNext(out LevelSpec spec)
        {
            spec = default;
            if (_catalog == null || _catalog.Count <= 0) return false;

            var p = _store.Profile;

            // treat CurrentLevelIndex as "next main level id"
            if (p.CurrentLevelIndex > 0 && _catalog.TryGetById(p.CurrentLevelIndex, out var byId))
            {
                spec = byId;
                return true;
            }

            // fallback to first level in catalog order
            if (_catalog.TryGetByIndex(0, out var first))
            {
                p.CurrentLevelIndex = first.LevelId;
                _store.NotifyChanged();
                spec = first;
                return true;
            }

            return false;
        }
    }
}
