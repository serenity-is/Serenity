using QUnit;
using Serenity;
using System;
using System.Html;

namespace Serenity.Test
{
    [TestFixture]
    public class QHtmlTests
    {
        [Test]
        public void HtmlEncodeWorks()
        {
            Assert.AreEqual(Q.HtmlEncode(null), "");
            Assert.AreEqual(Q.HtmlEncode("a"), "a");
            Assert.AreEqual(Q.HtmlEncode("cdef"), "cdef");
            Assert.AreEqual(Q.HtmlEncode("<"), "&lt;");
            Assert.AreEqual(Q.HtmlEncode(">"), "&gt;");
            Assert.AreEqual(Q.HtmlEncode("&"), "&amp;");
            Assert.AreEqual(Q.HtmlEncode("if (a < b && c > d) x = 5 & 3 else y = (u <> w)"), 
                "if (a &lt; b &amp;&amp; c &gt; d) x = 5 &amp; 3 else y = (u &lt;&gt; w)");

            var q = Window.Instance.As<dynamic>()["Q"];
            Assert.AreEqual(q.htmlEncode("<script&>"), "&lt;script&amp;&gt;", 
                "check direct script access");
        }
    }
}