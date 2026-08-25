using UnityEngine;

namespace SeungyungLib.Core.MonoSingleton
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance 
        {
            get
            {
                if (_instance == null)
                    _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                {
                    GameObject singletonObj = new GameObject();
                    singletonObj.AddComponent<T>();
                    singletonObj.name = typeof(T).Name;

                    DontDestroyOnLoad(singletonObj);
                }

                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == this)
            {
                DontDestroyOnLoad(gameObject);
                return;
            }

            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            OnAwake();
        }

        protected virtual void OnAwake() { }
    }
}