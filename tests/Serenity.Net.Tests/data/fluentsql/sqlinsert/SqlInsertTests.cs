namespace Serenity.Data;

public class SqlInsertTests
{
    [Fact]
    public void Constructor_Throws_For_Null_Or_Empty_Table()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlInsert(null!));
        Assert.Throws<ArgumentNullException>(() => new SqlInsert(""));
    }

    [Fact]
    public void IdentityColumn_Get_Set_And_TableName()
    {
        var insert = new SqlInsert("T");

        Assert.Null(insert.IdentityColumn());
        Assert.Same(insert, insert.IdentityColumn("ID"));
        Assert.Equal("ID", insert.IdentityColumn());
        Assert.Equal("T", insert.TableName());
    }

    [Fact]
    public void SetTo_Validates_And_Adds()
    {
        var insert = new SqlInsert("T");

        Assert.Throws<ArgumentNullException>(() => insert.SetTo((string)null!, "1"));
        Assert.Throws<ArgumentException>(() => insert.SetTo("", "1"));
        Assert.Throws<ArgumentNullException>(() => insert.SetTo("A", null!));
        Assert.Throws<ArgumentException>(() => insert.SetTo("A", ""));

        Assert.Same(insert, insert.SetTo("A", "1"));
        Assert.Single(insert.GetFieldExpressions());
    }

    [Fact]
    public void Explicit_SetTo_Validates_And_Adds()
    {
        ISetFieldByStatement insert = new SqlInsert("T");

        Assert.Throws<ArgumentNullException>(() => insert.SetTo(null!, "1"));
        Assert.Throws<ArgumentException>(() => insert.SetTo("", "1"));
        Assert.Throws<ArgumentNullException>(() => insert.SetTo("A", null!));
        Assert.Throws<ArgumentException>(() => insert.SetTo("A", ""));

        insert.SetTo("A", "1");
        Assert.Single(((SqlInsert)insert).GetFieldExpressions());
    }

    [Fact]
    public void SetTo_With_Field_Validates_And_Adds()
    {
        var insert = new SqlInsert("T");

        Assert.Throws<ArgumentNullException>(() => insert.SetTo((IField)null!, "1"));

        insert.SetTo(IdNameRow.Fields.ID, "1");
        var pair = Assert.Single(insert.GetFieldExpressions());
        Assert.Equal("ID", pair.Field);
        Assert.Equal("1", pair.Expression);
    }

    [Fact]
    public void SetNull_Validates_And_Adds()
    {
        var insert = new SqlInsert("T");

        Assert.Throws<ArgumentNullException>(() => insert.SetNull(null!));
        Assert.Throws<ArgumentException>(() => insert.SetNull(""));

        Assert.Same(insert, insert.SetNull("A"));
        var pair = Assert.Single(insert.GetFieldExpressions());
        Assert.Equal("A", pair.Field);
    }

    [Fact]
    public void Clone_Copies_Field_Expressions()
    {
        var insert = new SqlInsert("T").SetTo("A", "1").SetTo("B", "2");
        var clone = insert.Clone();

        Assert.Equal(insert.ToString(), clone.ToString());
        Assert.Equal(2, clone.GetFieldExpressions().Count);
    }

    [Fact]
    public void Clone_Copies_AutoParameterCounter()
    {
        var insert = new SqlInsert("T");
        var first = insert.AddParam(1);
        insert.SetTo("A", first.Name);

        var clone = insert.Clone();
        var second = clone.AddParam(2);

        Assert.Equal("@p2", second.Name);
        Assert.Equal(2, clone.ParamCount);
        Assert.Equal(1, insert.ParamCount);
    }

    [Fact]
    public void Clone_Copies_Dialect_For_RegeneratedQuery()
    {
        var insert = new SqlInsert("T")
            .Dialect(PostgresDialect.Instance)
            .SetTo("A", "1");
        _ = insert.ToString();

        var clone = insert.Clone().SetTo("B", "2");

        Assert.Same(PostgresDialect.Instance, clone.Dialect());
        Assert.True(clone.IsDialectOverridden);
        Assert.Equal(SqlInsert.Format("T", clone.GetFieldExpressions(), PostgresDialect.Instance),
            clone.ToString());
    }

    [Fact]
    public void ToString_Uses_Cache()
    {
        var insert = new SqlInsert("T").SetTo("A", "1");
        var first = insert.ToString();
        var second = insert.ToString();

        Assert.Same(first, second);
        Assert.Throws<NotSupportedException>(() =>
            ((IList<FieldExpressionPair>)insert.GetFieldExpressions()).Add(new("B", "2")));
    }

    [Fact]
    public void Dialect_Change_Invalidates_ToString_Cache()
    {
        var insert = new SqlInsert("T").SetTo("A", "1");
        var beforeDialectChange = insert.ToString();

        insert.Dialect(PostgresDialect.Instance);
        var afterDialectChange = insert.ToString();

        Assert.NotSame(beforeDialectChange, afterDialectChange);
        Assert.Equal(SqlInsert.Format("T", insert.GetFieldExpressions(), PostgresDialect.Instance),
            afterDialectChange);
    }

    [Fact]
    public void Dialect_Throws_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlInsert("T").Dialect(null!));
    }

    [Fact]
    public void Format_Validates_And_Formats()
    {
        Assert.Equal("INSERT INTO [T] ([A]) VALUES (1)", SqlInsert.Format("T", new FieldExpressionPair[] { new("A", "1") }));
        Assert.Throws<ArgumentNullException>(() => SqlInsert.Format(null!, new FieldExpressionPair[] { new("A", "1") }));
        Assert.Throws<ArgumentNullException>(() => SqlInsert.Format("T", (IEnumerable<FieldExpressionPair>)null!));
    }

#pragma warning disable CS0618
    [Fact]
    public void Obsolete_Format_Validates_And_Formats()
    {
        List<string>? nullList = null;

        Assert.Equal("INSERT INTO [T] ([A]) VALUES (1)",
            SqlInsert.Format("T", ["A", "1"]));
        Assert.Throws<ArgumentNullException>(() => SqlInsert.Format("T", nullList!));
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlInsert.Format("T", ["A"]));
    }
#pragma warning restore CS0618

    [Fact]
    public void FormatUpsert_Throws_For_Empty_KeyFields()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            SqlInsert.FormatUpsert("T", [new("A", "1")], [], SqliteDialect.Instance));
    }

    [Fact]
    public void FormatUpsert_Throws_For_Empty_Field_Name()
    {
        Assert.Throws<ArgumentException>(() =>
            SqlInsert.FormatUpsert("T", [new("A", "1")], [""], SqliteDialect.Instance));
    }

    [Fact]
    public void Clone_Copies_Alias_Expressions()
    {
        var insert = new AliasSqlInsert("T");
        insert.SetAlias("j1", "Table J1");

        var clone = insert.Clone();

        Assert.True(clone.HasAlias("j1"));

        insert.SetAlias("j2", "Table J2");

        Assert.False(clone.HasAlias("j2"));
    }

    private sealed class AliasSqlInsert(string table) : SqlInsert(table)
    {
        public void SetAlias(string alias, string expression) => SetAliasExpression(alias, expression);
    }
}
