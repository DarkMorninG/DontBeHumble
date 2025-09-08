using System;
using JetBrains.Annotations;

namespace Attributes {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter)]
    [MeansImplicitUse]
#pragma warning disable 0649
    public class Prototype : Attribute {
    }
}