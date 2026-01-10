using MC.Modules.Keyboard;
using MC.Utility;
using System;
using UnityEngine;

namespace SWL
{
    /// <summary>
    /// Manages localization settings and resources.
    /// </summary>
    /// <remarks>
    /// This class is responsible for handling localization-related tasks such as loading language files,
    /// switching languages, and providing localized strings to other parts of the application.
    /// </remarks>
    /// 

    [Serializable]
    public enum SWL_LanguagesEnum
    {
        Dutch = 0,
        English = 1,
        German = 2,
        French = 3,
        Spanish = 4,
        Turkish = 5,
    }


    public class LocalizationManager : MonoBehaviour
    {
        [SerializeField] SerializableDictionary<SWL_LanguagesEnum, LetterSet> LettersetsByLanguage;

        public static SWL_LanguagesEnum CurrentLanguage 
        {
            get => (SWL_LanguagesEnum)PlayerPrefs.GetInt("chosen_language", (int)SWL_LanguagesEnum.English);
            private set => PlayerPrefs.SetInt("chosen_language", (int)value);
        }

        public LetterSet GetCurrentLetterSet() => GetLettersetByLanguage(CurrentLanguage);

        LetterSet GetLettersetByLanguage(SWL_LanguagesEnum language)
        {
            if (LettersetsByLanguage.Dictionary.TryGetValue(language, out var letterSet))
            {
                return letterSet;
            }
            else
            {
                Debug.LogError($"LetterSet for language {language} not found.");
                return null;
            }
        }

        
    }
}

