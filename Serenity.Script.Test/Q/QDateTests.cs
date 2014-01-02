using QUnit;
using Serenity;
using System;

namespace Serenity.Test
{
    [TestFixture]
    public class QDateTests
    {
        [Test]
        public void FormatDateWorks()
        {
            var backupDec = Q.Culture.DateSeparator;
            try
            {
                Q.Culture.DateSeparator = "/";

                var date = new JsDate(2009, 0, 2, 3, 4, 5, 6); // 02.01.2009 03:04:05.006

                Assert.AreEqual(Q.FormatDate(date, "dd/MM/yyyy"), "02/01/2009", "'/': dd/MM/yyy");
                Assert.AreEqual(Q.FormatDate(date, "d/M/yy"), "2/1/09", "'/': d/M/yy");
                Assert.AreEqual(Q.FormatDate(date, "d.M.yyyy"), "2.1.2009", "'/': d.M.yyy");
                Assert.AreEqual(Q.FormatDate(date, "yyyyMMdd"), "20090102", "'/': yyyyMMdd");
                Assert.AreEqual(Q.FormatDate(date, "hh:mm tt"), "03:04 AM", "'/': hh:mm tt");
                Assert.AreEqual(Q.FormatDate(date, "yyyy-MM-ddTHH:mm:ss.fff"), "2009-01-02T03:04:05.006", "'/': yyyy-MM-ddTHH:mm:ss.fff");

                Q.Culture.DateSeparator = ".";

                Assert.AreEqual(Q.FormatDate(date, "dd/MM/yyyy"), "02.01.2009", "'.': dd/MM/yyy");
                Assert.AreEqual(Q.FormatDate(date, "d/M/yy"), "2.1.09", "'.': d/M/yy");
                Assert.AreEqual(Q.FormatDate(date, "d-M-yyyy"), "2-1-2009", "'.': d-M-yyy");
                Assert.AreEqual(Q.FormatDate(date, "yyyy-MM-ddTHH:mm:ss.fff"), "2009-01-02T03:04:05.006", "'.': yyyy-MM-ddTHH:mm:ss.fff");
            }
            finally
            {
                Q.Culture.DecimalSeparator = backupDec;
            }
        }
    }
}