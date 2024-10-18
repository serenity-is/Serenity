using Microsoft.AspNetCore.Mvc;

namespace Serenity.Web;

public static class ModulePageExtensions
{
    public static string PageTitle(this RowFieldsBase fields)
    {
        return "Db." + fields.LocalTextPrefix + ".EntityPlural";
    }

    public static ModulePageResult GridPage<TRow>(this Controller controller, string module,
        object options = null, string layout = null, LocalText pageTitle = null)
        where TRow: IRow, new()
    {
        return GridPage(controller, module, pageTitle ?? new TRow().Fields.PageTitle(), options, layout: layout);
    }

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

    public static ModulePageResult GridPage(this Controller controller, ModulePageModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.HtmlMarkup ??= "<div id=\"GridDiv\"></div>";
        return ModulePage(controller, model);
    }

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

    public static ModulePageResult PanelPage(this Controller controller, ModulePageModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.HtmlMarkup ??= "<div id=\"PanelDiv\"></div>";
        return ModulePage(controller, model);
    }

    public static ModulePageResult PageTitle<TRow>(this ModulePageResult result)
        where TRow : IRow, new()
    {
        result.Model.PageTitle = new TRow().Fields.PageTitle();
        return result;
    }

    public static ModulePageResult PageTitle(this ModulePageResult result, LocalText pageTitle)
    {
        result.Model.PageTitle = pageTitle;
        return result;
    }

    public static ModulePageResult Layout(this ModulePageResult result, string layout)
    {
        result.Model.Layout = layout;
        return result;
    }
}