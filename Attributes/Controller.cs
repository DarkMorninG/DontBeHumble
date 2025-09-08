using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace Dont_Be_Humble.Attributes {
    [AttributeUsage(AttributeTargets.Class)]
    [BaseTypeRequired(typeof(MonoBehaviour))]
    [InjectionScanned]
    [DisallowMultipleComponent]
    public class Controller : Attribute {
        public Controller([CallerMemberName] string className = null) {
            ClassName = className;
        }

        public string ClassName { get; }

        public Guid G { get; } = Guid.NewGuid();
    }
}