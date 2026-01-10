using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace MC.Utility
{
    public class InternetConnectionChecker : MonoBehaviour
    {
        public static InternetConnectionChecker Instance;

        private void Awake()
        {
            // singleton
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
            StartCoroutine(CheckConnectionRoutine());
        }

        private const string URL = "https://www.google.com";
        private const int RequestTimeOutInSeconds = 10;
        private const int ConnectionCheckIntervalInSeconds = 5;

        public static UnityEvent<bool> OnInternetConnectionChanged = new UnityEvent<bool>();

        private bool? _lastConnectionStatus;
        

        protected static bool? HasInternetConnection 
        { 
            get; 
            private set; 
        }

        [SerializeField] bool editorTestInternetConnectionON = false;
        static bool? internetConnectionON = null;
        public static bool InternetConnectionON
        {
            get
            {
                if (internetConnectionON == null)
                {
#if UNITY_EDITOR
                    if (Application.isPlaying) internetConnectionON = Instance.editorTestInternetConnectionON;
                    else internetConnectionON = true;
#else
                    internetConnectionON = Application.internetReachability != NetworkReachability.NotReachable;
#endif
                }
                return internetConnectionON.Value;
            }
            private set
            {
                if (internetConnectionON == value) return;
                internetConnectionON = value;
                OnInternetConnectionChanged.Invoke(internetConnectionON.Value);
            }
        }

        float timer = 0f;
        private void Update()
        {
            if(timer >= ConnectionCheckIntervalInSeconds)
            {
                timer -= ConnectionCheckIntervalInSeconds;
#if UNITY_EDITOR
                InternetConnectionON = editorTestInternetConnectionON;
#else
                InternetConnectionON = Application.internetReachability != NetworkReachability.NotReachable;
#endif
            }
            else
            {
                timer += Time.deltaTime;
            }
        }

        private IEnumerator CheckConnectionRoutine()
        {
            while (true)
            {
                StartCoroutine(CheckConnection(isConnected =>
                {
                    if (isConnected == _lastConnectionStatus) return;

                    HasInternetConnection = isConnected;
                    _lastConnectionStatus = isConnected;
                    OnInternetConnectionChanged?.Invoke(isConnected);
                }));

                yield return new WaitForSecondsRealtime(ConnectionCheckIntervalInSeconds);
            }

            IEnumerator CheckConnection(Action<bool> callback)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    callback(false);
                    yield break;
                }

                var request = UnityWebRequest.Head(URL);
                request.timeout = RequestTimeOutInSeconds;

                yield return request.SendWebRequest();

                callback(request.result != UnityWebRequest.Result.ConnectionError);
            }
        }
    }
}