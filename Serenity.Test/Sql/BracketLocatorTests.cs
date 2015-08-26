using System;
using Xunit;

namespace Serenity.Data.Test
{
    public partial class BracketLocatorTests
    {
        [Fact]
        public void ReplaceBracketContents_IgnoresEmptyBrackets()
        {
            bool isCalled = false;
            var result = BracketLocator.ReplaceBracketContents("[]", '!', s =>
            {
                isCalled = true;
                return null;
            });
            Assert.Equal("[]", result);
            Assert.False(isCalled);
        }

        [Fact]
        public void ReplaceBracketContents_IgnoresDoubleEmptyBrackets()
        {
            bool isCalled = false;
            var result = BracketLocator.ReplaceBracketContents("[[]]", '!', s =>
            {
                isCalled = true;
                return null;
            });
            Assert.Equal("[[]]", result);
            Assert.False(isCalled);
        }


        [Fact]
        public void ReplaceBracketContents_IgnoresBracketsInsideQuotes()
        {
            bool isCalled = false;
            var result = BracketLocator.ReplaceBracketContents("'[a]'bc'd[e]'", '!', s =>
            {
                isCalled = true;
                return null;
            });
            Assert.Equal("'[a]'bc'd[e]'", result);
            Assert.False(isCalled);
        }

        [Fact]
        public void ReplaceBracketContents_HandlesSimpleBrackets()
        {
            int calls = 0;
            var result = BracketLocator.ReplaceBracketContents("[a]", '!', s =>
            {
                Assert.Equal("a", s);
                calls++;
                return "bcd";
            });
            Assert.Equal("[bcd]", result);
            Assert.Equal(1, calls);
        }

        [Fact]
        public void ReplaceBracketContents_KeepsUnchangedContents()
        {
            int calls = 0;
            var result = BracketLocator.ReplaceBracketContents("[a][b][cd]", '!', s =>
            {
                calls++;
                return s;
            });
            Assert.Equal("[a][b][cd]", result);
            Assert.Equal(3, calls);
        }
    }
}