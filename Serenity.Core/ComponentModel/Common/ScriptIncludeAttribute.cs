using System;

namespace Serenity.ComponentModel
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum, AllowMultiple = false)]
    public class ScriptIncludeAttribute : Attribute
    {
    }
}