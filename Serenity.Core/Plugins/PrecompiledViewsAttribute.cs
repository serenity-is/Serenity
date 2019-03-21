using System;

namespace Serenity.Plugins
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class PrecompiledViewsAttribute : Attribute
    {
    }
}
