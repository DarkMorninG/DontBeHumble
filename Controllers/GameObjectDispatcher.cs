using DBH.Attributes;
using DBH.Base;
using DBH.Injection;
using UnityEngine;

namespace DBH.Controllers {
    [Controller]
    public class GameObjectDispatcher : DBHMono, IGameObjectDispatcher {
        public new GameObject CreateGameObject(GameObject tobeCreated) {
            return DependencyInjector.InjectGameObject(Instantiate(tobeCreated));
        }

        public new GameObject CreateGameObject(GameObject tobeCreated, Transform parent) {
            return DependencyInjector.InjectGameObject(Instantiate(tobeCreated, parent));
        }

        public new GameObject CreateGameObject(GameObject tobeCreated,
            Transform parent,
            bool worldPositionStays) {
            return DependencyInjector.InjectGameObject(Instantiate(tobeCreated, parent, worldPositionStays));
        }

        public new GameObject CreateGameObject(GameObject tobeCreated,
            Vector3 position,
            Quaternion rotation) {
            return DependencyInjector.InjectGameObject(Instantiate(tobeCreated, position, rotation));
        }

        public new GameObject CreateGameObject(GameObject tobeCreated,
            Vector3 position,
            Quaternion rotation,
            Transform parent) {
            return DependencyInjector.InjectGameObject(Instantiate(tobeCreated, position, rotation, parent));
        }

        public new void Destroy(GameObject gameObject) {
            Object.Destroy(gameObject);
        }

        public void DestroyImmediate(GameObject gameObject) {
            Object.DestroyImmediate(gameObject);
        }
    }
}