using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DBH.Adapter;
using DBH.Attributes;
using DBH.Controllers;
using DBH.Injection.dto;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vault;
using Vault.BetterCoroutine;
using Object = UnityEngine.Object;

namespace DBH.Injection {
    [DisallowMultipleComponent]
    public class DependencyInjector : MonoBehaviour {
        [SerializeField]
        private List<string> assemblyName;

        private static readonly Dictionary<Type, Action<Object>> AfterInjections = new();
        private static readonly HashSet<Injectable> Beans = new();

        private static readonly HashSet<Injectable> Controllers = new();

        private static readonly List<Assembly> CurrentAssembly = new();
        private static readonly List<IInjectAdapter> InjectAdapters = new();

        private static Config.Config currentConfig;

        public static bool InjectionFinished { get; private set; } = true;

        public delegate void VoidNoParameter();

        public static event VoidNoParameter OnFinishedInjection;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Bootstrap() {
            UnityThread.initUnityThread();
            currentConfig = new Config.Config();
            SceneManager.sceneLoaded += (scene, mode) => InjectScene(scene);
            SceneManager.sceneUnloaded += AfterSceneUnload;
        }

        public void StartUp() {
            InjectionFinished = false;
            currentConfig.AssemblysToScan.AddRange(assemblyName);
            var componentFromScene = GetComponentFromScene(gameObject.scene).ToList();
            RegisterControllers(componentFromScene);
            InstantiateBeans();
            InjectAdapters.AddRange(FilterInjectorAdapters(Beans));
            InjectFields<Grab>(componentFromScene);
            SubscribeInterfaces(componentFromScene);
            InvokePostConstructMethods(componentFromScene);
            InvokeAfterSceneLoading(GatherInjectables());
            CallAfterInjection();
            InjectionFinished = true;
        }


        public static void InjectScene(Scene scene) {
            InjectionFinished = false;
            var componentFromScene = GetComponentFromScene(scene).ToList();
            RegisterControllers(componentFromScene);
            InjectFields<Grab>(componentFromScene);
            SubscribeInterfaces(componentFromScene);
            InvokePostConstructMethods(componentFromScene);
            OnFinishedInjection?.Invoke();
            InjectionFinished = true;
        }

        private static void AfterSceneUnload(Scene scene) {
            Controllers.RemoveWhere(injectable => injectable.Inject == null);
            InvokeAfterSceneUnLoaded(GatherInjectables());
        }


        public static void Purge() {
            Controllers.Clear();
            Beans.Clear();
        }

        public static void PurgeControllers() {
            Controllers.Clear();
        }

        public static void Register(object ob) {
            var injectable = new Injectable(ob);

            Controllers.TryGetValue(injectable, out var found);
            if (found == null) {
                Controllers.Add(injectable);
            } else {
                ReplaceControllerOnNewScene(found, injectable);
            }
        }

        private static void ReplaceControllerOnNewScene(Injectable found, Injectable injectable) {
            if (found.Inject is not Component component) return;
            if (component.gameObject.scene.name == "DontDestroyOnLoad") return;
            Controllers.Remove(found);
            Controllers.Add(injectable);
            Debug.Log("replaced: " + injectable.Inject.GetType());
        }


        private static void InjectFields<T>(IEnumerable<Object> components = null) {
            components ??= GetComponentsInScene();
            var controllersAndBeans = GatherInjectables();
            components.ToList().ForEach(component => Injector.InjectField<T>(component, controllersAndBeans));
            foreach (var injectAdapter in InjectAdapters) {
                injectAdapter.Inject(new InjectionContext(controllersAndBeans));
            }
        }

        private static void InstantiateBeans() {
            BeanCreator.InstantiateBeans(GetBeanTypesInAssembly(), GatherInjectables())
                .ForEach(injectable => Beans.Add(injectable));
        }

        private static void SubscribeInterfaces(IEnumerable<Object> components = null) {
            components ??= GetComponentsInScene();
            foreach (var controller in Controllers) {
                Injector.SubscribeInterface(controller.Inject, components);
            }
        }

        private static void InvokeAfterSceneLoading(HashSet<Injectable> injectables) {
            foreach (var injectable in injectables) {
                Injector.InvokeWithAttribute<AfterSceneLoad>(injectable.Inject);
            }
        }

        private static void InvokeAfterSceneUnLoaded(HashSet<Injectable> injectables) {
            foreach (var injectable in injectables) {
                Injector.InvokeWithAttribute<AfterSceneUnLoad>(injectable.Inject);
            }
        }


        private static void InvokePostConstructMethods(IEnumerable<Object> components = null, bool async = false) {
            var start = DateTime.Now;
            var infos = new List<PostConstructValue>();
            var toInvoke = new List<Object>();
            toInvoke.AddRange(components ?? GetComponentsInScene());

            toInvoke.RemoveAll(o => !Injector.HasAttribute<InjectionScanned>(o));
            // toInvoke.RemoveAll(o => o.GetType().BaseType == typeof(ScriptableObject));
            toInvoke.ForEach(o => {
                if (o == null) return;
                Injector.GetMethodsOfOnlyBase(o)
                    .ToList()
                    .ForEach(methodInfo => {
                        if (!Injector.HasAttribute<PostConstruct>(methodInfo)) return;
                        var postConstruct = Injector.GetAttribute<PostConstruct>(methodInfo);
                        infos.Add(new PostConstructValue(postConstruct.ExecutionOrder, methodInfo, o));
                    });
            });

            var end = DateTime.Now;
            // Debug.Log("gather postconstruct methods took: " + (end - start).TotalSeconds);

            infos.RemoveNullItems();
            foreach (var postConstructValue in infos.OrderBy(value => value.Priority)) {
                if (async) {
                    UnityThread.executeInUpdate(() => {
                        Injector.InvokeMethod<PostConstruct>(
                            postConstructValue.ObjectThatHasMethodToInvoke,
                            postConstructValue.MethodToInvoke);
                    });
                } else {
                    Injector.InvokeMethod<PostConstruct>(
                        postConstructValue.ObjectThatHasMethodToInvoke,
                        postConstructValue.MethodToInvoke);
                }
            }
        }

        private static HashSet<Injectable> GatherInjectables() {
            var controllersAndBeans = new HashSet<Injectable>();

            foreach (var controller in Controllers) {
                controllersAndBeans.Add(controller);
            }

            foreach (var bean in Beans) {
                controllersAndBeans.Add(bean);
            }

            return controllersAndBeans;
        }

        private static void RegisterControllers(IEnumerable<Object> components = null) {
            Controllers.RemoveWhere(injectable => (injectable.Inject as Component)?.gameObject == null);
            components ??= GetComponentsInScene();
            var foundComponents = components
                .Where(Injector.HasAttribute<Controller>)
                .ToList();

            foreach (var comp in foundComponents) {
                Register(comp);
            }
        }

        public static GameObject InjectGameObject(GameObject gameObject, bool async = false) {
            var componentsToInject = new List<Component>();
            componentsToInject.AddRange(gameObject.GetComponentsInChildren<Component>());
            var gatherInjectables = GatherInjectables();
            Parallel.ForEach(componentsToInject, component => OnlyInject(component, gatherInjectables));
            InvokePostConstructMethods(componentsToInject.Select(component => (Object)component).ToList(), async);

            return gameObject;
        }

        private static void InjectTheUsual(Component component,
            HashSet<Injectable> toInject,
            IEnumerable<Object> componentsInScene) {
            Injector.InjectField<Grab>(component, toInject);
            Injector.SubscribeInterface(component, componentsInScene);
        }

        private static void OnlyInject(Component toInject, HashSet<Injectable> injectables) {
            Injector.InjectField<Grab>(toInject, injectables);
        }

        private static IEnumerable<Object> GetComponentsInScene(Scene? scene = null) {
            return scene.HasValue ? GetComponentFromScene(scene.Value) : GetComponentInActiveScene();
        }

        private static IEnumerable<Object> GetComponentInActiveScene() {
            var gameObjectsInScene = FindObjectsOfType<GameObject>();
            var unityObjects = Resources.FindObjectsOfTypeAll<ScriptableObject>();

            var components = GetComponentsFromGameObjects(gameObjectsInScene);
            components.AddRangeNoDuplicates(unityObjects.Cast<Object>().ToList());

            var loadedScenes = new List<Scene>();
            for (var i = 0; i < SceneManager.sceneCount; i++) {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded && SceneManager.GetActiveScene() != scene) {
                    loadedScenes.Add(scene);
                }
            }

            foreach (var loadedScene in loadedScenes) {
                components.AddRangeNoDuplicates(GetComponentsFromGameObjects(loadedScene.GetRootGameObjects()));
            }

            return components;
        }

        private static IEnumerable<Object> GetComponentFromScene(Scene scene) {
            var gameObjectsInScene = scene.GetRootGameObjects();
            var componentsFromGameObjects = GetComponentsFromGameObjects(gameObjectsInScene);
            foreach (var gameObject in gameObjectsInScene) {
                var allChildrenRecursive = gameObject.FindAllChildrenRecursive();
                componentsFromGameObjects.AddRange(GetComponentsFromGameObjects(allChildrenRecursive));
            }

            return componentsFromGameObjects;
        }


        private static List<Object> GetComponentsFromGameObjects(IEnumerable<GameObject> gameObjectsInScene) {
            var components = new List<Object>();
            foreach (var rootGameObject in gameObjectsInScene) {
                var toBeScannedComponents = rootGameObject.GetComponents<Component>()
                    .Where(component => Injector.HasAttribute<InjectionScanned>(component) ||
                                        Injector.HasAttribute<Controller>(component));
                foreach (var component in toBeScannedComponents) {
                    components.AddNotNull(component);
                }
            }

            return components;
        }

        private static IEnumerable<Type> GetBeanTypesInAssembly() {
            if (CurrentAssembly.IsNullOrEmpty()) {
                CurrentAssembly.Clear();
                CurrentAssembly.AddRange(AppDomain.CurrentDomain.GetAssemblies()
                    .Where(assembly => currentConfig.AssemblysToScan.Any(s => assembly.GetName().Name.Contains(s)))
                    .ToList());
            }

            return (from assembly in CurrentAssembly
                from type in assembly.GetTypes()
                where Injector.HasAttribute<Bean>(type)
                select type).ToList();
        }

        private static IEnumerable<IInjectAdapter> FilterInjectorAdapters(IEnumerable<Injectable> allBeans) {
            return (from injectable in allBeans
                where Injector.HasInterface(injectable.Inject, typeof(IInjectAdapter))
                select (IInjectAdapter)injectable.Inject).ToList();
        }


        private void AddDefaultControllerIfNotPresent() {
            if (gameObject.GetComponent<GameObjectDispatcher>()) {
                gameObject.AddComponent<GameObjectDispatcher>();
            }

            if (gameObject.GetComponent<ComponentController>()) {
                gameObject.AddComponent<ComponentController>();
            }
        }

        private static void CallAfterInjection() {
            foreach (var (key, value) in AfterInjections) {
                foreach (var controller in Controllers
                    .Where(controller => controller.Inject.GetType() == key)) {
                    value.Invoke(controller.Inject as Object);
                }
            }
        }

        public static void FullInject(Component component) {
            Injector.InjectField<Grab>(component, GatherInjectables());
            Injector.SubscribeInterface(component, GetComponentsInScene());
            Injector.InvokeWithAttribute<PostConstruct>(component);
        }

        public static void FullInjectIgnoreSubscribe(Component component) {
            Injector.InjectField<Grab>(component, GatherInjectables());
            Injector.InvokeWithAttribute<PostConstruct>(component);
        }

        public static void InjectFields(object component) {
            Injector.InjectField<Grab>(component, GatherInjectables());
        }

        public static void GetController<T>(Action<T> afterInjectionFinished) {
            AfterInjections.Add(typeof(T), o => afterInjectionFinished((T)Convert.ChangeType(o, typeof(T))));
        }
    }
}