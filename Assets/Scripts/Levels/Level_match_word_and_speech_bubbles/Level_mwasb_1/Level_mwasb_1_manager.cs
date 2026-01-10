using PalaGames.CameraManagement;
using SWL;
using UnityEngine;

namespace SnelWoordenLeren.Levels
{
    public class Level_mwasb_1_manager : MonoBehaviour
    {
        [SerializeField] LevelController _levelController;

        [Space]
        [Header("Camera Size Fitter")]              
        [SerializeField] private ContentBoundsSceneContainer _contentBoundsSceneController;
        [SerializeField] private ContentBoundsUIContainer _contentBoundsUIController;
        [SerializeField] private CameraHelper _cameraHelper;
        CameraSizeFitter _cameraSizeFitter;

        [Space]
        [Header("Level UI Controllers")]
        [SerializeField] private LevelWordFrameController _levelWordFrameController;

        void Awake()
        {
            RegisterLevelInitEvents();

            _levelController.InitLevel();
        }

        void RegisterLevelInitEvents()
        {
            _levelController.OnLevelInitStarted.AddListener(AdjustCamera);
            _levelController.OnLevelInitCompleted.AddListener(PrepareWordFrameContents);
        }

        [InspectorButton("Adjust Camera Size and Position")]
        async void AdjustCamera()
        {
            // Create camera size fitter
            if (_cameraSizeFitter == null)
            {
                _cameraSizeFitter = new CameraSizeFitter(
                contentBoundsController: _contentBoundsSceneController,
                contentBoundsUIController: _contentBoundsUIController,
                cameraHelper: _cameraHelper,
                adjustOnStart: false
                );
            }
            _cameraSizeFitter.AdjustCamera();
        }

        void PrepareWordFrameContents()
        {
            WoordData[] woordenData = new WoordData[]
            {
                new WoordData { WOORD = "Kat" },
                new WoordData { WOORD = "Hond" },
                new WoordData { WOORD = "Vis" },
                new WoordData { WOORD = "Vogel" },
                new WoordData { WOORD = "Muis" }
            };

            _levelWordFrameController.Init(woordenData);
        }
    }
}
