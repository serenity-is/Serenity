namespace Serenity.Web;

/// <summary>
/// Sets the layout of the module pages of a controller, e.g. the ones created with <c>GridPage</c> or
/// <c>PanelPage</c>, when the page does not set a layout of its own. It lets a controller, and the ones that
/// derive from it, tell the layout of its pages in one place, which a page that is not based on a view file, e.g.
/// a panel page created from a module script only, has no other way to do.
/// </summary>
/// <param name="layout">The layout to use.</param>
/// <exception cref="ArgumentNullException">Layout is <c>null</c>.</exception>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ModulePageLayoutAttribute(string layout) : Attribute
{
    /// <summary>
    /// Gets the layout to use.
    /// </summary>
    public string Layout { get; } = layout ?? throw new ArgumentNullException(nameof(layout));
}
