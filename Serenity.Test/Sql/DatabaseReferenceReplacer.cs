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
            var result = DatabaseCaretReferences.Replace("[abc].[dbo].[Table]");
            Assert.Equal("[abc].[dbo].[Table]", result);
        }

        [Fact]
        public void Replace_HandlesExpressionsWithCaret()
        {
            SqlConnections.SetConnection("Dummy1",
                "Data Source=server1; Initial Catalog=catalog1",
                "System.Data.SqlClient");
            var result = DatabaseCaretReferences.Replace("[Dummy1^].[dbo].[Table]");
            Assert.Equal("[catalog1].[dbo].[Table]", result);
        }

        [Fact]
        public void Replace_IgnoresIfNoConnectionIsSpecified()
        {
            SqlConnections.SetConnection("Dummy1",
                "Data Source=server1; Initial Catalog=catalog1",
                "System.Data.SqlClient");
            var result = DatabaseCaretReferences.Replace("[^].[dbo].[Table]");
            Assert.Equal("[^].[dbo].[Table]", result);
        }

        [Fact]
        public void Replace_UsesAlternateDatabaseNameIfSpecifiedAndConnectionNotFound()
        {
            SqlConnections.SetConnection("Nevermind",
                "Data Source=server1; Initial Catalog=catalog1",
                "System.Data.SqlClient");
            var result = DatabaseCaretReferences.Replace("[InvalidConnectionKey^AlternateDB].[dbo].[Table]");
            Assert.Equal("[AlternateDB].[dbo].[Table]", result);
        }

        [Fact]
        public void Replace_HandlesComplexExpressions()
        {
            SqlConnections.SetConnection("MyDB",
                "Data Source=server1; Initial Catalog=MyMy",
                "System.Data.SqlClient");
            var result = DatabaseCaretReferences.Replace(
                "SELECT A, B, C FROM [MyDB^X].dbo.[Table] WHERE x > '[MyDB^X]'");
            Assert.Equal("SELECT A, B, C FROM [MyMy].dbo.[Table] WHERE x > '[MyDB^X]'", 
                result);
        }
    }
}