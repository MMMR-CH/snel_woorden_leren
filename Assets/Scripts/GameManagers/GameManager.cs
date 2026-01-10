using MC.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SWL
{
    public enum WoordDataType
    {
        A2,
        B1,
    }

    public class GameManager : SingletonBehaviour<GameManager>
    {
        MainBus mainBus;
        public MainBus MainBus
        {
            get
            {
                if (mainBus == null)
                {
                    mainBus = new MainBus();
                    MainBus.CurrentWoordDataType = () => CurrentWoordDataType;
                    MainBus.WoordDatas = levelManager.WoordenData.WoordenA2;
                    MainBus.GetCurrentLetterSetFunc = localizationManager.GetCurrentLetterSet;
                    MainBus.OnPlayButtonSound = OnPlayButtonSound;
                    MainBus.SetWoordData = SetWoordData;
                }
                return mainBus;
            }
        }

        [SerializeField] LevelManager levelManager = null;
        [SerializeField] LocalizationManager localizationManager = null;

        WoordDataType CurrentWoordDataType = WoordDataType.A2;

        protected override void Awake()
        {
            base.Awake();

            InitializeGame();
        }


        void InitializeGame()
        {
        }

        void OnPlayButtonSound()
        {

        }

        void SetWoordData(WoordDataType type)
        {
            CurrentWoordDataType = type;
            MainBus.WoordDatas = type switch
            {
                WoordDataType.A2 => levelManager.WoordenData.WoordenA2,
                WoordDataType.B1 => levelManager.WoordenData.WoordenB1,
                _ => levelManager.WoordenData.WoordenA2
            };
        }
    }
}
