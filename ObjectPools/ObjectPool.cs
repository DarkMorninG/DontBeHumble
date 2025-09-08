using System;
using System.Collections.Generic;
using System.Linq;
using Dont_Be_Humble.Base;
using Dont_Be_Humble.Controllers;
using UnityEditor;
using UnityEngine;

namespace Dont_Be_Humble.ObjectPools {
    public class ObjectPool {
        private string uuid = Guid.NewGuid().ToString();

        private readonly HashSet<GameObject> pooledObject = new();

        private readonly GameObject prefab;
        public string Uuid => uuid;

        public ObjectPool(GameObject prefab) {
            this.prefab = prefab;
        }

        public ObjectPool(GameObject prefab, int startCount) {
            this.prefab = prefab;
            for (var i = 0; i < startCount; i++) {
                var gameObject = GameObjectDispatcherStatic.CreateGameObject(prefab);
                gameObject.SetActive(false);
                pooledObject.Add(gameObject);
            }
        }

        public bool HasAvailableItem() {
            return pooledObject.Any(o => !o.activeInHierarchy);
        }

        public GameObject GetItem(Transform parent = null) {
            if (!HasAvailableItem()) {
                var item = GameObjectDispatcherStatic.CreateGameObject(prefab, parent);
                pooledObject.Add(item);
                return item;
            }

            var gameObject = pooledObject.First(o => !o.activeInHierarchy);
            gameObject.SetActive(true);
            foreach (var dbhMono in gameObject.GetComponents<DBHMono>()) {
                dbhMono.OnStart();
            }

            if (parent != null) {
                var scale = gameObject.transform.localScale;
                gameObject.transform.SetParent(parent, false);
                gameObject.transform.localScale = scale;
            }

            return gameObject;
        }

        public static void ReturnToPool(GameObject item) {
            item.SetActive(false);
            item.transform.SetParent(null);
        }

        public void AddItem(GameObject gameObject) {
            pooledObject.Add(gameObject);
        }

        public bool BelongsToPool(GameObject gameObject) {
            return pooledObject.Contains(gameObject);
        }
    }
}