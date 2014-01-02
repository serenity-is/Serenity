using jQueryApi;
using QUnit;
using Serenity;

namespace Serenity.Test
{
    [TestFixture]
    public class QBlockUITests
    {
        [Test]
        public void BlockUIWorksWithDefaults()
        {
            Q.BlockUI();
            Assert.IsTrue(jQuery.Select("div.blockUI.blockOverlay").Length > 0, 
                "Check BlockUI overlay exists.");

            Q.BlockUndo();
            Assert.IsTrue(jQuery.Select("div.blockUI.blockOverlay").Length == 0, 
                "Check that BlockUI overlay is removed after BlockUndo.");
        }

        [Test]
        public void BlockUICascadedCallWorks()
        {
            Q.BlockUI();
            Q.BlockUI();
            Assert.IsTrue(jQuery.Select("div.blockUI.blockOverlay").Length > 0,
                "Check BlockUI overlay exists after double calls.");

            Q.BlockUndo();
            Assert.IsTrue(jQuery.Select("div.blockUI.blockOverlay").Length > 0,
                "Check that BlockUI overlay IS NOT removed after first BlockUndo.");

            Q.BlockUndo();
            Assert.IsTrue(jQuery.Select("div.blockUI.blockOverlay").Length == 0,
                "Check that BlockUI overlay IS removed after second BlockUndo.");
        }
    }
}