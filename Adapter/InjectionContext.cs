using System.Collections.Generic;
using Dont_Be_Humble.Attributes;
using Dont_Be_Humble.Injection;
using UnityEngine;

namespace Dont_Be_Humble.Adapter {
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