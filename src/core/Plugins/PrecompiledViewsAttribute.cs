namespace Serenity.Plugins;

/// <summary>
/// Indicates that this plugin assembly contains precompiled views (by RazorGenerator).
/// </summary>
/// <remarks>
/// This type is part of a legacy plugin system design. It is generally not
/// required with the modern Razor SDK, which handles precompiled views.
/// </remarks>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public class PrecompiledViewsAttribute : Attribute
{
}