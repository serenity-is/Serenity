using Xunit;

namespace Serenity.Data.Test
{
    public partial class DatabaseReferenceReplacerTests
    {
        [Fact]
        public void Replace_IgnoresExpressionsWithoutCaret()
        {
            SqlConnections.SetConnection("Dummy1", 
                "Data Source=server1; Initial Catalog=catalog1",
                "System.Data.SqlClient");
            var result = DatabaseBracketReferences.Replace("[abc].[dbo].[Table]");
            Assert.Equal(result, "[abc].[dbo].[Table]");
        }

        [Fact]
        public void Replace_HandlesExpressionsWithCaret()
        {
            SqlConnections.SetConnection("Dummy1",
                "Data Source=server1; Initial Catalog=catalog1",
                "System.Data.SqlClient");
            var result = DatabaseBracketReferences.Replace("[Dummy1^].[dbo].[Table]");
            Assert.Equal(result, "[catalog1].[dbo].[Table]");
        }

        [Fact]
        public void Replace_IgnoresIfNoConnectionIsSpecified()
        {
            SqlConnections.SetConnection("Dummy1",
                "Data Source=server1; Initial Catalog=catalog1",
                "System.Data.SqlClient");
            var result = DatabaseBracketReferences.Replace("[^].[dbo].[Table]");
            Assert.Equal(result, "[^].[dbo].[Table]");
        }

        [Fact]
        public void Replace_UsesAlternateDatabaseNameIfSpecifiedAndConnectionNotFound()
        {
            SqlConnections.SetConnection("Nevermind",
                "Data Source=server1; Initial Catalog=catalog1",
                "System.Data.SqlClient");
            var result = DatabaseBracketReferences.Replace("[InvalidConnectionKey^AlternateDB].[dbo].[Table]");
            Assert.Equal(result, "[AlternateDB].[dbo].[Table]");
        }

        [Fact]
        public void Replace_HandlesComplexExpressions()
        {
            SqlConnections.SetConnection("MyDB",
                "Data Source=server1; Initial Catalog=MyMy",
                "System.Data.SqlClient");
            var result = DatabaseBracketReferences.Replace(
                "SELECT A, B, C FROM [MyDB^X].dbo.[Table] WHERE x > '[MyDB^X]'");
            Assert.Equal("SELECT A, B, C FROM [MyMy].dbo.[Table] WHERE x > '[MyDB^X]'", 
                result);
        }
    }
}