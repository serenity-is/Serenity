namespace Serenity.Web;

/// <summary>
/// The result of a module page action, which renders a module page view.
/// </summary>
public class ModulePageResult : ViewResult
{
    /// <summary>
    /// Gets the module page model.
    /// </summary>
    public new ModulePageModel Model => (ModulePageModel)base.Model;
}