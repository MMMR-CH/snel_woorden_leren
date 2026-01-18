using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using SWL.App;
using SWL.App.Ports;
using SWL.App.UseCases.Levels;
using SWL.Core.Domain.Levels;

namespace SWL.Features.Levels
{
    public sealed class LevelFlowController : MonoBehaviour
    {
        [SerializeField] private string mainMenuSceneKey = "MainMenu";

        private PlayerProfileStore _store;
        private ResolveLevelResultUseCase _resolve;
        private ITimeService _time;

        private LevelSpec _pendingSpec;
        private ILevelRunner _activeRunner;

        public void Construct(
            PlayerProfileStore store,
            ResolveLevelResultUseCase resolve,
            ITimeService time)
        {
            _store = store;
            _resolve = resolve;
            _time = time;
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void StartLevel(LevelSpec spec)
        {
            if (_resolve == null || _store == null)
            {
                Debug.LogError("LevelFlowController not constructed.");
                return;
            }

            if (_store.Profile.Life <= 0)
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

            var entry = UnityEngine.Object.FindFirstObjectByType<LevelSceneEntry>();
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

            var now = _time != null ? _time.UtcNowUnixSeconds : DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _resolve.Apply(_pendingSpec, result, now);

            if (_activeRunner != null)
                _activeRunner.Dispose();
            _activeRunner = null;

            SceneManager.LoadSceneAsync(mainMenuSceneKey, LoadSceneMode.Single);
        }
    }
}
