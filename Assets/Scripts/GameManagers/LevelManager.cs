using MC.Modules.Keyboard;
using MC.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SWL
{
    public class LevelManager : MonoBehaviour
    {
        public enum LevelType
        {
            MainMenu,
            Match_Words_Images,
            Match_Words_Sentences,
            Choose_Words_Image_Text,
            Fill_In_The_Blank,
            Crossword_Puzzle,
        }

        static Dictionary<LevelType, string> LevelNames = new Dictionary<LevelType, string>
        {
            { LevelType.MainMenu, "MainMenu" },
            { LevelType.Fill_In_The_Blank, "Fill_In_The_Blank" },
        };
        
       


        [field: SerializeField] public WoordenScrObj WoordenData { get; private set; } = null;


        public static void LoadScene(string levelName)
        {
            SceneManager.LoadScene(levelName);
        }

        public static void LoadScene(LevelType levelType)
        {
            SceneManager.LoadScene(LevelNames[levelType]);
        }
    }
}
