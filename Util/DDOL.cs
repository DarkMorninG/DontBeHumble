using Base;
using Injection;
using UnityEngine;

namespace Util
{
    public class DDOL : DBHMono {
        private static DDOL _instance;

        [SerializeField]
        private DependencyInjector dependencyInjector;


        public void Awake() {
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