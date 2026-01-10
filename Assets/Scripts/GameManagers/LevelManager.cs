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
            BetekenisLevel,
        }

        static Dictionary<LevelType, string> LevelNames = new Dictionary<LevelType, string>
        {
            { LevelType.MainMenu, "MainMenu" },
            { LevelType.BetekenisLevel, "BetekenisLevel" },
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
