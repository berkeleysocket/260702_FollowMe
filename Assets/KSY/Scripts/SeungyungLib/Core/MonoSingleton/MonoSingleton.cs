using SeungyungLib.Core.CustomDebug;

using UnityEngine;

namespace SeungyungLib.Core.MonoSingleton
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _shuttingDown = false;

        private bool dontDestroyOnLoad = true;

        public static T Instance
        {
            get
            {
                if (_shuttingDown)
                {
                    DebugLogger.LogWarning($"[MonoSingleton] {typeof(T)} instance already destroyed on application quit. Returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindAnyObjectByType(typeof(T));

                        if (_instance == null)
                        {
                            GameObject singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<T>();
                            singletonObject.name = $"{typeof(T)} (MonoSingleton)";

                            DontDestroyOnLoad(singletonObject);
                        }
                    }

                    return _instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;

                if (dontDestroyOnLoad)
                {
                    transform.SetParent(null);
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (_instance != this)
            {
                DebugLogger.LogWarning($"[MonoSingleton] Duplicate instance of {typeof(T)} detected and destroyed on {gameObject.name}");
                Destroy(gameObject);
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _shuttingDown = true;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}