using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DBH.Attributes;
using DBH.Exception;
using DBH.Injection.dto;
using Vault;

namespace DBH.Injection {
    public static class BeanCreator {
        public static List<Injectable> InstantiateBeans(IEnumerable<Type> beanTypesInAssembly,
            IEnumerable<Injectable> injectables) {
            var currentInjectables = new HashSet<Injectable>(injectables);

            var typesInAssembly = beanTypesInAssembly.ToList();
            var waitingForConstruct = InstantiateBeansRecursive(typesInAssembly, currentInjectables);
            while (!waitingForConstruct.IsEmpty()) {
                waitingForConstruct = InstantiateBeansRecursive(waitingForConstruct, currentInjectables);
            }

            return currentInjectables.ToList();
        }

        private static List<Type> InstantiateBeansRecursive(IEnumerable<Type> beanTypesInAssembly,
            HashSet<Injectable> injectables) {
            var createdBeans = CreateBeans(beanTypesInAssembly, injectables, out var waitingForConstructBeans)
                .ToList();
            createdBeans.ForEach(injectable => injectables.Add(injectable));
            return waitingForConstructBeans;
        }


        private static IEnumerable<Injectable> CreateBeans(IEnumerable<Type> beanTypesInAssembly,
            HashSet<Injectable> currentInjectables,
            out List<Type> laterBeans) {
            laterBeans = new List<Type>();
            var createdBeans = new List<Injectable>();

            foreach (var beanType in beanTypesInAssembly) {
                var constructorInfos = beanType.GetConstructors();
                var fieldInfos = beanType.GetFields();
                if (HasInjectableNeeded(constructorInfos, fieldInfos, currentInjectables)) {
                    var instantiateBean = InstantiateWithControllers(constructorInfos, currentInjectables);
                    var injectable = new Injectable(instantiateBean);
                    createdBeans.Add(injectable);
                    currentInjectables.Add(injectable);
                } else {
                    var missingInjectables = MissingInjectables(constructorInfos, currentInjectables);
                    if (missingInjectables.All(type =>
                        ContainsInBeans(beanTypesInAssembly, type) ||
                        WaitingForOtherBeans(currentInjectables, type))) {
                        laterBeans.Add(beanType);
                    }
                }
            }

            return createdBeans;
        }

        private static bool WaitingForOtherBeans(HashSet<Injectable> currentInjectables, Type type) {
            return currentInjectables
                .Select(injectable => injectable.Inject.GetType())
                .Contains(type);
        }

        private static bool ContainsInBeans(IEnumerable<Type> beanTypesInAssembly, Type toContain) {
            var typesInAssembly = beanTypesInAssembly.ToList();
            return typesInAssembly.Contains(toContain) ||
                   Injector.HasInjectableWithInterface(toContain, typesInAssembly);
        }

        public static object InstantiateBean(Type bean, HashSet<Injectable> injectables) {
            try {
                var concreteInjectable = Injector.GetInjectableWithInterface(bean, injectables);
                var constructorInfos = concreteInjectable.Inject.GetType().GetConstructors();
                var instantiateBean = InstantiateWithControllers(constructorInfos, injectables);
                return instantiateBean;
            }
            catch (BeanConstructionException) {
                throw new BeanConstructionException("Can not Construct Bean of Type", bean);
            }
        }

        private static object InstantiateWithControllers(IEnumerable<ConstructorInfo> constructorInfos,
            HashSet<Injectable> injectables) {
            var injectablesAsTypes = injectables.Select(injectable => injectable.Inject.GetType()).ToList();
            foreach (var constructorInfo in constructorInfos) {
                var instantiateParameters = new List<object>();
                var constructorInfoMemberType = constructorInfo.GetParameters();
                foreach (var parameterInfo in constructorInfoMemberType) {
                    if (parameterInfo.ParameterType.GetGenericTypeDefinition() == typeof(List<>)) {
                        ResolveInjectableListParameter(injectables, parameterInfo.ParameterType, instantiateParameters);
                    } else {
                        ResolveInjectableParameter(injectables, injectablesAsTypes, parameterInfo.ParameterType, instantiateParameters);
                    }
                }

                var instantiateWithDependency = constructorInfo.Invoke(instantiateParameters.ToArray());
                Injector.InjectField<Grab>(instantiateWithDependency, injectables);
                return instantiateWithDependency;
            }

            throw new BeanConstructionException("Can not Construct Bean");
        }

        private static void ResolveInjectableListParameter(HashSet<Injectable> injectables, Type parameterType, List<object> instantiateParameters) {
            var listType = parameterType.GetGenericArguments()[0];
            var injectablesWithList = Injector.GetAllInjectableWithInterface(listType, injectables);
            var resultList = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(listType));
            foreach (var injectable in injectablesWithList) {
                resultList.Add(injectable.Inject);
            }

            instantiateParameters.Add(resultList);
        }

        private static void ResolveInjectableParameter(HashSet<Injectable> injectables,
            List<Type> injectablesAsTypes,
            Type type,
            List<object> instantiateParameters) {
            if (ContainsInBeans(injectablesAsTypes, type)) {
                Injectable bean;
                bean = type.IsInterface
                    ? Injector.GetInjectableWithInterface(type, injectables)
                    : injectables.First(injectable => injectable.Inject.GetType() == type);

                instantiateParameters.Add(bean);
            } else {
                throw new MissingInjectableException("missing Controller to be injected: ", type);
            }
        }

        private static bool HasInjectableNeeded(IEnumerable<ConstructorInfo> constructorInfos,
            IEnumerable<FieldInfo> fields,
            HashSet<Injectable> injectables) {
            var hasConstructorDependency = constructorInfos
                .SelectMany(constructorInfo => constructorInfo.GetParameters())
                .Select(parameterInfo => parameterInfo.ParameterType.GetGenericTypeDefinition() == typeof(List<>)
                    ? parameterInfo.ParameterType.GetGenericArguments()[0]
                    : parameterInfo.ParameterType)
                .All(type => ContainsInBeans(injectables.Select(injectable => injectable.Inject.GetType()), type));

            var hasFieldDependency = fields
                .Where(Injector.HasAttribute<Grab>)
                .Select(fieldinfo => fieldinfo.FieldType.GetGenericTypeDefinition() == typeof(List<>)
                    ? fieldinfo.FieldType.GetGenericArguments()[0]
                    : fieldinfo.FieldType)
                .All(type => ContainsInBeans(injectables.Select(injectable => injectable.Inject.GetType()), type));
            return hasFieldDependency && hasConstructorDependency;
        }

        private static IEnumerable<Type> MissingInjectables(IEnumerable<ConstructorInfo> constructorInfos,
            HashSet<Injectable> injectables) {
            var list = new List<Type>();
            foreach (ConstructorInfo constructorInfo in constructorInfos) {
                foreach (var parameterInfo in constructorInfo.GetParameters()) {
                    var injectableTypes = injectables.Select(injectable => injectable.Inject.GetType()).ToList();
                    if (!injectableTypes.Contains(parameterInfo.ParameterType)) {
                        if (!Injector.HasInjectableWithInterface(parameterInfo.ParameterType, injectableTypes)) {
                            list.Add(parameterInfo.ParameterType);
                        }
                    }
                }
            }

            return list;
        }
    }
}