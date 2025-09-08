using UnityEngine;

namespace Dont_Be_Humble.Controllers {
    public interface IComponentController {
        T GetComponentFromGameObject<T>(GameObject toAttachTo);
        T AddComponentInjected<T>(GameObject gameObjectForComponentToBeAdded) where T : Component;
    }
}