using FakeItEasy;
using Serenity.Abstractions;
using Serenity.Localization;
using Serenity.Testing;
using System;
using System.Globalization;
using System.Threading;
using Xunit;

namespace Serenity.Test
{
    public class InitializedLocalTextTests
    {
        [Fact]
        public void InitializedLocalText_ConstructorCallsBaseProperly()
        {
            var text = new InitializedLocalText("SomeKey", "SomeText");
            Assert.Equal("SomeKey", text.Key);
        }

        [Fact]
        public void InitializedLocalText_StoresInitialTextAsIs()
        {
            var text = new InitializedLocalText("SomeKey", "  ..SomeText  ");
            Assert.Equal("  ..SomeText  ", text.InitialText);
        }
    }
}