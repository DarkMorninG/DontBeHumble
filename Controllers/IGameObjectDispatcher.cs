using UnityEngine;

namespace DBH.Controllers {
    public interface IGameObjectDispatcher {
        GameObject CreateGameObject(GameObject tobeCreated);
        GameObject CreateGameObject(GameObject tobeCreated, Transform parent);
        GameObject CreateGameObject(GameObject tobeCreated, Transform parent, bool worldPositionStays);
        GameObject CreateGameObject(GameObject tobeCreated, Vector3 position, Quaternion rotation);
        GameObject CreateGameObject(GameObject tobeCreated, Vector3 position, Quaternion rotation, Transform parent);

        void Destroy(GameObject gameObject);
        void DestroyImmediate(GameObject gameObject);
    }
}