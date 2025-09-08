using System;
using JetBrains.Annotations;

namespace Dont_Be_Humble.Attributes {
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public class AfterSceneLoad : Attribute {
    }
}