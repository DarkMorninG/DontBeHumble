using System;
using JetBrains.Annotations;

namespace DBH.Attributes {
    [AttributeUsage(AttributeTargets.Field)]
    [MeansImplicitUse]
#pragma warning disable 0649
    public class Mock : Attribute {
    }
}