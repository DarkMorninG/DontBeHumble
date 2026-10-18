using DBH.Base;
using UnityEngine;

namespace DBH.Injection {
    [DefaultExecutionOrder(-9000)]
    public class SceneBootstrapper : DBHMono {
        private void Awake() {
            DependencyInjector.InjectScene(gameObject.scene);
        }
    }
}