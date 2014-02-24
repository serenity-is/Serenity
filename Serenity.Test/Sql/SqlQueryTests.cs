using Serenity.Data;
using Serenity.Test.Testing;
using System;
using Xunit;

namespace Serenity.Test.Data
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
        public void FromMixedOverloadsMultipleCallsDoesCrossJoin()
        {
            var query = new SqlQuery()
                .From("TestTable1", new Alias("x1"))
                .From(new Alias("TestTable2", "x2"))
                .From("TestTable3")
                .Select("TestColumn");

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT TestColumn FROM TestTable1 x1, TestTable2 x2, TestTable3"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void FromWithNullOrEmptyArgumentsThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().From((string)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().From(""));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().From((Alias)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().From((string)null, (Alias)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().From("", (Alias)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().From("x", (Alias)null));
        }

        [Fact]
        public void FromWithAliasWithoutTableNameThrowsArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                new SqlQuery().From(new Alias("TestAlias"));
            });
        }

        [Fact]
        public void FromWithTableNameAndAliasWorks()
        {
            var query = new SqlQuery()
                .From("TestTable", new Alias("TestAlias"))
                .Select("TestColumn");

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT TestColumn FROM TestTable TestAlias"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void FromWithOnlyAliasWorks()
        {
            var query = new SqlQuery()
                .From(new Alias("TestTable", "TestAlias"))
                .Select("TestColumn");

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT TestColumn FROM TestTable TestAlias"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void FromWithSubQueryAndAliasWorks()
        {
            var query = new SqlQuery().With(me => me
                .From(me.SubQuery()
                    .From("SubTable")
                    .Select("SubColumn"), new Alias("sub"))
                .Select("SubColumn"));

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT SubColumn FROM (SELECT SubColumn FROM SubTable) sub"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void GetExpressionReturnsNullIfNoSuchColumn()
        {
            var query = new SqlQuery()
                .Select("SomeColumn")
                .From("SomeTable");

            Assert.Null(query.GetExpression("OtherColumn"));
        }

        [Fact]
        public void GetExpressionWorksWithNonAliasedColumns()
        {
            var query = new SqlQuery()
                .Select("SomeColumn")
                .From("SomeTable");

            Assert.Equal(
                "SomeColumn",
                query.GetExpression("SomeColumn"));
        }

        [Fact]
        public void GetExpressionWorksWithAliasedColumns()
        {
            var query = new SqlQuery()
                .Select("SomeColumn", "x")
                .From("SomeTable");

            Assert.Equal(
                "SomeColumn",
                query.GetExpression("x"));
        }

        [Fact]
        public void GetExpressionShouldNotUseOriginalExpressionIfItIsAliased()
        {
            var query = new SqlQuery()
                .Select("SomeColumn", "x")
                .From("SomeTable");

            Assert.Null(query.GetExpression("SomeColumn"));
        }

        [Fact]
        public void GetExpressionDoesntWorkWithAliasDotFieldnameAndNoColumnName()
        {
            // not sure if we could make a workaround for this case (e.g. parse expression and find expected column name?)
            var query = new SqlQuery()
                .Select(new Alias("x"), "SomeColumn")
                .From("SomeTable");

            Assert.Null(query.GetExpression("SomeColumn"));
        }

        [Fact]
        public void GetExpressionWithIndexWorks()
        {
            var query = new SqlQuery()
                .Select(new Alias("x"), "SomeColumn", "c1")
                .Select("a.b", "c2")
                .Select("c", "c3")
                .From("SomeTable");

            Assert.Equal("x.SomeColumn", query.GetExpression(0));
            Assert.Equal("a.b", query.GetExpression(1));
            Assert.Equal("c", query.GetExpression(2));
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => query.GetExpression(3));
        }

        [Fact]
        public void GetExpressionWorksWithAliasDotFieldnameAndColumnName()
        {
            // not sure if we could make a workaround for these
            var query = new SqlQuery()
                .Select(new Alias("x"), "SomeColumn", "SomeColumn")
                .From("SomeTable");

            Assert.Equal("x.SomeColumn", query.GetExpression("SomeColumn"));
        }

        [Fact]
        public void GroupByWithExpressionWorks()
        {
            var query = new SqlQuery()
                .Select("TestColumn")
                .From("TestTable")
                .GroupBy("TestColumn")
                .GroupBy("TestColumn2");

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT TestColumn FROM TestTable GROUP BY TestColumn, TestColumn2"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void GroupByWithAliasAndFieldnameWorks()
        {
            var query = new SqlQuery()
                .Select("u.TestColumn")
                .From("TestTable u")
                .GroupBy(new Alias("u"), "TestColumn")
                .GroupBy(new Alias("u"), "TestColumn2");

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT u.TestColumn FROM TestTable u GROUP BY u.TestColumn, u.TestColumn2"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void HavingWithExpressionWorks()
        {
            var query = new SqlQuery()
                .Select("TestColumn")
                .From("TestTable")
                .GroupBy("TestColumn")
                .Having("Count(*) > 5");

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT TestColumn FROM TestTable GROUP BY TestColumn HAVING Count(*) > 5"),
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