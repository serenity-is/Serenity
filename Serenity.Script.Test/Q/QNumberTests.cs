using QUnit;
using Serenity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serenity.Test
{
    [TestFixture]
    public class QNumberTests
    {
        [Test]
        public void FormatNumberWorks()
        {
            Assert.AreEqual(Q.FormatNumber(1.0, "1"), "1");

            var backupDec = Q.Culture.DecimalSeparator;
            try
            {
                Q.Culture.DecimalSeparator = ",";
                Assert.AreEqual(Q.FormatNumber(1.0, "0.00"), "1,00");
                Assert.AreEqual(Q.FormatNumber(1.0, "0.0000"), "1,0000");
                Assert.AreEqual(Q.FormatNumber(1234, "#,##0"), "1.234");
                Assert.AreEqual(Q.FormatNumber(1234.5, "#,##0.##"), "1.234,5");
                Assert.AreEqual(Q.FormatNumber(1234.5678, "#,##0.##"), "1.234,57");
                Assert.AreEqual(Q.FormatNumber(1234.5, "#,##0.00"), "1.234,50");

                Q.Culture.DecimalSeparator = ".";
                Assert.AreEqual(Q.FormatNumber(1234, "#,##0"), "1,234");
                Assert.AreEqual(Q.FormatNumber(1234.5, "#,##0.##"), "1,234.5");
                Assert.AreEqual(Q.FormatNumber(1234.5678, "#,##0.##"), "1,234.57");
                Assert.AreEqual(Q.FormatNumber(1234.5, "#,##0.00"), "1,234.50");
            }
            finally
            {
                Q.Culture.DecimalSeparator = backupDec;
            }
        }
    }
}