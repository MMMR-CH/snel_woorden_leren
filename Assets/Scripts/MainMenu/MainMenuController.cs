using MC.Modules.Tabsystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SWL
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] MainMenuPages mainMenuPages;


        void Awake()
        {
            Init();

        }

        void Init()
        {
            mainMenuPages.Init(OnPlayButtonSound, null, null);
        }

        void OnPlayButtonSound()
        {

        }
    }
}
