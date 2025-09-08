using System.Collections.Generic;
using DBH.Attributes;
using DBH.Injection;
using DBH.Injection.dto;
using UnityEngine;

namespace DBH.Adapter {
    public class InjectionContext {
        private readonly HashSet<Injectable> _controllersAndBeans;

        public HashSet<Injectable> ControllersAndBeans => _controllersAndBeans;

        public InjectionContext(HashSet<Injectable> controllersAndBeans) {
            _controllersAndBeans = controllersAndBeans;
        }

        public IEnumerable<T> FindResources<T>() where T : Object {
            return Resources.FindObjectsOfTypeAll<T>();
        }

        public void InjectField(object toInject) {
            Injector.InjectField<Grab>(toInject, _controllersAndBeans);
        }
    }
}