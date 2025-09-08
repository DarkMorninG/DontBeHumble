using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Dont_Be_Humble.Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    [MeansImplicitUse]
    public class Subscribe : Attribute {
        public Subscribe([CallerMemberName] string methodName = null) {
            MethodName = methodName;
        }

        public string MethodName { get; }
    }
}