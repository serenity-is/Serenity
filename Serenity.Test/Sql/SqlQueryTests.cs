using Serenity.Data;
using Serenity.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Serenity.Test.Data
{
    public partial class SqlQueryTests
    {
        private class MyField : IField
        {
            public string Name { get; set; }
            public string Expression { get; set; }
            public string ColumnAlias { get; set; }
        }

        private class MyEntity : IEntity
        {
            public string Table { get; set; }
        }

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
        public void FirstIntoRowShouldReturnNullIfNoEntityUsedYet()
        {
            Assert.Equal(null, ((ISqlQueryExtensible)new SqlQuery()).FirstIntoRow);
        }

        [Fact]
        public void FirstIntoRowShouldReturnFirstRowIfMoreThanOneEntityUsed()
        {
            var first = new MyEntity() { Table = "x" };
            var second = new MyEntity() { Table = "y" };
            var query = new SqlQuery().From(first).From(second.Table, Alias.T1).Into(second);
            Assert.Equal(first, ((ISqlQueryExtensible)query).FirstIntoRow);
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
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().From((IEntity)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().From((IEntity)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().From((ISqlQuery)null, new Alias("x")));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().From(new SqlQuery(), (Alias)null));
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
        public void FromThrowsArgumentOutOfRangeIfSameAliasUsedTwice()
        {
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                new SqlQuery()
                    .From("TestTable", new Alias("x"))
                    .From("AnotherTable", new Alias("x"));
            });
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
        public void GetExpressionThrowsArgumentNullIfColumnNameIsNull()
        {
            var query = new SqlQuery()
                .Select("SomeColumn")
                .From("SomeTable");

            Assert.Throws<ArgumentNullException>(() => ((IGetExpressionByName)query).GetExpression(null));
        }

        [Fact]
        public void GetExpressionReturnsNullIfNoSuchColumn()
        {
            var query = new SqlQuery()
                .Select("SomeColumn")
                .From("SomeTable");

            Assert.Null(((IGetExpressionByName)query).GetExpression("OtherColumn"));
        }

        [Fact]
        public void GetExpressionWorksWithNonAliasedColumns()
        {
            var query = new SqlQuery()
                .Select("SomeColumn")
                .From("SomeTable");

            Assert.Equal(
                "SomeColumn",
                ((IGetExpressionByName)query).GetExpression("SomeColumn"));
        }

        [Fact]
        public void GetExpressionWorksWithAliasedColumns()
        {
            var query = new SqlQuery()
                .Select("SomeColumn", "x")
                .From("SomeTable");

            Assert.Equal(
                "SomeColumn",
                ((IGetExpressionByName)query).GetExpression("x"));
        }

        [Fact]
        public void GetExpressionShouldNotUseOriginalExpressionIfItIsAliased()
        {
            var query = new SqlQuery()
                .Select("SomeColumn", "x")
                .From("SomeTable");

            Assert.Null(((IGetExpressionByName)query).GetExpression("SomeColumn"));
        }

        [Fact]
        public void GetExpressionDoesntWorkWithAliasDotFieldnameAndNoColumnName()
        {
            // not sure if we could make a workaround for this case (e.g. parse expression and find expected column name?)
            var query = new SqlQuery()
                .Select(new Alias("x"), "SomeColumn")
                .From("SomeTable");

            Assert.Null(((IGetExpressionByName)query).GetExpression("SomeColumn"));
        }

        [Fact]
        public void GetExpressionWithIndexWorks()
        {
            var query = new SqlQuery()
                .Select(new Alias("x"), "SomeColumn", "c1")
                .Select("a.b", "c2")
                .Select("c", "c3")
                .From("SomeTable");

            Assert.Equal("x.SomeColumn", ((ISqlQueryExtensible)query).Columns[0].Expression);
            Assert.Equal("a.b", ((ISqlQueryExtensible)query).Columns[1].Expression);
            Assert.Equal("c", ((ISqlQueryExtensible)query).Columns[2].Expression);
        }

        [Fact]
        public void GetExpressionsWithOutOfBoundsIndexThrowsArgumentOutOfRange()
        {
            var query = new SqlQuery().Select("a").Select("b");
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => ((ISqlQueryExtensible)query).Columns[2].Expression);
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => ((ISqlQueryExtensible)query).Columns[-1].Expression);
        }

        [Fact]
        public void GetExpressionWorksWithAliasDotFieldnameAndColumnName()
        {
            // not sure if we could make a workaround for these
            var query = new SqlQuery()
                .Select(new Alias("x"), "SomeColumn", "SomeColumn")
                .From("SomeTable");

            Assert.Equal("x.SomeColumn", ((IGetExpressionByName)query).GetExpression("SomeColumn"));
        }

        [Fact]
        public void GetExpressionReturnsColumnNameIfNoExpression()
        {
            var query = new SqlQuery()
                .Select("SomeColumn");

            Assert.Equal("SomeColumn", ((IGetExpressionByName)query).GetExpression("SomeColumn"));
        }

        [Fact]
        public void GroupByWithEmptyOrNullArgumentsThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().GroupBy((string)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().GroupBy(String.Empty));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().GroupBy((MyField)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().GroupBy((Alias)null, "x"));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().GroupBy(new Alias("x"), (string)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().GroupBy(new Alias("x"), String.Empty));
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
        public void HavingWithEmptyOrNullArgumentsThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Having((string)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Having(String.Empty));
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
        public void OrderByWithEmptyOrNullArgumentsThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderBy((string)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderBy(String.Empty));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderBy((MyField)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderBy((Alias)null, "x"));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderBy(new Alias("x"), (string)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderBy(new Alias("x"), String.Empty));
        }

        [Fact]
        public void OrderByWithExpressionWorks()
        {
            var query = new SqlQuery()
                .Select("TestColumn")
                .From("TestTable")
                .OrderBy("TestColumn")
                .OrderBy("TestColumn2");

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT TestColumn FROM TestTable ORDER BY TestColumn, TestColumn2"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void OrderByWithAliasAndFieldnameWorks()
        {
            var query = new SqlQuery()
                .Select("u.TestColumn")
                .From("TestTable u")
                .OrderBy(new Alias("u"), "TestColumn")
                .OrderBy(new Alias("u"), "TestColumn2");

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT u.TestColumn FROM TestTable u ORDER BY u.TestColumn, u.TestColumn2"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void OrderByAppendsDescKeywordWhenDescArgumentIsTrue()
        {
            var query = new SqlQuery()
                .Select("u.TestColumn")
                .From("TestTable u")
                .OrderBy(new Alias("u"), "TestColumn", desc: true)
                .OrderBy("TestColumn2", desc: true);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT u.TestColumn FROM TestTable u ORDER BY u.TestColumn DESC, TestColumn2 DESC"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void OrderByFirstWithEmptyOrNullArgumentsThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderByFirst((string)null, desc: false));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderByFirst((string)null, desc: true));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderByFirst("", desc: false));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderByFirst("", desc: true));
        }

        [Fact]
        public void OrderByFirstInsertsExpressionToStart()
        {
            var query = new SqlQuery()
                .From("TestTable")
                .Select("a")
                .OrderBy("a")
                .OrderBy("b")
                .OrderByFirst("c");

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT a FROM TestTable ORDER BY c, a, b"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void OrderByFirstMovesExpressionToStartIfAlreadyInStatement()
        {
            var query = new SqlQuery()
                .From("TestTable")
                .Select("a")
                .OrderBy("a")
                .OrderBy("b")
                .OrderBy("c")
                .OrderByFirst("b");

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT a FROM TestTable ORDER BY b, a, c"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void OrderByFirstHandlesDescWhileMovingExpressionToFirst()
        {
            var query1 = new SqlQuery()
                .From("TestTable")
                .Select("a")
                .OrderBy("a")
                .OrderBy("b", desc: true)
                .OrderBy("c")
                .OrderByFirst("b");

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT a FROM TestTable ORDER BY b, a, c"),
                TestSqlHelper.Normalize(
                    query1.ToString()));

            var query2 = new SqlQuery()
                .From("TestTable")
                .Select("a")
                .OrderBy("a")
                .OrderBy("b")
                .OrderBy("c")
                .OrderByFirst("b", desc: true);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT a FROM TestTable ORDER BY b DESC, a, c"),
                TestSqlHelper.Normalize(
                    query2.ToString()));
        }

        [Fact]
        public void OrderByFirstAppendsDescKeywordWhenDescArgumentIsTrue()
        {
            var query = new SqlQuery()
                .Select("u.TestColumn")
                .From("TestTable u")
                .OrderBy(new Alias("u"), "TestColumn", desc: true)
                .OrderByFirst("TestColumn2", desc: true);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT u.TestColumn FROM TestTable u ORDER BY TestColumn2 DESC, u.TestColumn DESC"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void OrderByFirstWorksProperlyWhenNoOrderByExists()
        {
            var query = new SqlQuery()
                .Select("TestColumn")
                .From("TestTable")
                .OrderByFirst("TestColumn")
                .OrderBy("SecondColumn");

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT TestColumn FROM TestTable ORDER BY TestColumn, SecondColumn"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void SkipThrowsExceptionIfNoOrderByForSql2000Dialect()
        {
            var query = new SqlQuery()
                .Dialect(SqlServer2000Dialect.Instance)
                .Select("c")
                .From("t")
                .Skip(10);

            Assert.Throws<InvalidOperationException>(delegate
            {
                query.ToString();
            });
        }

        [Fact]
        public void SkipUsesWorkAroundWithOneOrderByForSql2000Dialect()
        {
            var query = new SqlQuery()
                .Dialect(SqlServer2000Dialect.Instance)
                .Select("c")
                .From("t")
                .OrderBy("x")
                .Skip(10);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "DECLARE @Value0 SQL_VARIANT;" +
                    "SELECT TOP 10 @Value0 = x FROM t ORDER BY x;" +
                    "SELECT c FROM t WHERE ((((x IS NOT NULL AND @Value0 IS NULL) OR (x > @Value0)))) ORDER BY x"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void SkipUsesWorkAroundWithTwoOrderByForSql2000Dialect()
        {
            var query = new SqlQuery()
                .Dialect(SqlServer2000Dialect.Instance)
                .Select("c")
                .From("t")
                .Where("c > 2")
                .OrderBy("x")
                .OrderBy("y")
                
                .Skip(100)
                .Take(50);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "DECLARE @Value0 SQL_VARIANT;" + 
                    "DECLARE @Value1 SQL_VARIANT;" +
                    "SELECT TOP 100 @Value0 = x,@Value1 = y FROM t WHERE c > 2 ORDER BY x, y;" +
                    "SELECT TOP 50 c FROM t WHERE c > 2 AND " + 
                        "((((x IS NOT NULL AND @Value0 IS NULL) OR (x > @Value0))) " + 
                            "OR (((x IS NULL AND @Value0 IS NULL) OR (x = @Value0)) " + 
                            "AND ((y IS NOT NULL AND @Value1 IS NULL) OR (y > @Value1)))) ORDER BY x, y"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void SkipUsesRowNumberForSql2005Dialect()
        {
            var query = new SqlQuery()
                .Dialect(SqlServer2005Dialect.Instance)
                .Select("c")
                .From("t")
                .OrderBy("x")
                .Skip(10)
                .Take(20);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT * FROM (\n" +
                        "SELECT TOP 30 c, ROW_NUMBER() OVER (ORDER BY x) AS __num__ FROM t) __results__ " + 
                    "WHERE __num__ > 10"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void HavingDoesAndWhenCalledMoreThanOnce()
        {
            var query = new SqlQuery()
                .From("t")
                .GroupBy("c")
                .Select("c")
                .Having("count(*) > 2")
                .Having("sum(y) < 1000");

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT c FROM t GROUP BY c HAVING count(*) > 2 AND sum(y) < 1000"),
                TestSqlHelper.Normalize(
                    query.ToString())
            );
        }



        [Fact]
        public void IntoRowIndexCanBeSetToMinusOneWithNull()
        {
            var entity = new MyEntity() { Table = "x" };
            var query = new SqlQuery().From(entity).Select("x1");
            Assert.Equal(((ISqlQueryExtensible)query).Columns.ElementAt(0).IntoRowIndex, 0);
            query.Into(null).Select("x2");
            Assert.Equal(((ISqlQueryExtensible)query).Columns.ElementAt(1).IntoRowIndex, -1);
            Assert.Equal(1, ((ISqlQueryExtensible)query).IntoRows.Count);
            Assert.Equal(entity, ((ISqlQueryExtensible)query).IntoRows[0]);
        }

        [Fact]
        public void IntoRowCanBeChanged()
        {
            var entity1 = new MyEntity() { Table = "x" };
            var query = new SqlQuery().From(entity1).Select("x1");
            Assert.Equal(((ISqlQueryExtensible)query).Columns.ElementAt(0).IntoRowIndex, 0);
            var entity2 = new MyEntity() { Table = "y" };
            query.Into(entity2).Select("y1");
            Assert.Equal(1, ((ISqlQueryExtensible)query).Columns.ElementAt(1).IntoRowIndex);
            Assert.Equal(2, ((ISqlQueryExtensible)query).IntoRows.Count);
            Assert.Equal(entity1, ((ISqlQueryExtensible)query).IntoRows[0]);
            Assert.Equal(entity2, ((ISqlQueryExtensible)query).IntoRows[1]);
        }

        [Fact]
        public void SelectWithEmptyOrNullArgumentsThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((string)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(String.Empty));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((string)null, "x"));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(String.Empty, "y"));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select("x", (string)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select("y", String.Empty));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((Alias)null, "x"));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new Alias("a"), (string)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new Alias("a"), String.Empty));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((Alias)null, "x", "y"));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new Alias("a"), (string)null, "x"));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new Alias("b"), String.Empty, "y"));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new Alias("c"), "x", (string)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new Alias("c"), "x", String.Empty));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((ISqlQuery)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((ISqlQuery)null, "x"));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new SqlQuery(), (string)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new SqlQuery(), String.Empty));
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
        public void WhereDoesAndWhenCalledMoreThanOnce()
        {
            var query = new SqlQuery().From("t").Select("c").Where("x > 5").Where("y < 4");
            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT c FROM t WHERE x > 5 AND y < 4"),
                TestSqlHelper.Normalize(
                    query.ToString())
            );
        }

        [Fact]
        public void WhereWithEmptyOrNullArgumentsThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Where((string)null));
            Assert.Throws<ArgumentNullException>(() => new SqlQuery().Where(String.Empty));
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
                        "((SELECT TOP 1 SubColumn FROM SubTable) >= @p1)"),
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
                        "((SELECT TOP 1 SubColumn FROM SubTable) >= @p1)"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void TakeUsesCorrectSyntaxForSqliteDialect()
        {
            var query = new SqlQuery()
                .Dialect(SqliteDialect.Instance)
                .Select("c")
                .From("t")
                .Take(10);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT c FROM t LIMIT 10"),
                TestSqlHelper.Normalize(
                    query.ToString()));

        }

        [Fact]
        public void TakeUsesCorrectSyntaxForOracleDialect()
        {
            var query = new SqlQuery()
                .Dialect(OracleDialect.Instance)
                .Select("c")
                .From("t")
                .Take(10);

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT * FROM ( SELECT c FROM t) WHERE ROWNUM > 0 AND ROWNUM <= 10"),
                TestSqlHelper.Normalize(
                    query.ToString()));
        }

        [Fact]
        public void WithoutTakeUsesCorrectSyntaxForOracleDialect()
        {
            var query = new SqlQuery()
                .Dialect(OracleDialect.Instance)
                .Select("c")
                .From("t");

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT c FROM t"),
                TestSqlHelper.Normalize(
                    query.ToString()));

        }

        [Fact]
        public void JoinIgnoresExistingJoinsWithSameAliasAndSameExpression()
        {
            var row = new Serenity.Test.Data.RowMappingTests.ComplexRow();
            var fld = Serenity.Test.Data.RowMappingTests.ComplexRow.Fields;
            var query = new SqlQuery()
                .From(row)
                .Select(fld.CountryName)
                .LeftJoin(new Alias("TheCountryTable", "c"), new Criteria("c", "TheCountryID") == new Criteria(0, "CountryID"));

            Assert.Equal(
                TestSqlHelper.Normalize(
                    "SELECT c.Name AS [CountryName] FROM ComplexTable T0 LEFT JOIN TheCountryTable c ON (c.TheCountryID = T0.CountryID)"),
                TestSqlHelper.Normalize(
                    query.ToString()));

        }

        [Fact]
        public void JoinThrowsExceptionForJoinsWithSameAliasButDifferentExpression()
        {
            var row = new Serenity.Test.Data.RowMappingTests.ComplexRow();
            var fld = Serenity.Test.Data.RowMappingTests.ComplexRow.Fields;

            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                var query = new SqlQuery()
                    .From(row)
                    .Select(fld.CountryName)
                    .LeftJoin(new Alias("City", "c"), new Criteria("c", "CityID") == new Criteria(0, "CountryID"));
            });

            Assert.Contains("already has a join 'c'", exception.Message);
            Assert.Contains("LEFT JOIN TheCountryTable c ON (c.TheCountryID = T0.CountryID)", exception.Message);
            Assert.Contains("Attempted join expression is 'LEFT JOIN City c ON (c.CityID = T0.CountryID)", exception.Message);
        }
    }
}