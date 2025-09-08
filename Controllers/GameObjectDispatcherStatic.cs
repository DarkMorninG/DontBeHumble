using DBH.Injection;
using UnityEngine;

namespace DBH.Controllers {
    public class GameObjectDispatcherStatic  {
        public static GameObject CreateGameObject(GameObject tobeCreated) {
            return DependencyInjector.InjectGameObject(Object.Instantiate(tobeCreated));
        }

        public static GameObject CreateGameObject(GameObject tobeCreated, Transform parent) {
            return DependencyInjector.InjectGameObject(Object.Instantiate(tobeCreated, parent));
        }

        public static GameObject CreateGameObject(GameObject tobeCreated,
            Transform parent,
            bool worldPositionStays) {
            return DependencyInjector.InjectGameObject(Object.Instantiate(tobeCreated, parent, worldPositionStays));
        }

        public static GameObject CreateGameObject(GameObject tobeCreated,
            Vector3 position,
            Quaternion rotation) {
            return DependencyInjector.InjectGameObject(Object.Instantiate(tobeCreated, position, rotation));
        }

        public static GameObject CreateGameObject(GameObject tobeCreated,
            Vector3 position,
            Quaternion rotation,
            Transform parent) {
            return DependencyInjector.InjectGameObject(Object.Instantiate(tobeCreated, position, rotation, parent));
        }

    }
}