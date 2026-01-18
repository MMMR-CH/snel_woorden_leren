using UnityEngine;
using SWL.Core.Domain.Levels;

namespace SWL.Features.Levels
{
    public sealed class LevelSceneEntry : MonoBehaviour
    {
        [Header("Optional: If set, use this runner (unique scenes)")]
        [SerializeField] private MonoBehaviour runnerComponent;

        [Header("Optional: If runnerComponent is null, spawn from registry")]
        [SerializeField] private LevelRegistrySO registry;
        [SerializeField] private Transform runnerParent;

        private GameObject _spawned;
        private ILevelRunner Runner => runnerComponent as ILevelRunner;

        public ILevelRunner CreateRunner(LevelSpec spec)
        {
            // 1) Unique scene: runner already placed in scene
            if (Runner != null) return Runner;

            // 2) Spawn from registry
            if (registry == null)
            {
                Debug.LogError("LevelSceneEntry: registry is null and runnerComponent not set.");
                return null;
            }

            if (!registry.TryGet(spec.Type, out var prefab) || prefab == null)
            {
                Debug.LogError($"LevelSceneEntry: No prefab registered for type {spec.Type}");
                return null;
            }

            var parent = runnerParent != null ? runnerParent : transform;
            _spawned = Instantiate(prefab, parent);

            // Find the first MonoBehaviour that implements ILevelRunner
            runnerComponent = null;
            var all = _spawned.GetComponentsInChildren<MonoBehaviour>(true);
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] is ILevelRunner)
                {
                    runnerComponent = all[i];
                    break;
                }
            }

            if (Runner == null)
            {
                Debug.LogError("Spawned prefab does not implement ILevelRunner.");
                return null;
            }

            return Runner;
        }

        public void StartLevel(LevelSpec spec)
        {
            var r = Runner;
            if (r == null)
            {
                Debug.LogError("LevelSceneEntry: Runner is null. Call CreateRunner(spec) first.");
                return;
            }

            r.StartLevel(spec);
        }

        private void OnDestroy()
        {
            // clean up spawned prefab when scene unloads
            if (_spawned != null) Destroy(_spawned);
            _spawned = null;
        }
    }
}
