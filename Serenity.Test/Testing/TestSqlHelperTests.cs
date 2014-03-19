using Serenity.Testing;
using Xunit;

namespace Serenity.Test.Testing
{
    public class TestSqlHelperTests
    {
        [Fact]
        public void NormalizeRemovesSpacesAndLineEndsAtStartOrEnd()
        {
            Assert.Equal(
                "SELECT FROM",
                TestSqlHelper.Normalize("   \r\n\nSELECT FROM   \n\r\n"));
        }

        [Fact]
        public void NormalizeRemovesDoubleSpaces()
        {
            Assert.Equal(
                "SELECT FROM",
                TestSqlHelper.Normalize("SELECT  FROM")); 
        }

        [Fact]
        public void NormalizeRemovesTripleSpaces()
        {
            Assert.Equal(
                "SELECT FROM",
                TestSqlHelper.Normalize("SELECT   FROM"));
        }

        [Fact]
        public void NormalizeConvertsLineEndsToSpaces()
        {
            Assert.Equal(
                "SELECT FROM",
                TestSqlHelper.Normalize("SELECT \r\n   FROM"));
        }

        [Fact]
        public void NormalizeRemovesSpacesAndLineEndsAfterDoubleComma()
        {
            Assert.Equal(
                "SELECT FROM;TEST STATEMENT 2;TEST STATEMENT 3;",
                TestSqlHelper.Normalize("SELECT FROM;   TEST STATEMENT 2;\n   TEST STATEMENT 3;  "));
        }
    }
}