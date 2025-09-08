// using Dont_Be_Humble.Injection;
// using NUnit.Framework;
// using UnityEngine.SceneManagement;
//
// namespace Dont_Be_Humble.TestEnvs {
// #pragma warning disable 0649
//     public abstract class DBHIntegrationTest {
//         [SetUp]
//         public void Init() {
//             SceneManager.LoadScene(PathToScene(), LoadSceneMode.Additive);
//             SceneManager.sceneLoaded += (scene, mode) => SceneManager.SetActiveScene(scene);
//
//             DependencyInjector.OnFinishedInjection += () => DependencyInjector.InjectFields(this);
//         }
//
//         [TearDown]
//         public void TearDown() {
//             DependencyInjector.Purge();
//             foreach (var g in SceneManager.GetActiveScene().GetRootGameObjects()) {
//                 if (g.name != "TaskManager") {
//                     g.SetActive(false);
//                 }
//             }
//         }
//
//         protected abstract string PathToScene();
//     }
// }