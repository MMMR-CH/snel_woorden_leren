using MC.Modules.Tabsystem;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SWL
{
    public class FITBMenuContainer : PageContainer
    {
        [field: SerializeField] public Button PlayButton_A2 { get; private set; }
        [field: SerializeField] public Button PlayButton_B1 { get; private set; }

        private void Awake()
        {
            // assign play button event
            PlayButton_A2.onClick.AddListener(OnPlayButtonClickA2);
            PlayButton_B1.onClick.AddListener(OnPlayButtonClickB1);
        }

        void OnPlayButtonClickA2()
        {
            // set current woord data type
            GameManager.Instance.MainBus.SetWoordData.Invoke(WoordDataType.A2);

            // load game scene
            LevelManager.LoadScene(LevelManager.LevelType.Fill_In_The_Blank);
        }

        void OnPlayButtonClickB1()
        {
            // set current woord data type
            GameManager.Instance.MainBus.SetWoordData.Invoke(WoordDataType.B1);

            // load game scene
            LevelManager.LoadScene(LevelManager.LevelType.Fill_In_The_Blank);
        }
    }
}

