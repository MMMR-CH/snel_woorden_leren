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
    public sealed class LevelCatalogSO : ScriptableObject
    {
        public List<LevelSpec> Levels = new();

        public bool TryGet(int levelId, out LevelSpec spec)
        {
            for (int i = 0; i < Levels.Count; i++)
            {
                if (Levels[i].LevelId == levelId)
                {
                    spec = Levels[i];
                    return true;
                }
            }
            spec = default;
            return false;
        }

        static string SceneName(LevelType key) => key switch
        {
            LevelType.Match_Words_Images => "Level_MatchWordsImages",
            LevelType.Match_Words_Sentences => "Level_MatchWordsSentences",
            LevelType.Choose_Words_Image_Text => "Level_ChooseWordsImageText",
            LevelType.Crossword_Puzzle => "Level_Crossword",
            LevelType.Fill_In_The_Blank => "Level_FillInTheBlank",
            LevelType.Unique => "Please set scene name maunally",
            _ => throw new ArgumentOutOfRangeException()
        };

#if UNITY_EDITOR
        void OnValidate()
        {
            // Update scene names
            for (int i = 0; i < Levels.Count; i++)
            {
                string sceneName = SceneName(Levels[i].Type);
                if (string.IsNullOrEmpty(sceneName))
                {
                    Debug.LogError($"SceneName for {Levels[i].Type} is empty.");
                    continue;
                }

                // update scene name             
                Levels[i] = new LevelSpec(
                    Levels[i].LevelId,
                    Levels[i].Type,
                    sceneName,  // for unique levels must be manual,
                    Levels[i].Difficulty,
                    Levels[i].Seed
                );

                // Verify name from build list
                bool sceneFound = false;
                var scenes = EditorBuildSettings.scenes;
                for (int j = 0; j < scenes.Length; j++)
                {
                    var scene = scenes[j];
                    string name = Path.GetFileNameWithoutExtension(scene.path);
                    if (name == sceneName)
                    {
                        sceneFound = true;
                        break;
                    }
                }

                if (!sceneFound)
                {
                    Debug.LogError($"Scene {sceneName} not found in build list.");
                    continue;
                }
            }
        }
#endif
    }
}
