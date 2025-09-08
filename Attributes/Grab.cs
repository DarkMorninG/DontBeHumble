using System;
using JetBrains.Annotations;

namespace Attributes {
    /// <summary>
    /// Finds MonoBehaviours from Context and injects in to the Field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    [MeansImplicitUse]
    public class Grab : Attribute {
    }
}