using System;
using JetBrains.Annotations;

namespace Attributes {
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public class AfterSceneUnLoad : Attribute {
    }
}