using Serenity.Data;
using Serenity.Testing;
using Xunit;

namespace Serenity.Test.Testing
{
    public class DbScriptTests
    {
        [Fact]
        public void CanLoadCreateDatabaseScript()
        {
            Assert.True(new DbScript().GetResourceScript("Serenity.Testing.SqlScript.CreateDatabase.sql").Length > 0);
        }

        [Fact]
        public void CanAddScriptProperly()
        {
            var script = new DbScript();
            script.AddScript("HELLO");
            Assert.Equal(
                TestSqlHelper.Normalize("HELLO"), 
                TestSqlHelper.Normalize(script.ToString()));
        }

        [Fact]
        public void InsertsGoBetweenScripts()
        {
            var script = new DbScript();
            script.AddScript("FIRST;");
            script.AddScript("SECOND;");
            Assert.Equal(
                TestSqlHelper.Normalize("FIRST;GO\nSECOND;"), 
                TestSqlHelper.Normalize(script.ToString()));
        }

        [Fact]
        public void CanAddSqlInsertProperly()
        {
            var script = new DbScript();
            script.Add(new SqlInsert(new TestRow
            {
                TrackAssignments = true,
                TestId = 1,
                Description = "Test"
            }));

            Assert.Equal(TestSqlHelper.Normalize(
                "DECLARE @p1 INT = 1;" + 
                "DECLARE @p2 NVARCHAR(4) = 'Test';" + 
                "INSERT INTO Tests (TestId, Description) VALUES (@p1, @p2)"), 
                TestSqlHelper.Normalize(script.ToString()));
        }

        [Fact]
        public void CanAddSqlUpdateProperly()
        {
            var script = new DbScript();
            script.Add(new SqlUpdate(new TestRow
            {
                TestId = 1,
                Description = "Test"
            }));

            Assert.Equal(TestSqlHelper.Normalize(
                "DECLARE @p1 NVARCHAR(4) = 'Test';" +
                "DECLARE @p2 BIGINT = 1;" +
                "UPDATE Tests SET Description = @p1 WHERE TestId = @p2"),
                TestSqlHelper.Normalize(script.ToString()));
        }
    }
}