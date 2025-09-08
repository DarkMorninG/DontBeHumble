using System.Collections.Generic;
using System.Linq;
using DBH.Base;
using DBH.Controllers;
using UnityEngine;
using Vault.BetterCoroutine;

namespace DBH.ObjectPools {
    public class ObjectPooler : DBHMono {
        private static Dictionary<string, ObjectPool> objectPools = new();

        public static GameObject CreateFromPoolIfPossible(GameObject prefab, ObjectPool objectPool) {
            objectPools.TryAdd(objectPool.Uuid, objectPool);
            if (objectPool.HasAvailableItem()) {
                var item = objectPool.GetItem();
                item.SetActive(true);
                foreach (var dbhMono in item.GetComponents<DBHMono>()) {
                    dbhMono.OnStart();
                }

                return item;
            }

            var o = GameObjectDispatcherStatic.CreateGameObject(prefab);
            objectPool.AddItem(o);
            return o;
        }

        public static void PreGenerate(GameObject prefab, ObjectPool objectPool, int count) {
            objectPools.TryAdd(objectPool.Uuid, objectPool);
            Enumerable.Range(0, count)
                .ToList()
                .ForEach(i =>
                    UnityAsyncRuntime.Create(() => {
                        var created = GameObjectDispatcherStatic.CreateGameObject(prefab);
                        objectPool.AddItem(created);
                        ReturnToPool(created);
                    }));
        }


        public static void ReturnToPool(GameObject gameObject) {
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            gameObject.SetActive(false);
        }
    }
}