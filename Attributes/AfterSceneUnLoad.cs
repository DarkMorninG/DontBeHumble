using System;
using JetBrains.Annotations;

namespace DBH.Attributes {
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public class AfterSceneUnLoad : Attribute {
    }
}