using jQueryApi;
using QUnit;
using Serenity;
using System;

namespace Serenity.Test
{
    [TestFixture]
    public class FormatterTypeRegistryTests : ScriptContext
    {
        [Test]
        public void FormatterTypeRegistry_CanLocateSerenityFormatters()
        {
            Assert.AreStrictEqual(typeof(EnumFormatter), FormatterTypeRegistry.Get("Enum"), "shortest");
            Assert.AreStrictEqual(typeof(EnumFormatter), FormatterTypeRegistry.Get("EnumFormatter"), "with suffix");
            Assert.AreStrictEqual(typeof(EnumFormatter), FormatterTypeRegistry.Get("Serenity.Enum"), "with namespace no suffix");
            Assert.AreStrictEqual(typeof(EnumFormatter), FormatterTypeRegistry.Get("Serenity.EnumFormatter"), "with namespace and suffix");
        }

        [Test]
        public void FormatterTypeRegistry_CanLocateDummyFormatter()
        {
            Assert.AreStrictEqual(typeof(FormatterTypeRegistryTestNamespace.DummyFormatter),
                FormatterTypeRegistry.Get("FormatterTypeRegistryTestNamespace.Dummy"), 
                    "with namespace and no suffix");
            
            Assert.AreStrictEqual(typeof(FormatterTypeRegistryTestNamespace.DummyFormatter),
                FormatterTypeRegistry.Get("FormatterTypeRegistryTestNamespace.DummyFormatter"), 
                    "with namespace and suffix");

            Assert.Throws<Exception>(() =>
            {
                FormatterTypeRegistry.Get("DummyFormatter"); 
            }, "can't find if no root namespace");

            Q.Config.RootNamespaces.Add("FormatterTypeRegistryTestNamespace");
            try
            {
                FormatterTypeRegistry.Reset();

                Assert.AreStrictEqual(typeof(FormatterTypeRegistryTestNamespace.DummyFormatter),
                    FormatterTypeRegistry.Get("DummyFormatter"),
                        "can find if root namespace and suffix");

                Assert.AreStrictEqual(typeof(FormatterTypeRegistryTestNamespace.DummyFormatter),
                    FormatterTypeRegistry.Get("Dummy"),
                        "can find if root namespace and no suffix");
            }
            finally
            {
                Q.Config.RootNamespaces.Remove("FormatterTypeRegistryTestNamespace");
                FormatterTypeRegistry.Reset();
            }
        }
    }
}

namespace FormatterTypeRegistryTestNamespace
{
    public class DummyFormatter : ISlickFormatter
    {
        public string Format(SlickFormatterContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}