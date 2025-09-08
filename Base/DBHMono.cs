using System;
using System.Linq;
using DBH.Attributes;
using DBH.Controllers;
using DBH.Injection;
using UnityEngine;
using Vault;

namespace DBH.Base {
#pragma warning disable 0649
    [InjectionScanned]
    public class DBHMono : MonoBehaviour {
        [Grab]
        protected IComponentController ComponentController;

        [Grab]
        protected IGameObjectDispatcher GameObjectDispatcher;

        [SerializeField]
        [HideInInspector]
        private string UUID = Guid.NewGuid().ToString();

        [SerializeField]
        [HideInInspector]
        private string gameObjectId;

        public string GameObjectId => gameObjectId;
        public string Uuid => UUID;

        [field: NonSerialized]
        public bool StartFinished { get; private set; }

        private void Reset() {
            UUID = Guid.NewGuid().ToString();
            if (gameObjectId.IsNotEmpty()) return;
            var currentGameObjectId = GetComponents<DBHMono>()
                .Select(mono => mono.gameObjectId)
                .FirstOrDefault(s => s.IsNotEmpty());
            gameObjectId = currentGameObjectId ?? Guid.NewGuid().ToString();
            OnReset();
        }

        private void Start() {
            OnStart();
            StartFinished = true;
        }

        public virtual void OnStart() {
        }

        public virtual void OnReset() {
        }

        protected T AddComponent<T>() where T : Component {
            return ComponentController.AddComponentInjected<T>(gameObject);
        }

        public static T AddComponentInjected<T>(GameObject gameObject) where T : Component {
            var addComponent = gameObject.AddComponent<T>();
            DependencyInjector.InjectFields(addComponent);
            return addComponent;
        }

        protected T AddComponent<T>(GameObject target) where T : Component {
            return ComponentController.AddComponentInjected<T>(target);
        }

        protected T GetOrAddComponent<T>() where T : Component {
            var componentFromGameObject = GetComponent<T>();
            return componentFromGameObject != null ? componentFromGameObject : AddComponent<T>();
        }

        protected bool IsPlaying() {
            return Application.isPlaying;
        }

        protected new T GetComponent<T>() {
            if (!Application.isPlaying) {
                return base.GetComponent<T>();
            }

            T component = default;
            if (ComponentController == null) {
                DependencyInjector.OnFinishedInjection += () => {
                    component = ComponentController.GetComponentFromGameObject<T>(gameObject);
                };
            } else {
                return ComponentController.GetComponentFromGameObject<T>(gameObject);
            }

            return component;
        }

        protected T GetComponent<T>(GameObject target) {
            T component = default;
            if (ComponentController == null) {
                DependencyInjector.OnFinishedInjection += () => {
                    component = ComponentController.GetComponentFromGameObject<T>(target);
                };
            } else {
                return ComponentController.GetComponentFromGameObject<T>(target);
            }

            return component;
        }

        protected void Destroy(GameObject gameObject) {
            if (GameObjectDispatcher == null) {
                if (Application.isPlaying) {
                    GameObject.Destroy(gameObject);
                } else {
                    DestroyImmediate(gameObject);
                }
            } else {
                if (Application.isPlaying) {
                    GameObjectDispatcher.Destroy(gameObject);
                } else {
                    GameObjectDispatcher.DestroyImmediate(gameObject);
                }
            }
        }

        protected GameObject CreateGameObject(GameObject tobeCreated) {
            return Application.isPlaying
                ? Instantiate(tobeCreated)
                : GameObjectDispatcher.CreateGameObject(tobeCreated);
        }

        protected GameObject CreateGameObject(GameObject tobeCreated, Transform parent) {
            return Application.isPlaying
                ? GameObjectDispatcher.CreateGameObject(tobeCreated, parent)
                : Instantiate(tobeCreated, parent);
        }

        protected GameObject CreateGameObject(GameObject tobeCreated, Transform parent, bool worldPositionStays) {
            return Application.isPlaying
                ? GameObjectDispatcher.CreateGameObject(tobeCreated, parent, worldPositionStays)
                : Instantiate(tobeCreated, parent, worldPositionStays);
        }

        protected GameObject CreateGameObject(GameObject tobeCreated, Vector3 position, Quaternion rotation) {
            return Application.isPlaying
                ? GameObjectDispatcher.CreateGameObject(tobeCreated, position, rotation)
                : Instantiate(tobeCreated, position, rotation);
        }

        protected GameObject CreateGameObject(GameObject tobeCreated, Vector3 position) {
            return Application.isPlaying
                ? GameObjectDispatcher.CreateGameObject(tobeCreated, position, Quaternion.Euler(0, 0, 0))
                : Instantiate(tobeCreated, position, Quaternion.Euler(0, 0, 0));
        }

        protected GameObject CreateGameObject(GameObject tobeCreated,
            Vector3 position,
            Quaternion rotation,
            Transform parent) {
            return Application.isPlaying
                ? GameObjectDispatcher.CreateGameObject(tobeCreated, position, rotation, parent)
                : Instantiate(tobeCreated, position, rotation, parent);
        }

        protected GameObject CreateGameObject(GameObject tobeCreated,
            Vector3 position,
            Transform parent) {
            return Application.isPlaying
                ? GameObjectDispatcher.CreateGameObject(tobeCreated, position, Quaternion.Euler(0, 0, 0), parent)
                : Instantiate(tobeCreated, position, Quaternion.Euler(0, 0, 0), parent);
        }
    }
}