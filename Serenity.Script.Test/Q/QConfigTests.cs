using QUnit;
using Serenity;
using System;
using System.Html;

namespace Serenity.Test
{
    [TestFixture]
    public class QConfigTests
    {
        [Test]
        public void ConfigApplicationPathCanBeSet()
        {
            // to check script name safety
            var qConfig = Window.Instance.As<dynamic>()["Q$Config"];
            var backup = Q.Config.ApplicationPath;
            try
            {
                Q.Config.ApplicationPath = "/Dummy1";
                Assert.AreEqual(qConfig.applicationPath, "/Dummy1", "Set by C#, read directly");

                qConfig.applicationPath = "/Dummy2";
                Assert.AreEqual(Q.Config.ApplicationPath, "/Dummy2", "Set directly, read by C#");
            }
            finally
            {
                Q.Config.ApplicationPath = backup;
            }
        }
    }
}