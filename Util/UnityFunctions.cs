using System.Reflection;
using UnityEngine;

namespace Dont_Be_Humble.Util {
    public class UnityFunctions {
        public static void Start(Component component) {
            foreach (var methodInfo in component.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
                if (methodInfo.Name.Equals("Start"))
                    methodInfo.Invoke(component, null);
        }

        public static void OnDisable(Component component) {
            foreach (var methodInfo in component.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
                if (methodInfo.Name.Equals("OnDisable"))
                    methodInfo.Invoke(component, null);
        }

        public static void Update(Component component) {
            foreach (var methodInfo in component.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
                if (methodInfo.Name.Equals("Update"))
                    methodInfo.Invoke(component, null);
        }
    }
}