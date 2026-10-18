using DBH.Base;
using UnityEngine;

namespace DBH.Injection {
    [DefaultExecutionOrder(-10000)]
    public class SceneBootstrapper : DBHMono {
        private void Awake() {
            DependencyInjector.InjectScene(gameObject.scene);
        }
    }
}