using System;
using JetBrains.Annotations;

namespace Dont_Be_Humble.Attributes {
    [AttributeUsage(AttributeTargets.Class)]
    [MeansImplicitUse]
#pragma warning disable 0649
    public class IgnoreActiveInScene : Attribute {
    }
}