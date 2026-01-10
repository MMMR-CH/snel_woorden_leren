using ExtendedUnityEventSystem;
using UnityEngine;
using UnityEngine.Events;

namespace SnelWoordenLeren.Levels
{
    public class LevelController : MonoBehaviour
    {
        #region EVENTS
        public ExtendedAsyncUnityEvent OnLevelInitStarted { get; } = new ExtendedAsyncUnityEvent();
        public ExtendedAsyncUnityEvent OnLevelInitCompleted { get; } = new ExtendedAsyncUnityEvent();
        public ExtendedAsyncUnityEvent OnLevelFinishStarted { get; } = new ExtendedAsyncUnityEvent();
        public ExtendedAsyncUnityEvent OnLevelFinishCompleted { get; } = new ExtendedAsyncUnityEvent();
        public ExtendedAsyncUnityEvent OnLevelDestroyed { get; } = new ExtendedAsyncUnityEvent();
        #endregion

        public async void InitLevel()
        {
            // await until OnLevelInitStarted listeners are done
            await OnLevelInitStarted.Invoke();

            // Additional initialization logic can be added here.
            
            await OnLevelInitCompleted.Invoke();
        }

        public async void FinishLevel()
        {
            // await until OnLevelFinishStarted listeners are done
            await OnLevelFinishStarted.Invoke();

            // Additional finishing logic can be added here.

            await OnLevelFinishCompleted.Invoke();
        }

        public async void DestroyLevel()
        {
            await OnLevelDestroyed.Invoke();
            // Additional destruction logic can be added here.
        }


    }
}
    