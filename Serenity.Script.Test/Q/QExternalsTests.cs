using jQueryApi;
using QUnit;
using Serenity;
using System;

namespace Serenity.Test
{
    [TestFixture]
    public class QExternalsTests
    {
        [Test]
        public void TurkishLocaleCompareWorksProperly()
        {
            Assert.AreEqual(Q.Externals.TurkishLocaleCompare("a", "b"), -1);
            Assert.AreEqual(Q.Externals.TurkishLocaleCompare("b", "a"), 1);
            Assert.AreEqual(Q.Externals.TurkishLocaleCompare("b", "b"), 0);
            Assert.AreEqual(Q.Externals.TurkishLocaleCompare("i", "ı"), 1);
            Assert.AreEqual(Q.Externals.TurkishLocaleCompare("ı", "i"), -1);
            Assert.AreEqual(Q.Externals.TurkishLocaleCompare("İSTANBUL", "ıSpaRTA"), 1);
            Assert.AreEqual(Q.Externals.TurkishLocaleCompare("ıSpaRTA", "İSTANBUL"), -1);
            Assert.AreEqual(Q.Externals.TurkishLocaleCompare("0", "TEXT"), -1);
            Assert.AreEqual(Q.Externals.TurkishLocaleCompare("TEXT", "5"), 1);
            Assert.AreEqual(Q.Externals.TurkishLocaleCompare("123ABC", "123AB"), 1);
            Assert.AreEqual(Q.Externals.TurkishLocaleCompare("", null), 0);
            Assert.AreEqual(Q.Externals.TurkishLocaleCompare("_", null), 1);
            Assert.AreEqual(Q.Externals.TurkishLocaleCompare(null, "X"), -1);
        }
    }
}