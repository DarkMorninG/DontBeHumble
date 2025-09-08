using System;
using JetBrains.Annotations;

namespace Attributes {
    [AttributeUsage(AttributeTargets.Class)]
    [MeansImplicitUse]
#pragma warning disable 0649
    public class InjectionScanned : Attribute {
    }
}