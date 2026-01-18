using UnityEngine;
using UnityEngine.SceneManagement;
using SWL.App;
using SWL.App.UseCases;
using SWL.Core.Domain.Levels;

namespace SWL.Features.Levels
{
    public sealed class LevelFlowController : MonoBehaviour
    {
        [SerializeField] private string mainMenuSceneKey = "MainMenu";

        private ConsumeLifeUseCase _consumeLife;
        private GrantLevelRewardUseCase _grantReward;
        private PlayerProfileStore _store;

        private LevelSpec _pendingSpec;
        private ILevelRunner _activeRunner;

        public void Construct(
            PlayerProfileStore store,
            ConsumeLifeUseCase consumeLife,
            GrantLevelRewardUseCase grantReward)
        {
            _store = store;
            _consumeLife = consumeLife;
            _grantReward = grantReward;
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void StartLevel(LevelSpec spec)
        {
            if (_consumeLife == null || _grantReward == null || _store == null)
            {
                Debug.LogError("LevelFlowController not constructed.");
                return;
            }

            if (!_consumeLife.CanStart())
            {
                Debug.Log("No lives left -> show OutOfLives UI (later).");
                return;
            }

            if (string.IsNullOrWhiteSpace(spec.SceneKey))
            {
                Debug.LogError("LevelSpec.SceneKey is empty. (Rule: always open with SceneKey)");
                return;
            }

            _pendingSpec = spec;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadSceneAsync(spec.SceneKey, LoadSceneMode.Single);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            var entry = Object.FindFirstObjectByType<LevelSceneEntry>();
            if (entry == null)
            {
                Debug.LogError("No LevelSceneEntry found in loaded level scene.");
                return;
            }

            _activeRunner = entry.CreateRunner(_pendingSpec);
            if (_activeRunner == null) return;

            _activeRunner.Finished += OnLevelFinished;

            // Start AFTER subscribing
            entry.StartLevel(_pendingSpec);
        }


        private void OnLevelFinished(LevelResult result)
        {
            if (_activeRunner != null)
                _activeRunner.Finished -= OnLevelFinished;

            if (!result.Success)
            {
                // ✅ Life only on FAIL
                _consumeLife.ConsumeOnFail();
            }
            else
            {
                _grantReward.Grant(result.CoinsReward, result.GemsReward);

                // simple progression, for now
                _store.Profile.CurrentLevelIndex++;
                _store.NotifyChanged();
            }

            if (_activeRunner != null)
                _activeRunner.Dispose();
            _activeRunner = null;

            SceneManager.LoadSceneAsync(mainMenuSceneKey, LoadSceneMode.Single);
        }
    }
}
