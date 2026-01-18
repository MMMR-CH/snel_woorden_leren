using System;

namespace SWL.Core.Domain.Levels
{
    [Serializable]
    public struct LevelSpec
    {
        public int LevelId;
        public LevelType Type;
        public string SceneKey;

        public int Difficulty;
        public int Seed;          // for crossword / variation

        public LevelSpec(int levelId, LevelType type, string sceneKey, int difficulty, int seed)
        {
            LevelId = levelId;
            Type = type;
            SceneKey = sceneKey;
            Difficulty = difficulty;
            Seed = seed;
        }
    }
}
