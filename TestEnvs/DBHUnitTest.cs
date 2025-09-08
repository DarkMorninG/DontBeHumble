// using NSubstitute;
// using NUnit.Framework;

namespace DBH.TestEnvs {
#pragma warning disable 0649
    public class DBHUnitTest {
        // private readonly List<GameObject> _createdGameObjects = new List<GameObject>();
        // protected IGameObjectDispatcher GameObjectDispatcher;
        // private ComponentMockController _componentControllerMockController;
        // private GameObject _testEnvoirement;
        //
        // [SetUp]
        // public void Init() {
        //     _testEnvoirement = new GameObject("TestEnvoirement" + Guid.NewGuid());
        //     GameObjectDispatcher = Substitute.For<IGameObjectDispatcher>();
        //     MockInject(GameObjectDispatcher);
        //     AddCreatedMocksToTestClass();
        //     AddCreatedMocksToDependencyInjector();
        //
        //     _componentControllerMockController = new ComponentMockController(_testEnvoirement);
        //     MockInject(_componentControllerMockController);
        // }
        //
        // [TearDown]
        // public void TearDown() {
        //     Cleanup();
        //     ComponentMockController.ClearMocksDictionary();
        //     DependencyInjector.Purge();
        //     MockInjector.Purge();
        //     _testEnvoirement.SetActive(false);
        // }
        //
        // protected T CreateMonoBehavior<T>(bool inject = true) where T : Component {
        //     var component = _testEnvoirement.AddComponent<T>();
        //     if (inject) DependencyInjector.FullInject(component);
        //
        //     return component;
        // }
        //
        // protected T AddToMockInject<T>() where T : Component {
        //     var monoBehavior = CreateMonoBehavior<T>();
        //     MockInject(monoBehavior);
        //     return monoBehavior;
        // }
        //
        // protected List<object> AddedComponentsToGameobject() {
        //     return ComponentMockController.GetAddedComponents();
        // }
        //
        // protected bool HasComponentAdded<T>() {
        //     var addedComponents = ComponentMockController.GetAddedComponents();
        //     return addedComponents != null && addedComponents.OfType<T>().Any();
        // }
        //
        // protected GameObject CreateChildGameObject(string name = "") {
        //     var gameObject = CreateGameObject(name);
        //     gameObject.transform.SetParent(_testEnvoirement.transform);
        //     return gameObject;
        // }
        //
        // protected GameObject CreateParentGameObject(string name = "") {
        //     var parentGameobject = CreateGameObject(name);
        //     _testEnvoirement.transform.SetParent(parentGameobject.transform);
        //     return parentGameobject;
        // }
        //
        // protected GameObject CreateGameObject(string name = "") {
        //     var gameObject = new GameObject(name + Guid.NewGuid());
        //     _createdGameObjects.Add(gameObject);
        //     return gameObject;
        // }
        //
        // protected T MockGetComponent<T>(object toReturn) {
        //     return _componentControllerMockController.GetComponentFromGameObject<T>(toReturn);
        // }
        //
        // protected GameObject ThisGameObject() {
        //     return _testEnvoirement;
        // }
        //
        // protected void MockInject(object ob) {
        //     DependencyInjector.Register(ob);
        // }
        //
        // protected Transform EnvTransform() {
        //     return _testEnvoirement.transform;
        // }
        //
        // public void Cleanup() {
        //     _createdGameObjects.Clear();
        // }
        //
        // private void AddCreatedMocksToTestClass() {
        //     MockInjector.InjectAll(this);
        // }
        //
        // private void AddCreatedMocksToDependencyInjector() {
        //     foreach (var mock in MockInjector.Mocks) MockInject(mock);
        // }
    }
}