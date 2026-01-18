using UnityEngine;
using SWL.App;
using SWL.App.Ports;
using SWL.Presentation.Controllers;
using SWL.Infrastructure.Save;
using SWL.Infrastructure.Time;
using SWL.App.UseCases;
using SWL.App.UseCases.DailyGift;
using SWL.App.UseCases.League;
using SWL.App.UseCases.Levels;
using SWL.App.UseCases.Shop;
using SWL.App.UseCases.Words;
using SWL.Features.Levels;
using SWL.Content.Levels;
using SWL.Content.Words;
using SWL.Infrastructure.Firebase;
using SWL.Infrastructure.RemoteConfig;
using SWL.Infrastructure.IAP;
using SWL.Infrastructure.Leaderboards;
using SWL.Infrastructure.Localization;
using SWL.Infrastructure.Connectivity;

namespace SWL.Composition
{
    public sealed class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private HUDPresenter hudPresenter;
        [SerializeField] private LifeRegenTicker lifeRegenTicker;
        [SerializeField] private LevelFlowController levelFlow;
        [Header("Catalogs")]
        [SerializeField] private LevelCatalogSO levelCatalog;
        [SerializeField] private WordCatalogSO wordCatalog;

        private PlayerProfileStore _profileStore;

        private void Awake()
        {
            IPlayerSave playerSave = new LocalJsonPlayerSave(); // Infrastructure implementasyonu
            _profileStore = new PlayerProfileStore(playerSave);  // App

            // --- Infrastructure services (stubs for now)
            ITimeService time = new DeviceTimeService();
            var analytics = new DebugAnalytics();
            var remoteConfig = new StubRemoteConfig();
            var iap = new StubIapService();
            var leaderboards = new StubLeaderboardService();
            var localization = new StubLocalizationService();
            var connectivity = new UnityConnectivityService();

            // Basic init
            analytics.SetUserId(_profileStore.Profile.PlayerId);
            remoteConfig.FetchAndActivate();
            iap.Initialize();
            // (connectivity.Poll is optional; caller can poll via a MonoBehaviour if needed)

            // --- Presentation wiring (optional)
            if (hudPresenter != null)
                hudPresenter.Construct(_profileStore);

            var tickLife = new TickLifeRegenUseCase(_profileStore);
            if (lifeRegenTicker != null)
                lifeRegenTicker.Construct(_profileStore, tickLife, time);

            // --- Level flow
            var resolver = new ResolveLevelResultUseCase(_profileStore, levelCatalog);
            if (levelFlow != null)
                levelFlow.Construct(_profileStore, resolver, time);

            // --- Pre-create use cases so they're ready for presenters later (scene wiring step)
            _ = new GetNextMainLevelUseCase(_profileStore, levelCatalog);
            _ = new GetRoadmapProgressUseCase(_profileStore, levelCatalog);
            _ = new GetDailyGiftStateUseCase(_profileStore);
            _ = new ClaimDailyGiftUseCase(_profileStore);
            _ = new PurchaseProductUseCase(_profileStore, iap, remoteConfig);
            _ = new RestorePurchasesUseCase(_profileStore, iap);
            _ = new GetLeagueWindowUseCase(_profileStore, leaderboards);
            _ = new GetWordsProgressUseCase(_profileStore);
            _ = new UnlockWordsUseCase(_profileStore);
            _ = new MarkWordLearnedUseCase(_profileStore);
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
