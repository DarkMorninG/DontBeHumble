using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dont_Be_Humble.Controllers {
    public class ComponentMockController : IComponentController {
        private static readonly Dictionary<Type, object> Components = new Dictionary<Type, object>();

        private static readonly List<object> AddedComponents = new List<object>();
        private GameObject _gameObject;

        public ComponentMockController(GameObject gameObject) {
            _gameObject = gameObject;
        }

        public T GetComponentFromGameObject<T>(GameObject toAttachTo) {
            Components.TryGetValue(typeof(T), out var component);
            return (T) component;
        }

        public T AddComponentInjected<T>(GameObject gameObjectForComponentToBeAdded) where T : Component {
            var addedComponent = gameObjectForComponentToBeAdded.AddComponent<T>();
            AddedComponents.Add(addedComponent);
            return addedComponent;
        }

        public T GetComponentFromGameObject<T>(object component) {
            if (Components.ContainsKey(typeof(T))) return (T) component;
            Components.Add(typeof(T), component);
            return (T) component;
        }

        public static void AddMock(Type type, object componentMock) {
            if (!Components.ContainsKey(type)) Components.Add(type, componentMock);
        }

        public static void ClearMocksDictionary() {
            Components.Clear();
            AddedComponents.Clear();
        }


        public static List<object> GetAddedComponents() {
            return AddedComponents;
        }
    }
}