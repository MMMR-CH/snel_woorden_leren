namespace SWL.App.UseCases.Levels
{
    public readonly struct RoadmapProgress
    {
        public readonly int CompletedCount;
        public readonly int TotalCount;
        public readonly int NextLevelId;

        public RoadmapProgress(int completedCount, int totalCount, int nextLevelId)
        {
            CompletedCount = completedCount;
            TotalCount = totalCount;
            NextLevelId = nextLevelId;
        }

        public float Percent => TotalCount <= 0 ? 0f : (float)CompletedCount / TotalCount;
    }
}
