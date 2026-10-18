using DBH.Base;
using DBH.Injection;
using UnityEngine;

namespace DBH.Util {
    [DefaultExecutionOrder(-10001)]
    public class DDOL : DBHMono {
        private static DDOL _instance;

        [SerializeField]
        private DependencyInjector dependencyInjector;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public void Init() {
            if (_instance == null) {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                dependencyInjector.StartUp();
            } else if (_instance != this) {
                Destroy(gameObject);
            }
        }
    }
}