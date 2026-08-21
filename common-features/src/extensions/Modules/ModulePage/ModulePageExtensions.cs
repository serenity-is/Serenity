namespace Serenity.Web;

/// <summary>
/// Extension methods for creating module and grid pages from controllers.
/// </summary>
public static class ModulePageExtensions
{
    /// <summary>
    /// Gets the page title local text key for the specified row fields.
    /// </summary>
    /// <param name="fields">The row fields.</param>
    /// <returns>The page title local text key.</returns>
    public static string PageTitle(this RowFieldsBase fields)
    {
        return "Db." + fields.LocalTextPrefix + ".EntityPlural";
    }

    /// <summary>
    /// Creates a grid page for the specified row type and module.
    /// </summary>
    /// <typeparam name="TRow">The row type.</typeparam>
    /// <param name="controller">The controller.</param>
    /// <param name="module">The module name.</param>
    /// <param name="options">Optional options passed to the module script.</param>
    /// <param name="layout">The layout to use.</param>
    /// <param name="pageTitle">The page title.</param>
    /// <returns>The module page result.</returns>
    public static ModulePageResult GridPage<TRow>(this Controller controller, string module,
        object options = null, string layout = null, LocalText pageTitle = null)
        where TRow: IRow, new()
    {
        return GridPage(controller, module, pageTitle ?? new TRow().Fields.PageTitle(), options, layout: layout);
    }

    /// <summary>
    /// Creates a grid page for the specified module and page title.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="module">The module name.</param>
    /// <param name="pageTitle">The page title.</param>
    /// <param name="options">Optional options passed to the module script.</param>
    /// <param name="layout">The layout to use.</param>
    /// <returns>The module page result.</returns>
    public static ModulePageResult GridPage(this Controller controller, string module, LocalText pageTitle,
        object options = null, string layout = null)
    {
        return GridPage(controller, new()
        {
            Module = module,
            PageTitle = pageTitle,
            Options = options,
            Layout = layout
        });
    }

    /// <summary>
    /// Creates a grid page from the specified module page model.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="model">The module page model.</param>
    /// <returns>The module page result.</returns>
    public static ModulePageResult GridPage(this Controller controller, ModulePageModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.HtmlMarkup ??= "<div id=\"GridDiv\"></div>";
        return ModulePage(controller, model);
    }

    /// <summary>
    /// Creates a module page from the specified module page model.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="model">The module page model.</param>
    /// <returns>The module page result.</returns>
    public static ModulePageResult ModulePage(this Controller controller, ModulePageModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (string.IsNullOrEmpty(model.Module))
            throw new ArgumentNullException(nameof(model), $"{nameof(model.Module)} cannot be null or empty!");

        if (model.Module.StartsWith("@/"))
        {
            model.Module = "~/esm/Modules/" + model.Module[2..];
            if (!model.Module.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
                model.Module += ".js";
        }

        controller.ViewData.Model = model;

        return new()
        {
            ViewName = Extensions.MVC.Views.ModulePage.ModulePage_,
            ViewData = controller.ViewData,
            TempData = controller.TempData,
        };
    }

    /// <summary>
    /// Creates a panel page for the specified module and page title.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="module">The module name.</param>
    /// <param name="pageTitle">The page title.</param>
    /// <param name="options">Optional options passed to the module script.</param>
    /// <param name="layout">The layout to use.</param>
    /// <returns>The module page result.</returns>
    public static ModulePageResult PanelPage(this Controller controller, string module, LocalText pageTitle,
        object options = null, string layout = null)
    {
        return PanelPage(controller, new()
        {
            Module = module,
            PageTitle = pageTitle,
            Options = options,
            Layout = layout
        });
    }

    /// <summary>
    /// Creates a panel page from the specified module page model.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="model">The module page model.</param>
    /// <returns>The module page result.</returns>
    public static ModulePageResult PanelPage(this Controller controller, ModulePageModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.HtmlMarkup ??= "<div id=\"PanelDiv\"></div>";
        return ModulePage(controller, model);
    }

    /// <summary>
    /// Sets the page title of the result from the specified row type.
    /// </summary>
    /// <typeparam name="TRow">The row type.</typeparam>
    /// <param name="result">The module page result.</param>
    /// <returns>The module page result.</returns>
    public static ModulePageResult PageTitle<TRow>(this ModulePageResult result)
        where TRow : IRow, new()
    {
        result.Model.PageTitle = new TRow().Fields.PageTitle();
        return result;
    }

    /// <summary>
    /// Sets the page title of the result.
    /// </summary>
    /// <param name="result">The module page result.</param>
    /// <param name="pageTitle">The page title.</param>
    /// <returns>The module page result.</returns>
    public static ModulePageResult PageTitle(this ModulePageResult result, LocalText pageTitle)
    {
        result.Model.PageTitle = pageTitle;
        return result;
    }

    /// <summary>
    /// Sets the layout of the result.
    /// </summary>
    /// <param name="result">The module page result.</param>
    /// <param name="layout">The layout to use.</param>
    /// <returns>The module page result.</returns>
    public static ModulePageResult Layout(this ModulePageResult result, string layout)
    {
        result.Model.Layout = layout;
        return result;
    }
}