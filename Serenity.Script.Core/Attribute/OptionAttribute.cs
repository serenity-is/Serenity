using System;

namespace Serenity.ComponentModel
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class OptionAttribute : Attribute
    {
    }
}