using UnityEngine;

namespace MC.Utility
{
    /// <summary> 
    /// To access the heir by a static field "Instance".
    /// </summary>
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        [SerializeField] private bool dontDestroyOnLoad;

        private static T _instance;

        public static T Instance
        {
            get
            {
                if (!_instance) CreateNewInstance();
                return _instance;
            }
        }

        private static void CreateNewInstance()
        {
            _instance = FindFirstObjectByType<T>(findObjectsInactive: FindObjectsInactive.Include);

            if (!_instance)
            {
                Debug.LogError($"The singleton object could not found! : {typeof(T)}");
            }
        }

        protected virtual void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(this);
                Debug.LogWarning($"There are more than one singleton object! : {typeof(T)}. Destroyed!");
                return;
            }

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}