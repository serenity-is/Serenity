using jQueryApi;
using QUnit;
using System.Html;

namespace Serenity.Test
{
    [TestFixture]
    public class ScriptContextTests
    {
        private class SubClass : ScriptContext
        {
            public jQueryObject Select(object x)
            {
                return J(x);
            }

            public jQueryObject Select(object x, object y)
            {
                return J(x, y);
            }
        }

        [Test]
        public void JWorksWithOneParam()
        {
            Assert.AreEqual(1, new SubClass().Select("body").Length);
        }

        [Test]
        public void JWorksWithTwoParams()
        {
            Assert.AreEqual(1, new SubClass().Select("body", Window.Document).Length);
            Assert.AreEqual(0, new SubClass().Select("document", Window.Document.Body).Length);
        }
    }
}