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
    }
}