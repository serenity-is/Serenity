using QUnit;
using Serenity;
using System;
using System.Html;

namespace Serenity.Test
{
    [TestFixture]
    public class QCultureTests
    {
        [Test]
        public void CultureDateSeparatorCanBeSet()
        {
            // to check script name safety
            var qCulture = Window.Instance.As<dynamic>()["Q$Culture"];
            var backup = Q.Culture.DateSeparator;
            try
            {
                Q.Culture.DateSeparator = "$";
                Assert.AreEqual(qCulture.dateSeparator, "$", "Set by C#, read directly");

                qCulture.dateSeparator = "/";
                Assert.AreEqual(Q.Culture.DateSeparator, "/", "Set by C#, read directly");
            }
            finally
            {
                Q.Culture.DateSeparator = backup;
            }
        }

        [Test]
        public void CultureDateOrderCanBeSet()
        {
            // to check script name safety
            var qCulture = Window.Instance.As<dynamic>()["Q$Culture"];
            var backup = Q.Culture.DateOrder;
            try
            {
                Q.Culture.DateOrder = "myd";
                Assert.AreEqual(qCulture.dateOrder, "myd", "Set by C#, read directly");

                qCulture.dateOrder = "dmy";
                Assert.AreEqual(Q.Culture.DateOrder, "dmy", "Set by C#, read directly");
            }
            finally
            {
                Q.Culture.DateOrder = backup;
            }
        }

        [Test]
        public void CultureDateFormatCanBeSet()
        {
            // to check script name safety
            var qCulture = Window.Instance.As<dynamic>()["Q$Culture"];
            var backup = Q.Culture.DateFormat;
            try
            {
                Q.Culture.DateFormat = "%";
                Assert.AreEqual(qCulture.dateFormat, "%", "Set by C#, read directly");

                qCulture.dateFormat = ".";
                Assert.AreEqual(Q.Culture.DateFormat, ".", "Set by C#, read directly");
            }
            finally
            {
                Q.Culture.DateFormat = backup;
            }
        }

        [Test]
        public void CultureDateTimeFormatCanBeSet()
        {
            // to check script name safety
            var qCulture = Window.Instance.As<dynamic>()["Q$Culture"];
            var backup = Q.Culture.DateTimeFormat;
            try
            {
                Q.Culture.DateTimeFormat = "%";
                Assert.AreEqual(qCulture.dateTimeFormat, "%", "Set by C#, read directly");

                qCulture.dateTimeFormat = ".";
                Assert.AreEqual(Q.Culture.DateTimeFormat, ".", "Set by C#, read directly");
            }
            finally
            {
                Q.Culture.DateTimeFormat = backup;
            }
        }

        [Test]
        public void CultureDecimalSeparatorCanBeSet()
        {
            // to check script name safety
            var qCulture = Window.Instance.As<dynamic>()["Q$Culture"];
            var backup = Q.Culture.DecimalSeparator;
            try
            {
                Q.Culture.DecimalSeparator = "%";
                Assert.AreEqual(qCulture.decimalSeparator, "%", "Set by C#, read directly");

                qCulture.decimalSeparator = ".";
                Assert.AreEqual(Q.Culture.DecimalSeparator, ".", "Set by C#, read directly");
            }
            finally
            {
                Q.Culture.DecimalSeparator = backup;
            }
        }
    }
}