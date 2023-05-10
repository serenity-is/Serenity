namespace Serenity.Tests.Entity;

public class RowDialectExpressionTests
{
    #region Rows used in tests
    [LeftJoin("a", "x", "y")]
    [LeftJoin("a", "z", "w")]
    public class DuplicateLeftJoinNoDialectRow : Row<DuplicateLeftJoinNoDialectRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
        }
    }

    [LeftJoin("a", "x", "y", ServerType.MySql)]
    [LeftJoin("a", "z", "w", ServerType.SqlServer)]
    [LeftJoin("b", "y", "z", ServerType.MySql, NegateDialect = true)]
    [LeftJoin("b", "w", "y", ServerType.SqlServer, NegateDialect = true)]
    [InnerJoin("inner", "a", "b", ServerType.MySql)]
    [InnerJoin("inner", "b", "b", ServerType.SqlServer)]
    [InnerJoin("inner negate", "y", "z", ServerType.MySql, NegateDialect = true)]
    [InnerJoin("inner negate", "w", "y", ServerType.SqlServer, NegateDialect = true)]
    [OuterApply("outer", "y", ServerType.MySql)]
    [OuterApply("outer", "w", ServerType.SqlServer)]
    [OuterApply("outer negate", "y", ServerType.MySql, NegateDialect = true)]
    [OuterApply("outer negate", "w", ServerType.SqlServer, NegateDialect = true)]
    public class DuplicateLeftJoinDifferentDialectWithForeignKeyLeftJoinRow : Row<DuplicateLeftJoinDifferentDialectWithForeignKeyLeftJoinRow.RowFields>
    {
        [ForeignKey("a", "x"), LeftJoin("y", ServerType.Sqlite), LeftJoin("z", ServerType.SqlServer)]
        [LeftJoin("q", "w", "x", ServerType.MySql, NegateDialect = true), LeftJoin("q", "z", "y", ServerType.SqlServer, NegateDialect = true)]
        [InnerJoin("e", "w", "x", ServerType.MySql), InnerJoin("e", "z", "y", ServerType.SqlServer)]
        [InnerJoin("w", "w", "x", ServerType.MySql, NegateDialect = true), InnerJoin("w", "z", "y", ServerType.SqlServer, NegateDialect = true)]
        public int? Test
        {
            get => fields.Test[this];
            set => fields.Test[this] = value;
        }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Test;
        }
    }


    #endregion

    [Fact]
    public void Raises_Exception_If_Join_Is_Ambiguous()
    {
        var ex = Assert.Throws<AmbiguousMatchException>(() =>
        {
            new DuplicateLeftJoinNoDialectRow.RowFields().Initialize(null, SqlServer2012Dialect.Instance);
        });

        Assert.Contains(nameof(DuplicateLeftJoinNoDialectRow), ex.Message);
        Assert.Contains("'a'", ex.Message);
    }

    [Fact]
    public void Uses_One_Of_LeftJoins_If_Dialect_For_Same_Alias_Joins_Are_Different()
    {
        var fields = new DuplicateLeftJoinDifferentDialectWithForeignKeyLeftJoinRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        var join = Assert.Contains("a", fields.Joins);
        Assert.Equal("z", join.Table);
        Assert.Equal("w", join.OnCriteria?.ToString());
    }

    [Fact]
    public void Should_Use_Sqlite_Dialect_On_ForeignKey_LeftJoin()
    {
        var fields = new DuplicateLeftJoinDifferentDialectWithForeignKeyLeftJoinRow.RowFields();
        fields.Initialize(null, SqliteDialect.Instance);

        var join = Assert.Contains("y", fields.Joins);
        Assert.Equal("a", join.Table);
        Assert.Equal("(y.x = T0.Test)", join.OnCriteria?.ToString());
    }

    [Fact]
    public void Should_Use_SqlServer_Dialect_On_ForeignKey_LeftJoin()
    {
        var fields = new DuplicateLeftJoinDifferentDialectWithForeignKeyLeftJoinRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        var join = Assert.Contains("z", fields.Joins);
        Assert.Equal("a", join.Table);
        Assert.Equal("(z.x = T0.Test)", join.OnCriteria?.ToString());
    }

    [Fact]
    public void Uses_Matching_Dialect_LeftJoin_On_Property()
    {
        var fields = new DuplicateLeftJoinDifferentDialectWithForeignKeyLeftJoinRow.RowFields();
        fields.Initialize(null, SqliteDialect.Instance);

        var join = Assert.Contains("y", fields.Joins);
        Assert.Equal("a", join.Table);
        Assert.Equal("(y.x = T0.Test)", join.OnCriteria?.ToString());
    }

    [Fact]
    public void Can_Negate_Matching_Dialect_On_SqlServer_LeftJoin()
    {
        var fields = new DuplicateLeftJoinDifferentDialectWithForeignKeyLeftJoinRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        var join = Assert.Contains("b", fields.Joins);
        Assert.Equal("y", join.Table);
        Assert.Equal("z", join.OnCriteria?.ToString());
    }

    [Fact]
    public void Can_Negate_Matching_Dialect_On_MySql_LeftJoin()
    {
        var fields = new DuplicateLeftJoinDifferentDialectWithForeignKeyLeftJoinRow.RowFields();
        fields.Initialize(null, MySqlDialect.Instance);

        var join = Assert.Contains("b", fields.Joins);
        Assert.Equal("w", join.Table);
        Assert.Equal("y", join.OnCriteria?.ToString());
    }

    [Fact]
    public void Can_Negate_Matching_Dialect_On_SqlServer_LeftJoin_On_Property()
    {
        var fields = new DuplicateLeftJoinDifferentDialectWithForeignKeyLeftJoinRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        var join = Assert.Contains("q", fields.Joins);
        Assert.Equal("w", join.Table);
        Assert.Equal("(q.x = T0.Test)", join.OnCriteria?.ToString());
    }

    [Fact]
    public void Can_Negate_Matching_Dialect_On_MySql_LeftJoin_On_Property()
    {
        var fields = new DuplicateLeftJoinDifferentDialectWithForeignKeyLeftJoinRow.RowFields();
        fields.Initialize(null, MySqlDialect.Instance);

        var join = Assert.Contains("q", fields.Joins);
        Assert.Equal("z", join.Table);
        Assert.Equal("(q.y = T0.Test)", join.OnCriteria?.ToString());
    }

    [Fact]
    public void Uses_One_Of_InnerJoins_If_Dialect_For_Same_Alias_Joins_Are_Different()
    {
        var fields = new DuplicateLeftJoinDifferentDialectWithForeignKeyLeftJoinRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        var join = Assert.Contains("inner", fields.Joins);
        Assert.Equal("b", join.Table);
        Assert.Equal("b", join.OnCriteria?.ToString());
    }

    [Fact]
    public void Uses_One_Of_OuterApplys_If_Dialect_For_Same_Alias_Joins_Are_Different()
    {
        var fields = new DuplicateLeftJoinDifferentDialectWithForeignKeyLeftJoinRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        var join = Assert.Contains("outer", fields.Joins);
        Assert.Equal("(w)", join.Table);
    }

    [Fact]
    public void Can_Negate_One_Of_InnerJoins_If_Dialect_For_Same_Alias_Joins_Are_Different()
    {
        var fields = new DuplicateLeftJoinDifferentDialectWithForeignKeyLeftJoinRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        var join = Assert.Contains("inner negate", fields.Joins);
        Assert.Equal("y", join.Table);
        Assert.Equal("z", join.OnCriteria?.ToString());
    }

    [Fact]
    public void Can_Negate_One_Of_OuterApplys_If_Dialect_For_Same_Alias_Joins_Are_Different()
    {
        var fields = new DuplicateLeftJoinDifferentDialectWithForeignKeyLeftJoinRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        var join = Assert.Contains("outer negate", fields.Joins);
        Assert.Equal("(y)", join.Table);
    }


    [Fact]
    public void Uses_One_Of_InnerJoins_If_Dialect_For_Same_Alias_Joins_Are_Different_On_Property()
    {
        var fields = new DuplicateLeftJoinDifferentDialectWithForeignKeyLeftJoinRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        var join = Assert.Contains("e", fields.Joins);
        Assert.Equal("z", join.Table);
        Assert.Equal("(e.y = T0.Test)", join.OnCriteria?.ToString());
    }

    [Fact]
    public void Can_Negate_One_Of_InnerJoins_If_Dialect_For_Same_Alias_Joins_Are_Different_On_Property()
    {
        var fields = new DuplicateLeftJoinDifferentDialectWithForeignKeyLeftJoinRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        var join = Assert.Contains("w", fields.Joins);
        Assert.Equal("w", join.Table);
        Assert.Equal("(w.x = T0.Test)", join.OnCriteria?.ToString());
    }
}
