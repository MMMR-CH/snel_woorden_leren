using System;
using System.Collections.Generic;
using System.IO;
using SWL.Core.Domain.Levels;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace SWL.Content.Levels
{
    [CreateAssetMenu(menuName = "SWL/Levels/Level Catalog", fileName = "LevelCatalog")]
    public sealed class LevelCatalogSO : ScriptableObject, ILevelCatalog
    {
        public List<LevelSpec> Levels = new();

        public int Count => Levels?.Count ?? 0;

        public IEnumerable<LevelSpec> All => Levels;

        public bool TryGetById(int levelId, out LevelSpec spec)
        {
            if (Levels != null)
            {
                for (int i = 0; i < Levels.Count; i++)
                {
                    if (Levels[i].LevelId == levelId)
                    {
                        spec = Levels[i];
                        return true;
                    }
                }
            }
            spec = default;
            return false;
        }

        public bool TryGetByIndex(int index, out LevelSpec spec)
        {
            if (Levels != null && index >= 0 && index < Levels.Count)
            {
                spec = Levels[index];
                return true;
            }
            spec = default;
            return false;
        }

        public int IndexOf(int levelId)
        {
            if (Levels == null) return -1;
            for (int i = 0; i < Levels.Count; i++)
            {
                if (Levels[i].LevelId == levelId)
                    return i;
            }
            return -1;
        }

        public bool TryGetNextAfter(int levelId, out LevelSpec next)
        {
            var idx = IndexOf(levelId);
            if (idx < 0) { next = default; return false; }
            return TryGetByIndex(idx + 1, out next);
        }

        static string SceneName(LevelType key) => key switch
        {
            LevelType.Match_Words_Images => "Level_MatchWordsImages",
            LevelType.Match_Words_Sentences => "Level_MatchWordsSentences",
            LevelType.Choose_Words_Image_Text => "Level_ChooseWordsImageText",
            LevelType.Crossword_Puzzle => "Level_Crossword",
            LevelType.Fill_In_The_Blank => "Level_FillInTheBlank",
            LevelType.Unique => null, // must be set manually
            _ => throw new ArgumentOutOfRangeException()
        };

#if UNITY_EDITOR
        void OnValidate()
        {
            if (Levels == null) return;

            for (int i = 0; i < Levels.Count; i++)
            {
                var spec = Levels[i];

                var autoScene = SceneName(spec.Type);

                // Non-unique types get auto-scene
                if (!string.IsNullOrEmpty(autoScene))
                {
                    spec.SceneKey = autoScene;
                }
                else
                {
                    // Unique levels must set scene key manually
                    if (string.IsNullOrWhiteSpace(spec.SceneKey))
                        Debug.LogWarning($"Level {spec.LevelId} is Unique but SceneKey is empty. Please set it manually.");
                }

                Levels[i] = spec;

                // Verify name from build list for non-unique auto scene keys
                if (string.IsNullOrEmpty(autoScene))
                    continue;

                bool sceneFound = false;
                var scenes = EditorBuildSettings.scenes;
                for (int j = 0; j < scenes.Length; j++)
                {
                    var scene = scenes[j];
                    string name = Path.GetFileNameWithoutExtension(scene.path);
                    if (name == autoScene)
                    {
                        sceneFound = true;
                        break;
                    }
                }

                if (!sceneFound)
                    Debug.LogError($"Scene {autoScene} not found in build list.");
            }
        }
#endif
    }
}
