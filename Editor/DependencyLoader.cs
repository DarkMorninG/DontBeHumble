using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using Vault;

namespace DBH.Editor {
    [InitializeOnLoad]
    public class DependencyLoader {
        static DependencyLoader() {
            new DependencyLoader().OnUnityStart();
            Debug.Log("Git DependencyLoader Started");
        }
        public void OnUnityStart() {
            Events.registeredPackages += EventsOnregisteredPackages;
        }

        private void EventsOnregisteredPackages(PackageRegistrationEventArgs packageRegistrationEventArgs) {
            foreach (var packageInfo in packageRegistrationEventArgs.added) {
                PullDependency(packageInfo.dependencies);
            }
        }

        private void PullDependency(IEnumerable<DependencyInfo> dependencyInfos) {
            var packageCollection = Client.List()
                .Result;
            var githubDependencies = dependencyInfos
                .Where(info => info.version.Contains("github.com"))
                .ToList();
            packageCollection.Where(info => !githubDependencies.Select(dependencyInfo => dependencyInfo.name).Contains(info.name))
                .ForEach(info => {
                    Debug.Log("Found Missing Dependency " + info.version);
                    Client.Add(info.version);
                });
        }
    }
}