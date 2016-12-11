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
            var backupDateFormat = Q.Culture.DateFormat;
            var backupDateTimeFormat = Q.Culture.DateTimeFormat;
            try
            {
                Q.Culture.DateSeparator = "/";
                Q.Culture.DateFormat = "dd/MM/yyyy";
                Q.Culture.DateTimeFormat = "dd/MM/yyyy HH:mm:ss";

                var date = new JsDate(2029, 0, 2, 3, 4, 5, 6); // 02.01.2029 03:04:05.006

                Assert.AreEqual(Q.FormatDate(date, "dd/MM/yyyy"), "02/01/2029", "'/': dd/MM/yyy");
                Assert.AreEqual(Q.FormatDate(date, "d/M/yy"), "2/1/29", "'/': d/M/yy");
                Assert.AreEqual(Q.FormatDate(date, "d.M.yyyy"), "2.1.2029", "'/': d.M.yyy");
                Assert.AreEqual(Q.FormatDate(date, "yyyyMMdd"), "20290102", "'/': yyyyMMdd");
                Assert.AreEqual(Q.FormatDate(date, "hh:mm tt"), "03:04 AM", "'/': hh:mm tt");
                Assert.AreEqual(Q.FormatDate(date, "yyyy-MM-ddTHH:mm:ss.fff"), "2029-01-02T03:04:05.006", "'/': yyyy-MM-ddTHH:mm:ss.fff");
                Assert.AreEqual(Q.FormatDate(date, "d"), "02/01/2029", "'/': d");
                Assert.AreEqual(Q.FormatDate(date, "g"), "02/01/2029 03:04", "'/': g");
                Assert.AreEqual(Q.FormatDate(date, "G"), "02/01/2029 03:04:05", "'/': G");
                Assert.AreEqual(Q.FormatDate(date, "s"), "2029-01-02T03:04:05", "'/': s");
                Assert.AreEqual(Q.FormatDate(date, "u"), Q.FormatISODateTimeUTC(date), "'/': u");

                Q.Culture.DateSeparator = ".";

                Assert.AreEqual(Q.FormatDate(date, "dd/MM/yyyy"), "02.01.2029", "'.': dd/MM/yyy");
                Assert.AreEqual(Q.FormatDate(date, "d/M/yy"), "2.1.29", "'.': d/M/yy");
                Assert.AreEqual(Q.FormatDate(date, "d-M-yyyy"), "2-1-2029", "'.': d-M-yyy");
                Assert.AreEqual(Q.FormatDate(date, "yyyy-MM-ddTHH:mm:ss.fff"), "2029-01-02T03:04:05.006", "'.': yyyy-MM-ddTHH:mm:ss.fff");
                Assert.AreEqual(Q.FormatDate(date, "g"), "02.01.2029 03:04", "'.': g");
                Assert.AreEqual(Q.FormatDate(date, "G"), "02.01.2029 03:04:05", "'.': G");
                Assert.AreEqual(Q.FormatDate(date, "s"), "2029-01-02T03:04:05", "'.': s");
                Assert.AreEqual(Q.FormatDate(date, "u"), Q.FormatISODateTimeUTC(date), "'.': u");
            }
            finally
            {
                Q.Culture.DecimalSeparator = backupDec;
                Q.Culture.DateFormat = backupDateFormat;
                Q.Culture.DateTimeFormat = backupDateTimeFormat;
            }
        }
    }
}