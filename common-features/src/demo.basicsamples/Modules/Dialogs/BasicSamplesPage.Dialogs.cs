using Microsoft.AspNetCore.Mvc;
using Views = Serenity.Demo.BasicSamples.MVC.Views.Dialogs;

namespace Serenity.Demo.BasicSamples;

public partial class BasicSamplesPage : Controller
{
    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult ChartInDialog()
    {
        return View(Views.ChartInDialog.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult CloneableEntityDialog()
    {
        return View(Views.CloneableEntityDialog.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult DefaultValuesInNewDialog()
    {
        return View(Views.DefaultValuesInNewDialog.Index);
    }

    public ActionResult DialogBoxes()
    {
        return View(Views.DialogBoxes.Index);
    }

    [Route("{orderID?}")]
    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult EntityDialogAsPanel(int? orderID)
    {
        return View(Views.EntityDialogAsPanel.Index, orderID);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult GetInsertedRecordId()
    {
        return View(Views.GetInsertedRecordId.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult PopulateLinkedData()
    {
        return View(Views.PopulateLinkedData.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult ReadOnlyDialog()
    {
        return View(Views.ReadOnlyDialog.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult OtherFormInTab()
    {
        return View(Views.OtherFormInTab.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult OtherFormInTabOneBar()
    {
        return View(Views.OtherFormInTabOneBar.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult SerialAutoNumber()
    {
        return View(Views.SerialAutoNumber.Index);
    }
}