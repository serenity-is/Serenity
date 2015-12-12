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

        [Fact]
        public void ReplaceBrackets_IgnoresEmptyBrackets()
        {
            Assert.Equal("[]", BracketLocator.ReplaceBrackets("[]", '"', '"'));
            Assert.Equal("xyz []", BracketLocator.ReplaceBrackets("xyz []", '"', '"'));
            Assert.Equal("[] abc", BracketLocator.ReplaceBrackets("[] abc", '"', '"'));
            Assert.Equal("u [] w [] []", BracketLocator.ReplaceBrackets("u [] w [] []", '"', '"'));
        }

        [Fact]
        public void ReplaceBrackets_IgnoresBracketsPrecededWithLettersOrNumbers()
        {
            Assert.Equal("a[b]", BracketLocator.ReplaceBrackets("a[b]", '"', '"'));
            Assert.Equal("xyz b[e]", BracketLocator.ReplaceBrackets("xyz b[e]", '"', '"'));
            Assert.Equal("0[f] abc", BracketLocator.ReplaceBrackets("0[f] abc", '"', '"'));
            Assert.Equal("u k[x] w 9[j] _[r]", BracketLocator.ReplaceBrackets("u k[x] w 9[j] _[r]", '"', '"'));
        }

        [Fact]
        public void ReplaceBrackets_IgnoresBracketsFollowedWithLettersOrNumbers()
        {
            Assert.Equal("[b]a", BracketLocator.ReplaceBrackets("[b]a", '"', '"'));
            Assert.Equal("xyz [e]b", BracketLocator.ReplaceBrackets("xyz [e]b", '"', '"'));
            Assert.Equal("[f]0 abc", BracketLocator.ReplaceBrackets("[f]0 abc", '"', '"'));
            Assert.Equal("u [x]k w [j]9 [r]_", BracketLocator.ReplaceBrackets("u [x]k w [j]9 [r]_", '"', '"'));
        }

        [Fact]
        public void ReplaceBrackets_IgnoresBracketsInStrings()
        {
            Assert.Equal("'[b]'", BracketLocator.ReplaceBrackets("'[b]'", '"', '"'));
            Assert.Equal("'sfg [c] ab [x]'", BracketLocator.ReplaceBrackets("'sfg [c] ab [x]'", '"', '"'));
        }

        [Fact]
        public void ReplaceBrackets_IgnoresBracketsWithNumericContents()
        {
            Assert.Equal("[0]", BracketLocator.ReplaceBrackets("[0]", '"', '"'));
            Assert.Equal("[77][89]", BracketLocator.ReplaceBrackets("[77][89]", '"', '"'));
        }

        [Fact]
        public void ReplaceBrackets_ReplacesSimpleIdentifiers()
        {
            Assert.Equal("\"a\"", BracketLocator.ReplaceBrackets("[a]", '"', '"'));
            Assert.Equal("x.`y` z.d", BracketLocator.ReplaceBrackets("x.[y] z.d", '`', '`'));
        }

        [Fact]
        public void ReplaceBrackets_ReplacesIdentifiersWithSpaces()
        {
            Assert.Equal("\"Order Details\"", BracketLocator.ReplaceBrackets("[Order Details]", '"', '"'));
            Assert.Equal("SELECT c.`Some Field` from Customers c", 
                BracketLocator.ReplaceBrackets("SELECT c.[Some Field] from Customers c", '`', '`'));
        }
    }
}