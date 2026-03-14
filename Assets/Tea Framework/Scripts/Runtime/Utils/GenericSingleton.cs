using UnityEngine;

namespace TeaFramework {
    public abstract class GenericSingleton<T> : MonoBehaviour
        where T : MonoBehaviour {
        public static T Instance { get; protected set; }

        // For the sake of editor, for now set to public
        public bool keepSingleton = false;

        protected virtual void InitSingleton() {
            if (Instance != null) {
                Destroy(Instance.gameObject);
                return;
            }

            Instance = (T)(MonoBehaviour)this;
        }

        protected virtual void UnInitSingleton() { Instance = null; }

        protected virtual void Awake() {
            InitSingleton();
            if (keepSingleton) DontDestroyOnLoad(this);
        }

        protected virtual void OnDestroy() { UnInitSingleton(); }
    }
}