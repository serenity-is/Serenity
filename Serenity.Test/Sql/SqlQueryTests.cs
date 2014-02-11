using Serenity.Testing.Test;
using Xunit;

namespace Serenity.Data.Test
{
    public partial class SqlQueryTests
    {
        [Fact]
        public void DistinctAddsKeyword()
        {
            var query = new SqlQuery()
                .Distinct(true)
                .Select("TestColumn")
                .From("TestTable");

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT DISTINCT TestColumn FROM TestTable"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void DistinctCanBeTurnedOff()
        {
            var query = new SqlQuery()
                .Distinct(true)
                .Select("TestColumn")
                .From("TestTable")
                .Distinct(false);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT TestColumn FROM TestTable"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void FromMultipleCallsDoesCrossJoin()
        {
            var query = new SqlQuery()
                .From("TestTable1")
                .From("TestTable2")
                .Select("TestColumn");

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT TestColumn FROM TestTable1, TestTable2"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void SubQueryShouldBeEnclosedInParen()
        {
            var sub = new SqlQuery().SubQuery()
                .Select("TestColumn")
                .From("TestTable");

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "(SELECT TestColumn FROM TestTable)"),
                TestSqlHelper.Normalize(
                    sub.ToString()));
        }

        [Fact]
        public void SubQuerySharesParameters()
        {
            var query = new SqlQuery();
            Assert.Equal(0, query.ParamCount);

            var sub = query.SubQuery();
            sub.AddParam("@px1", "value");
            Assert.Equal(1, query.ParamCount);
            Assert.Equal((string)query.Params["@px1"], "value");
        }

        [Fact]
        public void WithPassesAndReturnsTheQueryItself()
        {
            var query = new SqlQuery();
            var afterWith = query.With(insideWidth =>
            {
                Assert.Equal(query, insideWidth);
            });

            Assert.Equal(query, afterWith);
        }

        [Fact]
        public void SubQueryCanBeUsedAsCriteriaUsingVar()
        {
            var query = new SqlQuery()
                .From("ParentTable")
                .Select("ParentColumn");

            query.Where(new Criteria(query.SubQuery()
                .From("SubTable")
                .Take(1)
                .Select("SubColumn")) >= 1);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT ParentColumn FROM ParentTable WHERE " +
                        "(SELECT TOP 1 SubColumn FROM SubTable) >= @p1"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void SubQueryCanBeUsedAsCriteriaUsingWith()
        {
            var query = new SqlQuery()
                .From("ParentTable")
                .Select("ParentColumn")
                .With(me => me.Where(new Criteria(me.SubQuery()
                    .From("SubTable")
                    .Take(1)
                    .Select("SubColumn")) >= 1));

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT ParentColumn FROM ParentTable WHERE " +
                        "(SELECT TOP 1 SubColumn FROM SubTable) >= @p1"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }
    }
}