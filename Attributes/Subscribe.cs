using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    [MeansImplicitUse]
    public class Subscribe : Attribute {
        public Subscribe([CallerMemberName] string methodName = null) {
            MethodName = methodName;
        }

        public string MethodName { get; }
    }
}