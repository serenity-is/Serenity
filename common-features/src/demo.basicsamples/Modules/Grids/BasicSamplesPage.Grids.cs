using Microsoft.AspNetCore.Mvc;
using Views = Serenity.Demo.BasicSamples.MVC.Views.Grids;

namespace Serenity.Demo.BasicSamples;

public partial class BasicSamplesPage : Controller
{
    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult CustomLinksInGrid()
    {
        return View(Views.CustomLinksInGrid.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult EnablingRowSelection()
    {
        return View(Views.EnablingRowSelection.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult GridFilteredByCriteria()
    {
        return View(Views.GridFilteredByCriteria.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult GroupingAndSummariesInGrid()
    {
        return View(Views.GroupingAndSummariesInGrid.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult InitialValuesForQuickFilters()
    {
        return View(Views.InitialValuesForQuickFilters.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult InlineActionButtons()
    {
        return View(Views.InlineActionButtons.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult InlineImageInGrid()
    {
        return View(Views.InlineImageInGrid.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult RemovingAddButton()
    {
        return View(Views.RemovingAddButton.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult ViewWithoutID()
    {
        return View(Views.ViewWithoutID.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult WrappedHeaders()
    {
        return View(Views.WrappedHeaders.Index);
    }
}