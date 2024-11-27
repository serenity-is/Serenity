using Microsoft.AspNetCore.Mvc;
using Views = Serenity.Demo.BasicSamples.MVC.Views.Editors;

namespace Serenity.Demo.BasicSamples;

public partial class BasicSamplesPage : Controller
{
    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult ChangingLookupText()
    {
        return View(Views.ChangingLookupText.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult FilteredLookupInDetailDialog()
    {
        return View(Views.FilteredLookupInDetail.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult LookupFilterByMultipleValues()
    {
        return View(Views.LookupFilterByMultipleValues.Index);
    }

    public ActionResult SelectWithHardcodedValues()
    {
        return View(Views.SelectWithHardcodedValues.Index);
    }

    public ActionResult StaticTextBlock()
    {
        return View(Views.StaticTextBlock.Index);
    }
}
