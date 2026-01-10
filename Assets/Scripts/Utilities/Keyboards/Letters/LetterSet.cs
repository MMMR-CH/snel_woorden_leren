using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MC.Modules.Keyboard
{
    /// <summary>
    /// Represents a set of letters used in the keyboard.
    /// </summary>
    /// <remarks>
    /// This class is a ScriptableObject that can be used to define a collection of letters.
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// LetterSet letterSet = ScriptableObject.CreateInstance&lt;LetterSet&gt;();
    /// </code>
    /// </example>
    /// </remarks>
    /// 

    [CreateAssetMenu(fileName = "LetterSet", menuName = "Scriptable Objects/LetterSet")]
    public class LetterSet : ScriptableObject
    {
        [ReadOnlyTextArea, SerializeField] string Note = $"Only add letter enum and run {nameof(UpdateLetterObjectsDictionary)} to update the list and the dictionary.";

        [field: SerializeField] public KeyboardLanguageEnum KeyboardLanguage { get; private set; }

        [field: SerializeField] public List<LetterObj> LetterObjectsList { get; private set; }

        [field: SerializeField] public SerializedDictionary<Letter, LetterObj> LetterObjectsDictionary { get; private set; } = new();

        [InspectorButton]
        public void UpdateLetterObjectsDictionary()
        {
            // foreach member of Letter enum, add a new LetterObj to LetterObjectsList
            LetterObjectsList = new List<LetterObj>();
            foreach (Letter letter in System.Enum.GetValues(typeof(Letter)))
            {
                LetterObjectsList.Add(new LetterObj(letter));
            }

            // update LetterObjectsDictionary if LetterObjectsList is modified
            if (LetterObjectsList != null && LetterObjectsList.Count > 0)
            {
                LetterObjectsDictionary.Clear();
                for (int i = 0; i < LetterObjectsList.Count; i++)
                {
                    var letterObj = LetterObjectsList[i];
                    if (!LetterObjectsDictionary.ContainsKey(letterObj.Letter))
                    {
                        LetterObjectsDictionary.Add(letterObj.Letter, letterObj);
                    }
                }
            }

            // save scriptable object asset
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }

    [Serializable]
    public enum KeyboardLanguageEnum
    {
        Dutch = 0,
        English = 1,
        German = 2,
        French = 3,
        Spanish = 4,
        Turkish = 5,
    }
}


