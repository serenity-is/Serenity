using System;

namespace Serenity
{
    /// <summary>
    /// Obsolete attribute used to discriminate Serenity referencing assemblies from system ones.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [Obsolete("This is no longer used."), AttributeUsage(AttributeTargets.Assembly)]
    public class SelfAssemblyAttribute : Attribute
    {
    }
}