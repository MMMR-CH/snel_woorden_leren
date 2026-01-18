using System;
using SWL.Core.Domain.Levels;

namespace SWL.Features.Levels
{
    public interface ILevelRunner
    {
        void StartLevel(LevelSpec spec);
        event Action<LevelResult> Finished;
        void Dispose();
    }
}
