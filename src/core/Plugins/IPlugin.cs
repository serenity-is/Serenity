namespace Serenity.Plugins;

/// <summary>
/// Abstraction for plugins.
/// </summary>
/// <remarks>
/// This type is part of a legacy plugin system design and is no longer used by
/// the framework. Its responsibilities are superseded by other mechanisms, e.g.
/// <c>LayoutCssFile</c>/<c>LayoutScriptFile</c> for layout assets,
/// <c>NavigationLink</c> for navigation items, and dependency injection based
/// registration (e.g. <c>AddAutoRegisteredServices</c>) for services.
/// </remarks>
public interface IPlugin
{
}
