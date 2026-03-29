using System;

namespace Gast.Lib.Gaia
{
    /// <summary>
    /// Marks a DTO property as optional during deserialization.
    /// Properties without this attribute are treated as required by default.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class JsonOptionalAttribute : Attribute
    {
    }
}
