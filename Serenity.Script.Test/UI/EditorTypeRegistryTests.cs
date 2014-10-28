using jQueryApi;
using QUnit;
using Serenity;
using System;

namespace Serenity.Test
{
    [TestFixture]
    public class EditorTypeRegistryTests : ScriptContext
    {
        [Test]
        public void EditorTypeRegistry_CanLocateSerenityEditors()
        {
            Assert.AreStrictEqual(typeof(StringEditor), EditorTypeRegistry.Get("String"), "shortest");
            Assert.AreStrictEqual(typeof(StringEditor), EditorTypeRegistry.Get("StringEditor"), "with editor suffix");
            Assert.AreStrictEqual(typeof(StringEditor), EditorTypeRegistry.Get("Serenity.String"), "with namespace no suffix");
            Assert.AreStrictEqual(typeof(StringEditor), EditorTypeRegistry.Get("Serenity.StringEditor"), "with namespace and suffix");
        }

        [Test]
        public void EditorTypeRegistry_CanLocateDummyEditor()
        {
            Assert.AreStrictEqual(typeof(EditorTypeRegistryTestNamespace.DummyEditor),
                EditorTypeRegistry.Get("SomeOtherKeyForDummyEditor"), 
                    "with editor key");

            Assert.AreStrictEqual(typeof(EditorTypeRegistryTestNamespace.DummyEditor),
                EditorTypeRegistry.Get("EditorTypeRegistryTestNamespace.Dummy"), 
                    "with namespace and no suffix");
            
            Assert.AreStrictEqual(typeof(EditorTypeRegistryTestNamespace.DummyEditor),
                EditorTypeRegistry.Get("EditorTypeRegistryTestNamespace.DummyEditor"), 
                    "with namespace and suffix");

            Assert.Throws<Exception>(() =>
            {
                EditorTypeRegistry.Get("DummyEditor"); 
            }, "can't find if no root namespace");

            Q.Config.RootNamespaces.Add("EditorTypeRegistryTestNamespace");
            try
            {
                EditorTypeRegistry.Reset();

                Assert.AreStrictEqual(typeof(EditorTypeRegistryTestNamespace.DummyEditor),
                    EditorTypeRegistry.Get("DummyEditor"),
                        "can find if root namespace and suffix");

                Assert.AreStrictEqual(typeof(EditorTypeRegistryTestNamespace.DummyEditor),
                    EditorTypeRegistry.Get("Dummy"),
                        "can find if root namespace and no suffix");
            }
            finally
            {
                Q.Config.RootNamespaces.Remove("EditorTypeRegistryTestNamespace");
                EditorTypeRegistry.Reset();
            }
        }
    }
}

namespace EditorTypeRegistryTestNamespace
{
    [Editor(Key = "SomeOtherKeyForDummyEditor")]
    public class DummyEditor : Widget
    {
        public DummyEditor(jQueryObject element)
            : base(element)
        {
        }
    }
}