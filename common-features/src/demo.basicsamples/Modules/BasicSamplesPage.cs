namespace Serenity.Demo.BasicSamples;

[PageAuthorize, Route("BasicSamples/[action]")]
public partial class BasicSamplesPage : Controller
{
    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult ChartInDialog()
    {
        return this.SamplePanelPage(ESM.ChartInDialogPage, "Chart in a Dialog");
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult ChangingLookupText()
    {
        return View(MVC.Views.Editors.ChangingLookupText.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult CloneableEntityDialog()
    {
        return this.SampleGridPage(ESM.CloneableEntityDialogPage, "Cloneable Entity Dialog");
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult CustomLinksInGrid()
    {
        return View(MVC.Views.Grids.CustomLinksInGrid.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult DefaultValuesInNewDialog()
    {
        return this.SampleGridPage(ESM.DefaultValuesInNewDialogPage, "Default Values In New Dialog");
    }

    public ActionResult DialogBoxes()
    {
        return this.SamplePanelPage(ESM.DialogBoxesPage, "Dialog Box Types");
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult EnablingRowSelection()
    {
        return View(MVC.Views.Grids.EnablingRowSelection.Index);
    }

    [Route("{orderID?}")]
    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult EntityDialogAsPanel(int? orderID)
    {
        return this.SamplePanelPage(ESM.EntityDialogAsPanelPage, "Entity Dialog as Panel", options: orderID);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult FilteredLookupInDetailDialog()
    {
        return View(MVC.Views.Editors.FilteredLookupInDetail.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult GetInsertedRecordId()
    {
        return this.SampleGridPage(ESM.GetInsertedRecordIdPage, "Get Inserted Record Id From Dialog");
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult GridFilteredByCriteria()
    {
        return View(MVC.Views.Grids.GridFilteredByCriteria.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult GroupingAndSummariesInGrid()
    {
        return View(MVC.Views.Grids.GroupingAndSummariesInGrid.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult InitialValuesForQuickFilters()
    {
        return View(MVC.Views.Grids.InitialValuesForQuickFilters.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult InlineActionButtons()
    {
        return View(MVC.Views.Grids.InlineActionButtons.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult InlineImageInGrid()
    {
        return View(MVC.Views.Grids.InlineImageInGrid.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult LookupFilterByMultipleValues()
    {
        return View(MVC.Views.Editors.LookupFilterByMultipleValues.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult PopulateLinkedData()
    {
        return View(MVC.Views.Dialogs.PopulateLinkedData.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult ReadOnlyDialog()
    {
        return View(MVC.Views.Dialogs.ReadOnlyDialog.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult OtherFormInTab()
    {
        return View(MVC.Views.Dialogs.OtherFormInTab.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult OtherFormInTabOneBar()
    {
        return View(MVC.Views.Dialogs.OtherFormInTabOneBar.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult RemovingAddButton()
    {
        return View(MVC.Views.Grids.RemovingAddButton.Index);
    }

    public ActionResult SelectWithHardcodedValues()
    {
        return View(MVC.Views.Editors.SelectWithHardcodedValues.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult SerialAutoNumber()
    {
        return View(MVC.Views.Dialogs.SerialAutoNumber.Index);
    }

    public ActionResult StaticTextBlock()
    {
        return View(MVC.Views.Editors.StaticTextBlock.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult ViewWithoutID()
    {
        return View(MVC.Views.Grids.ViewWithoutID.Index);
    }

    [PageAuthorize(Northwind.PermissionKeys.General)]
    public ActionResult WrappedHeaders()
    {
        return View(MVC.Views.Grids.WrappedHeaders.Index);
    }
}

[Obsolete("Use BasicSamplesPage")]
public abstract class BasicSamplesController : BasicSamplesPage
{
}
