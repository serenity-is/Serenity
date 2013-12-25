using jQueryApi;
using jQueryApi.UI;
using jQueryApi.UI.Widgets;
using Serenity;
using QUnit;

namespace Serenity.Test
{
    [TestFixture]
    public class QDialogTests
    {
        [Test]
        public void AlertDialogWorks()
        {
            Q.Alert("Test");
            Assert.IsTrue(jQuery.Select("div.s-AlertDialog.ui-dialog:visible").Length > 0, 
                "Check alert dialog exists and visible.");

            jQuery.Select("div.s-AlertDialog.ui-dialog:visible .ui-dialog-content").Dialog().Close();

            Assert.IsTrue(jQuery.Select("div.s-AlertDialog.ui-dialog:visible").Length == 0, 
                "Check alert dialog is closed.");
        }

        [Test]
        public void ConfirmDialogWorks()
        {
            Q.Confirm("Test", delegate { });
            Assert.IsTrue(jQuery.Select("div.s-ConfirmDialog.ui-dialog:visible").Length > 0,
                "Check confirmation dialog exists and visible.");

            jQuery.Select("div.s-ConfirmDialog.ui-dialog:visible .ui-dialog-content").Dialog().Close();

            Assert.IsTrue(jQuery.Select("div.s-ConfirmDialog.ui-dialog:visible").Length == 0,
                "Check confirmation dialog is closed.");
        }

        [Test]
        public void InformationDialogWorks()
        {
            Q.Information("Test");
            Assert.IsTrue(jQuery.Select("div.s-InformationDialog.ui-dialog:visible").Length > 0,
                "Check information dialog exists and visible.");

            jQuery.Select("div.s-InformationDialog.ui-dialog:visible .ui-dialog-content").Dialog().Close();

            Assert.IsTrue(jQuery.Select("div.s-InformationDialog.ui-dialog:visible").Length == 0,
                "Check information dialog is closed.");
        }
    }
}