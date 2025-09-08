using UnityEngine;

namespace Controllers {
    public interface IComponentController {
        T GetComponentFromGameObject<T>(GameObject toAttachTo);
        T AddComponentInjected<T>(GameObject gameObjectForComponentToBeAdded) where T : Component;
    }
}