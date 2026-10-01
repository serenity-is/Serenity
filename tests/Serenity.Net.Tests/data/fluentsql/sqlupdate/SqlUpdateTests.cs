namespace Serenity.Data;

public class SqlUpdateTests
{
    [Fact]
    public void Constructor_Throws_For_Empty_TableName()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlUpdate(null!));
        Assert.Throws<ArgumentNullException>(() => new SqlUpdate(""));
    }

    [Fact]
    public void SetTo_And_Accessors_Work()
    {
        var update = new SqlUpdate("T")
            .SetTo("Name", "@p1")
            .SetTo("Age", "@p2");

        Assert.Equal("T", update.TableName());
        Assert.Equal(2, update.GetFieldExpressions().Count);
        Assert.Equal("Name", update.GetFieldExpressions()[0].Field);
        Assert.Equal("@p1", update.GetFieldExpressions()[0].Expression);
        Assert.Empty(update.GetWhereConditions());
        Assert.Equal("", ((IFilterableQuery)update).GetWhereClause());
    }

    [Fact]
    public void SetTo_Throws_For_Null_Field_Or_Expression()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlUpdate("T").SetTo((string)null!, "@p"));
        Assert.Throws<ArgumentException>(() => new SqlUpdate("T").SetTo("", "@p"));
        Assert.Throws<ArgumentNullException>(() => new SqlUpdate("T").SetTo("F", null!));
        Assert.Throws<ArgumentException>(() => new SqlUpdate("T").SetTo("F", ""));
    }

    [Fact]
    public void Explicit_SetTo_Works_And_Validates()
    {
        ISetFieldByStatement update = new SqlUpdate("T");
        update.SetTo("Name", "@p1");

        Assert.Single(((SqlUpdate)update).GetFieldExpressions());

        Assert.Throws<ArgumentNullException>(() => update.SetTo(null!, "@p"));
        Assert.Throws<ArgumentException>(() => update.SetTo("", "@p"));
        Assert.Throws<ArgumentNullException>(() => update.SetTo("F", null!));
        Assert.Throws<ArgumentException>(() => update.SetTo("F", ""));
    }

    [Fact]
    public void SetTo_Field_Overload_Works()
    {
        var update = new SqlUpdate("T");
        var result = update.SetTo(IdNameRow.Fields.Name, "@p1");

        Assert.Same(update, result);
        Assert.Equal(IdNameRow.Fields.Name.Name, update.GetFieldExpressions()[0].Field);
        Assert.Throws<ArgumentNullException>(() => new SqlUpdate("T").SetTo((IField)null!, "@p"));
    }

    [Fact]
    public void SetNull_Works_And_Validates()
    {
        var update = new SqlUpdate("T").SetNull("Name");

        Assert.Equal(SqlKeywords.Null, update.GetFieldExpressions()[0].Expression);
        Assert.Throws<ArgumentNullException>(() => new SqlUpdate("T").SetNull(null!));
        Assert.Throws<ArgumentException>(() => new SqlUpdate("T").SetNull(""));
    }

    [Fact]
    public void Inc_And_Dec_Work()
    {
        var update = new SqlUpdate("T").Inc("Counter", 5).Inc("Other", -3);

        Assert.Equal("Counter + 5", update.GetFieldExpressions()[0].Expression);
        Assert.Equal("Other-3", update.GetFieldExpressions()[1].Expression);

        update = new SqlUpdate("T").Dec("Counter", 5);
        Assert.Equal("Counter-5", update.GetFieldExpressions()[0].Expression);
    }

    [Fact]
    public void Inc_Brackets_Keyword_Field_In_Expression()
    {
        var update = new SqlUpdate("T").Inc("Order", 1);

        Assert.Equal("Order + 1", update.GetFieldExpressions()[0].Expression);
        Assert.Equal("UPDATE [T] SET [Order] = [Order] + 1", update.ToString());
    }

    [Fact]
    public void Dec_Brackets_Keyword_Field_In_Expression()
    {
        var update = new SqlUpdate("T").Dec("Order", 1);

        Assert.Equal("Order-1", update.GetFieldExpressions()[0].Expression);
        Assert.Equal("UPDATE [T] SET [Order] = [Order]-1", update.ToString());
    }

    [Fact]
    public void Inc_Uses_Dialect_Set_After_Inc_Call_When_Rendering_Expression()
    {
        var update = new SqlUpdate("T").Inc("Order", 1)
            .Dialect(new MockSqlDialect(autoQuotedIdentifiers: false));

        Assert.Equal("UPDATE T SET [Order] = [Order] + 1", update.ToString());
    }

    [Fact]
    public void Inc_Does_Not_Bracket_NonKeyword_Field_When_AutoQuoting_Is_Disabled()
    {
        var update = new SqlUpdate("T").Inc("Counter", 1)
            .Dialect(new MockSqlDialect(autoQuotedIdentifiers: false));

        Assert.Equal("UPDATE T SET Counter = Counter + 1", update.ToString());
    }

    [Fact]
    public void Dec_Does_Not_Bracket_NonKeyword_Field_When_AutoQuoting_Is_Disabled()
    {
        var update = new SqlUpdate("T").Dec("Counter", 1)
            .Dialect(new MockSqlDialect(autoQuotedIdentifiers: false));

        Assert.Equal("UPDATE T SET Counter = Counter-1", update.ToString());
    }

    [Fact]
    public void Inc_And_Dec_Field_Overloads_Work()
    {
        var update = new SqlUpdate("T").Inc(IdNameRow.Fields.Name, 2);
        Assert.Equal(IdNameRow.Fields.Name.Name + " + 2", update.GetFieldExpressions()[0].Expression);

        update = new SqlUpdate("T").Dec(IdNameRow.Fields.Name, 2);
        Assert.Equal(IdNameRow.Fields.Name.Name + "-2", update.GetFieldExpressions()[0].Expression);

        Assert.Throws<ArgumentNullException>(() => new SqlUpdate("T").Inc((IField)null!, 1));
        Assert.Throws<ArgumentNullException>(() => new SqlUpdate("T").Dec((IField)null!, 1));
    }

    [Fact]
    public void Where_Works_And_Validates()
    {
        var update = new SqlUpdate("T").Where("A = 1").Where("B = 2");

        Assert.Equal(2, update.GetWhereConditions().Count);
        Assert.Equal("A = 1 AND B = 2", ((IFilterableQuery)update).GetWhereClause());
        Assert.Equal("A = 1 AND B = 2", ((IFilterableQuery)update).GetWhereClause());
        Assert.Throws<ArgumentNullException>(() => new SqlUpdate("T").Where((string)null!));
        Assert.Throws<ArgumentNullException>(() => new SqlUpdate("T").Where(""));
    }

    [Fact]
    public void Where_Renders_Parameters_Once_And_Clone_Preserves_Counter()
    {
        var update = new SqlUpdate("T").Where(new Criteria("A") == 5);
        Assert.Equal(1, update.ParamCount);

        var where = ((IFilterableQuery)update).GetWhereClause();

        Assert.Equal(1, update.ParamCount);
        Assert.Equal(where, ((IFilterableQuery)update).GetWhereClause());
        Assert.Equal(1, update.ParamCount);

        var clone = update.Clone().Where(new Criteria("B") == 6);
        Assert.Equal(2, clone.ParamCount);
        Assert.Equal(1, update.ParamCount);

        var cloneWhere = ((IFilterableQuery)clone).GetWhereClause();
        Assert.Equal(cloneWhere, ((IFilterableQuery)clone).GetWhereClause());
        Assert.Equal(2, clone.ParamCount);
    }

    [Fact]
    public void Explicit_Where_Works()
    {
        var update = new SqlUpdate("T");
        var criteria = new Criteria("A = 1");
        ((IFilterableQuery)update).Where(criteria);
        Assert.Same(criteria, Assert.Single(((IFilterableQuery)update).GetWhereCriteria()));
        Assert.Single(update.GetWhereConditions());
    }

    [Fact]
    public void RemoveT0Reference_Handles_All_Cases()
    {
        Assert.Equal("ID", SqlUpdate.RemoveT0Reference("T0.ID"));
        Assert.Equal("[ID]", SqlUpdate.RemoveT0Reference("T0.[ID]"));
        Assert.Equal("NoRef", SqlUpdate.RemoveT0Reference("NoRef"));
        Assert.Equal("x + ID", SqlUpdate.RemoveT0Reference("x + T0.ID"));
        Assert.NotNull(SqlUpdate.RemoveT0Reference("T0.Category.Name"));
    }

    [Fact]
    public void Where_Removes_T0_References()
    {
        var update = new SqlUpdate("T").Where("T0.ID = 5");
        Assert.Equal("ID = 5", update.GetWhereConditions()[0]);
        Assert.Equal("ID = 5", ((IFilterableQuery)update).GetWhereClause());
    }

    [Fact]
    public void Where_Removes_BracketQuoted_T0_References()
    {
        var update = new SqlUpdate("T").Where("[T0].[ID] = 5");
        Assert.Equal("[ID] = 5", ((IFilterableQuery)update).GetWhereClause());
    }

    [Fact]
    public void Dialect_Sets_Dialect_And_Validates()
    {
        var update = new SqlUpdate("T").Dialect(new SqlServer2012Dialect());
        Assert.NotNull(update.ToString());
        Assert.Throws<ArgumentNullException>(() => new SqlUpdate("T").Dialect(null!));
    }

    [Fact]
    public void Clone_Copies_State()
    {
        var update = new SqlUpdate("T")
            .SetTo("A", "@a")
            .Where("B = 1");

        var clone = update.Clone();

        Assert.Equal("T", clone.TableName());
        Assert.Equal(update.GetFieldExpressions().Count, clone.GetFieldExpressions().Count);
        Assert.Equal(update.GetWhereConditions(), clone.GetWhereConditions());
    }

    [Fact]
    public void Clone_Copies_Dialect_For_RegeneratedQuery()
    {
        var update = new SqlUpdate("T")
            .Dialect(PostgresDialect.Instance)
            .SetTo("A", "1");
        var clone = update.Clone().SetTo("B", "2");

        Assert.Same(PostgresDialect.Instance, clone.Dialect());
        Assert.True(clone.IsDialectOverridden);
        Assert.Equal(SqlUpdate.Format("T", string.Empty, clone.GetFieldExpressions(), PostgresDialect.Instance),
            clone.ToString());
    }

    [Fact]
    public void ToString_Formats_Update()
    {
        var update = new SqlUpdate("T").SetTo("A", "@a").Where("B = 1");
        var sql = update.ToString();

        Assert.Contains("UPDATE", sql);
        Assert.Contains("[T]", sql);
        Assert.Contains("[A] = @a", sql);
        Assert.Contains("WHERE B = 1", sql);
    }

    [Fact]
    public void ToString_Caches_And_Invalidates_For_Mutations()
    {
        (string Name, Action<SqlUpdate> Mutate)[] mutations =
        [
            ("SetTo", update => update.SetTo("B", "2")),
            ("SetTo interface", update => ((ISetFieldByStatement)update).SetTo("B", "2")),
            ("SetNull", update => update.SetNull("B")),
            ("Where", update => update.Where(new Criteria("B") == 2)),
            ("Dialect", update => update.Dialect(PostgresDialect.Instance))
        ];

        foreach (var (name, mutate) in mutations)
        {
            var update = new SqlUpdate("T").SetTo("A", "1");
            var first = update.ToString();

            Assert.Same(first, update.ToString());

            mutate(update);
            var updated = update.ToString();

            Assert.NotSame(first, updated);
            Assert.Same(updated, update.ToString());
            Assert.True(updated.Length > 0, $"{name} produced an empty query.");
        }

        var updateForView = new SqlUpdate("T").SetTo("A", "1");
        Assert.Throws<NotSupportedException>(() =>
            ((IList<FieldExpressionPair>)updateForView.GetFieldExpressions()).Add(new("B", "2")));
    }

    [Fact]
    public void Format_Formats_Update()
    {
        var pairs = new List<FieldExpressionPair>
        {
            new("A", "@a"),
            new("B", "@b")
        };

        var sql = SqlUpdate.Format("T", "C = 1", pairs);
        Assert.Equal("UPDATE [T] SET [A] = @a, [B] = @b WHERE C = 1", sql);

        Assert.Equal("UPDATE [T] SET [A] = @a", SqlUpdate.Format("T", "", new List<FieldExpressionPair>
        {
            new("A", "@a")
        }));

        Assert.Equal("UPDATE [T] SET [A] = @a", SqlUpdate.Format("T", null, new List<FieldExpressionPair>
        {
            new("A", "@a")
        }));

        Assert.Throws<ArgumentNullException>(() => SqlUpdate.Format(null!, "x", pairs));
        Assert.Throws<ArgumentException>(() => SqlUpdate.Format("", "x", pairs));
        Assert.Throws<ArgumentNullException>(() => SqlUpdate.Format("T", "x", (IEnumerable<FieldExpressionPair>)null!));
    }

#pragma warning disable CS0618
    [Fact]
    public void Obsolete_Format_Works_And_Validates()
    {
        var sql = SqlUpdate.Format("T", "C = 1", ["A", "@a", "B", "@b"]);
        Assert.Equal("UPDATE [T] SET [A] = @a, [B] = @b WHERE C = 1", sql);

        Assert.Throws<ArgumentNullException>(() => SqlUpdate.Format("T", "x", (List<string>)null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlUpdate.Format("T", "x", ["A", "@a", "B"]));
    }
#pragma warning restore CS0618

    [Fact]
    public void Clone_Copies_Alias_Expressions()
    {
        var update = new AliasSqlUpdate("T");
        update.SetAlias("j1", "Table J1");

        var clone = update.Clone();

        Assert.True(clone.HasAlias("j1"));

        update.SetAlias("j2", "Table J2");

        Assert.False(clone.HasAlias("j2"));
    }

    private sealed class AliasSqlUpdate(string table) : SqlUpdate(table)
    {
        public void SetAlias(string alias, string expression) => SetAliasExpression(alias, expression);
    }

    [Fact]
    public void Freeze_Prevents_Modification()
    {
        var update = new SqlUpdate("T").SetTo("A", "1");
        update.Freeze();

        Assert.Throws<InvalidOperationException>(() => update.SetTo("B", "2"));
    }
}
