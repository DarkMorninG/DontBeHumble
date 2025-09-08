using Attributes;
using Injection;
using UnityEngine;

namespace Controllers {
    [Controller]
    public class ComponentController : MonoBehaviour, IComponentController {
        public T GetComponentFromGameObject<T>(GameObject toAttachTo) {
            return toAttachTo.GetComponent<T>();
        }

        public T AddComponentInjected<T>(GameObject gameObjectForComponentToBeAdded) where T : Component {
            var comp = gameObjectForComponentToBeAdded.AddComponent<T>();
            DependencyInjector.FullInjectIgnoreSubscribe(comp);
            return comp;
        }

        public new T GetComponent<T>() {
            throw new System.Exception("don't use getComponent directly use GetComponentFromGameObject instead");
        }

        public T GetComponent<T>(string s) {
            throw new System.Exception("don't use getComponent directly use GetComponentFromGameObject instead");
        }
    }
}