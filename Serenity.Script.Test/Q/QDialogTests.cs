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
        public void AlertDialogCanOpenClose()
        {
            Q.Alert("Test");
            Assert.IsTrue(jQuery.Select("div.s-AlertDialog.ui-dialog:visible").Length > 0, 
                "Check alert dialog exists and visible.");

            jQuery.Select(".ui-dialog-titlebar-close:visible").Click();

            Assert.IsTrue(jQuery.Select("div.s-AlertDialog.ui-dialog:visible").Length == 0, 
                "Check alert dialog is closed.");
        }

        [Test]
        public void ConfirmDialogCanOpenClose()
        {
            var confirmed = false;

            Q.Confirm("Test ABCDEFGHJKL", delegate { confirmed = true; });

            var dlg = jQuery.Select("div.s-ConfirmDialog.ui-dialog:visible");

            Assert.IsTrue(dlg.Length > 0,
                "Check confirmation dialog exists and visible.");

            Assert.IsTrue(dlg.Find(":contains('Test ABCDEFGHJKL'):visible").Length > 0,
                "Check that message is displayed");

            var yesButton = jQuery.Select(".ui-dialog-buttonset button:contains('" + Q.Text("Dialogs.YesButton") + "')");
            Assert.IsTrue(yesButton.Length == 1, "Check that dialog has Yes button");

            var noButton = jQuery.Select(".ui-dialog-buttonset button:contains('" + Q.Text("Dialogs.NoButton") + "')");
            Assert.IsTrue(noButton.Length == 1, "Check that dialog has No button");

            jQuery.Select(".ui-dialog-titlebar-close:visible").Click();

            Assert.IsTrue(jQuery.Select("div.s-ConfirmDialog.ui-dialog:visible").Length == 0,
                "Check confirmation dialog is closed.");

            Assert.IsFalse(confirmed, "Check that confirmed is not set when dialog is closed directly");
        }

        [Test]
        public void ConfirmDialogYesClickCallsSuccess()
        {
            var confirmed = false;

            Q.Confirm("Test", delegate { confirmed = true; });

            jQuery.Select(".ui-dialog-buttonset button:contains('" + Q.Text("Dialogs.YesButton") + "')").Click();

            Assert.IsTrue(confirmed, "Ensure yes button click called success delegate");
        }

        [Test]
        public void ConfirmDialogNoClickDoesntCallSuccess()
        {
            var confirmed = false;

            Q.Confirm("Test", delegate { confirmed = true; });

            jQuery.Select(".ui-dialog-buttonset button:contains('" + Q.Text("Dialogs.NoButton") + "')").Click();

            Assert.IsFalse(confirmed, "Ensure no button click didn't call success delegate");
        }

        [Test]
        public void InformationDialogCanOpenClose()
        {
            Q.Information("Test");
            Assert.IsTrue(jQuery.Select("div.s-InformationDialog.ui-dialog:visible").Length > 0,
                "Check information dialog exists and visible.");

            jQuery.Select(".ui-dialog-titlebar-close:visible").Click();

            Assert.IsTrue(jQuery.Select("div.s-InformationDialog.ui-dialog:visible").Length == 0,
                "Check information dialog is closed.");
        }
    }
}