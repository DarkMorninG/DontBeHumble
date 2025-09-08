using System;
using JetBrains.Annotations;

namespace Dont_Be_Humble.Attributes {
    [AttributeUsage(AttributeTargets.Field)]
    [MeansImplicitUse]
#pragma warning disable 0649
    public class MockGrab : Attribute {
    }
}