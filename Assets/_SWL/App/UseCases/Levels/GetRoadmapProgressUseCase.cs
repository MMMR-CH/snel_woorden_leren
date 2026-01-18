using SWL.Core.Domain.Levels;

namespace SWL.App.UseCases.Levels
{
    public sealed class GetRoadmapProgressUseCase
    {
        private readonly PlayerProfileStore _store;
        private readonly ILevelCatalog _catalog;

        public GetRoadmapProgressUseCase(PlayerProfileStore store, ILevelCatalog catalog)
        {
            _store = store;
            _catalog = catalog;
        }

        public RoadmapProgress Get()
        {
            var p = _store.Profile;
            var total = _catalog?.Count ?? 0;
            var completed = p.CompletedLevels?.Count ?? 0;
            var next = p.CurrentLevelIndex;
            return new RoadmapProgress(completed, total, next);
        }
    }
}
