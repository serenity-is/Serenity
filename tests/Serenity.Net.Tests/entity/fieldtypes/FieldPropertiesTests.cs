namespace Serenity.Data;

public class FieldPropertiesTests
{
    public class FkNoJoinRow : Row<FkNoJoinRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field CountryID;
            public RowFields()
            {
                CountryID = new Int32Field(this, "CountryID");
            }
        }

        [ForeignKey("TheCountryTable", "TheCountryID")]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }
    }

    [Fact]
    [Obsolete("Test for obsolete method")]
    public void ForeignJoin_CreatesJoinWithAliasFromFieldName()
    {
        var f = new FkNoJoinRow.RowFields();
        f.Initialize(null, SqlSettings.DefaultDialect);

        var byId = f.CountryID;
        byId.ForeignTable = "TheCountryTable";
        byId.ForeignField = "TheCountryID";
        var join = byId.ForeignJoin();
        Assert.NotNull(join);
        Assert.Equal("jCountryID", join.Name);

        var bySuffix = f.CountryID;
        bySuffix.ForeignTable = "TheCountryTable";
        bySuffix.Expression = "SomeInvalid('alias')";
        bySuffix.Expression = null;
        var joinSfx = bySuffix.ForeignJoin(2);
        Assert.NotNull(joinSfx);
    }

    [Fact]
    public void BasicProperties_SetGet()
    {
        var f = new ComplexRow.RowFields();
        f.Initialize(null, SqlSettings.DefaultDialect);
        var field = f.Overriden;

        field.DefaultValue = "x";
        Assert.Equal("x", field.DefaultValue);

        var aliases = new HashSet<string> { "a", "b" };
        field.ReferencedAliases = aliases;
        Assert.Same(aliases, field.ReferencedAliases);
        
        field.NaturalOrder = 5;
        Assert.Equal(5, field.NaturalOrder);

        field.ReadPermission = "read";
        field.InsertPermission = "insert";
        field.UpdatePermission = "update";
        Assert.Equal("read", field.ReadPermission);
        Assert.Equal("insert", field.InsertPermission);
        Assert.Equal("update", field.UpdatePermission);
    }

    [Fact]
    public void ExpressionSetter_ResolvesJoinAliasByFieldName()
    {
        var f = new ComplexRow.RowFields();
        f.Initialize(null, SqlSettings.DefaultDialect);
        var field = f.BasicExpression; // plain propSomeField

        field.Expression = "c.Name";
        Assert.NotNull(field.Join);
        Assert.Equal("c", field.JoinAlias);
        Assert.Equal("Name", field.Origin);

        field.Expression = "T0.Simple";
        Assert.Equal("T0.Simple", field.Expression);
        Assert.Null(field.JoinAlias);
        Assert.Null(field.Join);

        field.Expression = "Complex(x + y)";
        Assert.Equal("Complex(x + y)", field.Expression);
        Assert.True(field.Flags.HasFlag(FieldFlags.Calculated));
    }
}
