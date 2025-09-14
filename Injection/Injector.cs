using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DBH.Attributes;
using DBH.Exception;
using DBH.Injection.dto;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DBH.Injection {
    public class Injector {
        public static void InjectField<T>(object toBeInjected, HashSet<Injectable> injectables) {
            if (toBeInjected == null) return;
            foreach (var field in GetFieldsOf(toBeInjected)) {
                if (!HasAttribute<T>(field)) continue;
                if (HasAttribute<Prototype>(field)) {
                    field.SetValue(toBeInjected, BeanCreator.InstantiateBean(field.FieldType, injectables));
                } else {
                    if (InjectListField<T>(toBeInjected, injectables, field)) continue;

                    InjectSingleField<T>(toBeInjected, injectables, field);
                }
            }
        }

        private static bool InjectListField<T>(object toBeInjected, HashSet<Injectable> injectables, FieldInfo field) {
            if (!field.FieldType.IsGenericType || field.FieldType.GetGenericTypeDefinition() != typeof(List<>)) return false;
            var objects = GetInjectables(field.FieldType.GetGenericArguments()[0], injectables);
            var typedList = ConvertListToType<T>(field, objects);

            field.SetValue(toBeInjected, typedList);
            return true;
        }

        private static void InjectSingleField<T>(object toBeInjected, HashSet<Injectable> injectables, FieldInfo field) {
            var controller = GetInjectable(field.FieldType, injectables);
            if (controller == null) {
                throw new InjectionContextMissing(toBeInjected.GetType() +
                                                  " is Missing controller " +
                                                  field.FieldType +
                                                  " look up if it's present in the scene");
            }

            field.SetValue(toBeInjected, controller);
        }

        private static IList ConvertListToType<T>(FieldInfo field, List<object> objects) {
            var genericType = field.FieldType.GetGenericArguments()[0];
            var typedList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(genericType));
            foreach (var obj in objects) {
                typedList.Add(obj);
            }

            return typedList;
        }

        public static List<FieldInfo> GetFieldsWithAttribute<T>(object component, bool withInheritance = false) {
            return component == null
                ? null
                : GetFieldsOf(component, withInheritance).Where(HasAttribute<T>).ToList();
        }

        public static void InvokeWithAttribute<T>(object component) {
            if (component == null) return;
            foreach (var methodInfo in GetMethodsOf(component))
                if (HasAttribute<T>(methodInfo))
                    InvokeMethod<T>(component, methodInfo);
        }

        public static void InvokeMethod<T>(object component, MethodInfo methodInfo) {
            methodInfo.Invoke(component, null);
        }


        public static bool HasAttribute<T>(Object c) {
            if (c == null) {
                return false;
            }

            try {
                return c.GetType().GetCustomAttributes(typeof(T), true).Length > 0;
            }
            catch (System.Exception e) {
                Debug.Log(e.StackTrace);
            }

            return false;
        }

        public static bool HasAttribute<T>(Assembly c) {
            if (c == null) {
                return false;
            }

            try {
                return c.GetCustomAttributes(typeof(T), true).Length > 0;
            }
            catch (System.Exception e) {
                Debug.Log(e.StackTrace);
            }

            return false;
        }

        public static bool HasAttribute<T>(Type type) {
            try {
                return type.GetCustomAttributes(typeof(T), true).Length > 0;
            }
            catch (System.Exception e) {
                Debug.Log(e.StackTrace);
            }

            return false;
        }

        public static IEnumerable<Object> GetComponentsWithInterface(Type type, IEnumerable<Object> components) {
            return components.Where(component => HasInterface(component, type)).ToList();
        }

        public static bool HasInterface(object component, Type interfaceType) {
            try {
                return component.GetType().GetInterfaces().Contains(interfaceType);
            }
            catch (System.Exception e) {
                Debug.Log(e.StackTrace);
            }

            return false;
        }

        public static bool HasInterface(Type component, Type interfaceType) {
            try {
                return component.GetInterfaces().Contains(interfaceType);
            }
            catch (System.Exception e) {
                Debug.Log(e.StackTrace);
            }

            return false;
        }

        public static void SubscribeInterface(object component, IEnumerable<Object> components) {
            foreach (var methodInfo in component.GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)) {
                if (!HasAttribute<Subscribe>(methodInfo)) continue;

                var parameterType = methodInfo.GetParameters()[0].ParameterType;
                var interfaces = GetComponentsWithInterface(parameterType, components);
                foreach (var @interface in interfaces) {
                    object[] parameters = { @interface };
                    methodInfo.Invoke(component, parameters);
                }
            }
        }

        public static T[] GetAttributes<T>(MemberInfo ob) {
            return Array.ConvertAll(ob.GetCustomAttributes(typeof(T), true), item => (T)item);
        }

        public static T GetAttribute<T>(MemberInfo memberInfo) {
            return (T)Convert.ChangeType(memberInfo.GetCustomAttribute(typeof(T), true), typeof(T));
        }

        public static bool HasAttribute<T>(MemberInfo member) {
            try {
                return GetAttributes<T>(member).Length > 0;
            }
            catch (System.Exception e) {
                Debug.Log(e.StackTrace);
            }

            return false;
        }

        public static IEnumerable<FieldInfo> GetFieldsOf(object component, bool withInheritance = false) {
            var fieldInfos = new List<FieldInfo>();
            fieldInfos = GetFields(component.GetType(), fieldInfos, withInheritance);
            return fieldInfos;
        }

        public static IEnumerable<MethodInfo> GetMethodsOf(object component) {
            var methodInfos = new List<MethodInfo>();
            methodInfos = GetMethods(component.GetType(), methodInfos);
            return methodInfos;
        }

        public static IEnumerable<MethodInfo> GetMethodsOfOnlyBase(object component) {
            if (component == null) {
                return Enumerable.Empty<MethodInfo>();
            }

            return component.GetType()
                .GetMethods(BindingFlags.NonPublic |
                            BindingFlags.Instance |
                            BindingFlags.Public);
        }

        private static List<FieldInfo> GetFields(Type type, List<FieldInfo> fieldInfos, bool withInheritance = false) {
            if (type == typeof(Object) || type == typeof(MonoBehaviour) || type == null) return fieldInfos;
            if (withInheritance) {
                fieldInfos.AddRange(type.GetFields(BindingFlags.NonPublic |
                                                   BindingFlags.Instance |
                                                   BindingFlags.Static |
                                                   BindingFlags.Public |
                                                   BindingFlags.FlattenHierarchy));
            } else {
                fieldInfos.AddRange(type.GetFields(BindingFlags.NonPublic |
                                                   BindingFlags.Instance |
                                                   BindingFlags.Static |
                                                   BindingFlags.Public));
            }

            GetFields(type.BaseType, fieldInfos, withInheritance);

            return fieldInfos;
        }

        private static List<MethodInfo> GetMethods(Type type, List<MethodInfo> methodInfos) {
            if (type == typeof(Object) || type == typeof(MonoBehaviour) || type == null) return methodInfos;
            methodInfos.AddRange(type.GetMethods(BindingFlags.NonPublic |
                                                 BindingFlags.Instance |
                                                 BindingFlags.Public));
            GetMethods(type.BaseType, methodInfos);

            return methodInfos;
        }

        private static object GetInjectable(Type type, HashSet<Injectable> injectables) {
            if (type.IsInterface)
                foreach (var injectable in injectables) {
                    var controller = injectable.Inject;
                    if (controller.GetType().GetInterfaces().Contains(type)) {
                        return controller;
                    }
                }

            foreach (var injectable in injectables) {
                var controller = injectable.Inject;
                if (controller.GetType() == type) {
                    return Convert.ChangeType(controller, type);
                }
            }

            return null;
        }

        private static List<object> GetInjectables(Type type, HashSet<Injectable> injectables) {
            var objects = new List<object>();
            if (type.IsInterface) {
                foreach (var injectable in injectables) {
                    var controller = injectable.Inject;
                    if (controller.GetType().GetInterfaces().Contains(type)) {
                        objects.Add(controller);
                    }
                }
            } else {
                foreach (var injectable in injectables) {
                    var controller = injectable.Inject;
                    if (controller.GetType() == type) {
                        objects.Add(Convert.ChangeType(controller, type));
                    }
                }
            }

            return objects;
        }


        public static bool HasInjectableWithInterface(Type interfaceToFind, IEnumerable<Injectable> controllers) {
            return controllers.Any(injectable => HasInterface(injectable.Inject, interfaceToFind));
        }

        public static bool HasInjectableWithInterface(Type interfaceToFind, IEnumerable<Type> controllers) {
            return controllers.Any(injectable => HasInterface(injectable, interfaceToFind));
        }

        public static Injectable GetInjectableWithInterface(Type interfaceToFind, IEnumerable<Injectable> controllers) {
            var controllerWithInterface =
                controllers.First(injectable => HasInterface(injectable.Inject, interfaceToFind));
            return controllerWithInterface;
        }
    }
}