using Newtonsoft.Json;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MC.Utility
{
    [Serializable]
    public class LevelsWindow : EditorWindow
    {
        [JsonProperty, SerializeReference] public SceneAsset[] scenes;
        Vector2 sceollPos;
        SerializedObject so;

        [MenuItem("Window/LevelsWindow")]
        public static void Init()
        {
            EditorWindow.GetWindow<LevelsWindow>().Show();
        }

        private void OnEnable()
        {
            so = new SerializedObject(this);

            string path = PlayerPrefs.GetString("LevelsWindow");
            var paths = path.Split('-');
            scenes = new SceneAsset[paths.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                scenes[i] = AssetDatabase.LoadAssetAtPath<SceneAsset>(paths[i]);
            }
        }

        private void OnGUI()
        {
            so.Update();
            SerializedProperty stringsProperty = so.FindProperty("scenes");
            EditorGUILayout.PropertyField(stringsProperty, true); // True means show children
            so.ApplyModifiedProperties(); // Remember to apply modified properties

            EditorGUILayout.BeginVertical();
            sceollPos = EditorGUILayout.BeginScrollView(sceollPos, GUILayout.ExpandHeight(true));
            for (int i = 0; i < scenes.Length; i++)
            {
                if (scenes[i] != null && GUILayout.Button(scenes[i].name))
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scenes[i]), OpenSceneMode.Single);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void OnDisable()
        {
            string path = "";
            for (int i = 0; i < scenes.Length; i++)
            {
                if (i > 0) path += '-';
                path += AssetDatabase.GetAssetPath(scenes[i]);
            }
            PlayerPrefs.SetString("LevelsWindow", path);
        }
    }
}