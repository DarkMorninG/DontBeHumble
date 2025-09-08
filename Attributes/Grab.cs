using System;
using JetBrains.Annotations;

namespace Dont_Be_Humble.Attributes {
    /// <summary>
    /// Finds MonoBehaviours from Context and injects in to the Field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    [MeansImplicitUse]
    public class Grab : Attribute {
    }
}