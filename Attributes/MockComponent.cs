using System;
using JetBrains.Annotations;

namespace Attributes {
    [AttributeUsage(AttributeTargets.Field)]
    [MeansImplicitUse]
#pragma warning disable 0649
    public class MockComponent : Attribute {
    }
}