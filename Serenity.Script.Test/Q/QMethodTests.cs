using jQueryApi;
using QUnit;
using Serenity;
using System;

namespace Serenity.Test
{
    [TestFixture]
    public class QMethodTests
    {
        [Test]
        public void ToJSONWorks()
        {
            Assert.AreEqual(Q.ToJSON(12345), "12345", "Number");

            Assert.AreEqual(Q.ToJSON("abcd\"'e"), "\"abcd\\\"'e\"", "String");

            var date = new JsDate(2013, 12, 27, 16, 19, 35, 345);
            Assert.AreEqual(Q.ToJSON(date), "\"" + Q.Externals.FormatISODateTimeUTC(date) + "\"", "Date/Time");

            Assert.AreEqual(Q.ToJSON(12345.6780), "12345.678", "Double");

            object o = new { num = 5, str = "abc", date = date };
            
            string json = Q.ToJSON(o);
            Assert.IsTrue(Script.TypeOf(json) == "string", "Ensure serialized object is string");

            var deserialized = jQuery.ParseJson(json);
            o.As<dynamic>().date = Q.Externals.FormatISODateTimeUTC(date);
            Assert.AreEqual(deserialized, o, "Compare original object and deserialization");
        }

        [Test]
        public void IsTrueWorks()
        {
            Assert.AreEqual(Q.IsTrue(1), true, "1 is true");
            Assert.AreEqual(Q.IsTrue(0), false, "0 is false");
            Assert.AreEqual(Q.IsTrue(null), false, "null is false");
            Assert.AreEqual(Q.IsTrue(Script.Undefined), false, "undefined is false");
            Assert.AreEqual(Q.IsTrue("0"), true, "'0' is true");
            Assert.AreEqual(Q.IsTrue("1"), true, "'1' is true");
            Assert.AreEqual(Q.IsTrue("-1"), true, "'-1' is true");
            Assert.AreEqual(Q.IsTrue("xysa"), true, "any other value is true");
        }

        [Test]
        public void IsFalseWorks()
        {
            Assert.AreEqual(Q.IsFalse(1), false, "1 is false");
            Assert.AreEqual(Q.IsFalse(0), true, "0 is true");
            Assert.AreEqual(Q.IsFalse(null), true, "null is true");
            Assert.AreEqual(Q.IsFalse(Script.Undefined), true, "undefined is true");
            Assert.AreEqual(Q.IsFalse("0"), false, "'0' is false");
            Assert.AreEqual(Q.IsFalse("-1"), false, "-1 is false");
            Assert.AreEqual(Q.IsFalse("xysa"), false, "any other value is false");
        }
    }
}