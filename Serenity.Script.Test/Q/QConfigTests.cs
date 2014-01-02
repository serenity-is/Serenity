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
        public void ApplicationPathCanBeSet()
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

        [Test]
        public void RootNamespacesListWorks()
        {
            Assert.IsNotNull(Q.Config.RootNamespaces, "The list is initialized");
            
            var count = Q.Config.RootNamespaces.Count;
            Assert.IsTrue(count > 0 && Q.Config.RootNamespaces.Contains("Serenity"),
                "The list should contain 'Serenity'");

            Q.Config.RootNamespaces.Add("SomeDummyNamespace");
            Assert.IsTrue(Q.Config.RootNamespaces.Contains("SomeDummyNamespace"), "Can add a new root namespace to list");

            Q.Config.RootNamespaces.Remove("SomeDummyNamespace");
            Assert.IsTrue(!Q.Config.RootNamespaces.Contains("SomeDummyNamespace"), "Can remove namespace from the list");
        }
    }
}