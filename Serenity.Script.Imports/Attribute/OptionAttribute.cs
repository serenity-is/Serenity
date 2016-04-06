using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class OptionAttribute : Attribute
    {
    }
}

namespace Serenity.ComponentModel
{
    public class Dummy
    {
    }
}
