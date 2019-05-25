using System;

namespace Serenity.Plugins
{
    /// <summary>
    /// Indicates that this plugin assembly contains precompiled views (by RazorGenerator)
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class PrecompiledViewsAttribute : Attribute
    {
    }
}