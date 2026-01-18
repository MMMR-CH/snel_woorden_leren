using UnityEngine;
using SWL.App;
using SWL.App.Ports;
using SWL.Presentation.Controllers;
using SWL.Infrastructure.Save;
using SWL.Infrastructure.Time;
using SWL.App.UseCases;
using SWL.Features.Levels;
using SWL.Content.Levels;

namespace SWL.Composition
{
    public sealed class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private HUDPresenter hudPresenter;
        [SerializeField] private LifeRegenTicker lifeRegenTicker;
        [SerializeField] private LevelFlowController levelFlow;
        [SerializeField] private LevelCatalogSO catalog; //TEST

        private PlayerProfileStore _profileStore;

        private void Awake()
        {
            IPlayerSave playerSave = new LocalJsonPlayerSave(); // Infrastructure implementasyonu
            _profileStore = new PlayerProfileStore(playerSave);  // App

            // Presentation wiring
            hudPresenter.Construct(_profileStore);

            // time + usecase
            ITimeService time = new DeviceTimeService();
            var tickLife = new TickLifeRegenUseCase(_profileStore);
            lifeRegenTicker.Construct(_profileStore, tickLife, time);

            var consumeLife = new ConsumeLifeUseCase(_profileStore);
            var grantReward = new GrantLevelRewardUseCase(_profileStore);
            levelFlow.Construct(_profileStore,consumeLife, grantReward);

            // Test: direct Level 1 
            if (catalog != null && catalog.TryGet(1, out var spec))
                levelFlow.StartLevel(spec);
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause) _profileStore.Save();
        }
        private void OnApplicationQuit()
        {
            _profileStore.Save();
        }
    }
}
