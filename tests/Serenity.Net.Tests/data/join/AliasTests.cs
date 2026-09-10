namespace Serenity.Data;

public class AliasTests
{
    private class MyField : IField
    {
        public string Name { get; set; } = "FieldName";
        public string Expression { get; set; } = "FieldName";
        public string ColumnAlias { get; set; } = "FieldName";
    }

    [Fact]
    public void Alias_With_String_Name_Has_No_Table()
    {
        var alias = new Alias("x");

        Assert.Equal("x", alias.Name);
        Assert.Equal("x.", alias.NameDot);
        Assert.Null(alias.Table);
    }

    [Fact]
    public void Alias_With_Table_And_String_Name()
    {
        var alias = new Alias("TableName", "x");

        Assert.Equal("x", alias.Name);
        Assert.Equal("x.", alias.NameDot);
        Assert.Equal("TableName", alias.Table);
    }

    [Fact]
    public void Alias_With_Int_Index_Generates_TAlias()
    {
        var alias = new Alias(0);

        Assert.Equal("T0", alias.Name);
        Assert.Equal("T0.", alias.NameDot);
        Assert.Null(alias.Table);
    }

    [Fact]
    public void Alias_With_Int_Index_Generates_TAlias_For_Higher_Indices()
    {
        Assert.Equal("T3", new Alias(3).Name);
        Assert.Equal("T3.", new Alias(3).NameDot);
        Assert.Equal("T9", new Alias(9).Name);
        Assert.Equal("T9.", new Alias(9).NameDot);
    }

    [Fact]
    public void Alias_With_Table_And_Int_Index()
    {
        var alias = new Alias("TableName", 3);

        Assert.Equal("T3", alias.Name);
        Assert.Equal("T3.", alias.NameDot);
        Assert.Equal("TableName", alias.Table);
    }

    [Fact]
    public void Alias_With_Empty_Name_Throws_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Alias(""));
    }

    [Fact]
    public void Alias_With_Null_Name_Throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Alias((string)null!));
    }

    [Fact]
    public void Static_Aliases_Are_T0_To_T9()
    {
        Assert.Equal("T0", Alias.T0.Name);
        Assert.Equal("T1", Alias.T1.Name);
        Assert.Equal("T2", Alias.T2.Name);
        Assert.Equal("T3", Alias.T3.Name);
        Assert.Equal("T4", Alias.T4.Name);
        Assert.Equal("T5", Alias.T5.Name);
        Assert.Equal("T6", Alias.T6.Name);
        Assert.Equal("T7", Alias.T7.Name);
        Assert.Equal("T8", Alias.T8.Name);
        Assert.Equal("T9", Alias.T9.Name);
    }

    [Fact]
    public void Indexer_With_FieldName_Prefixes_With_AliasDot()
    {
        var alias = new Alias("x");

        Assert.Equal("x.Field", alias["Field"]);
    }

    [Fact]
    public void Indexer_With_Field_Prefixes_Field_Name()
    {
        var alias = new Alias("x");

        Assert.Equal("x.FieldName", alias[(IField)new MyField()]);
    }

    [Fact]
    public void Indexer_With_Null_Field_Throws_ArgumentNullException()
    {
        var alias = new Alias("x");

        Assert.Throws<ArgumentNullException>(() => alias[(IField)null!]);
    }

    [Fact]
    public void Underscore_Method_With_FieldName_Returns_Criteria()
    {
        var alias = new Alias("x");

        var criteria = alias._("Field");

        Assert.Equal("x.Field", criteria.ToString());
    }

    [Fact]
    public void Underscore_Method_With_Field_Returns_Criteria()
    {
        var alias = new Alias("x");

        var criteria = alias._(new MyField());

        Assert.Equal("x.FieldName", criteria.ToString());
    }

    [Fact]
    public void Plus_Operator_With_FieldName_Returns_Prefixed_String()
    {
        var alias = new Alias("x");

        Assert.Equal("x.Field", alias + "Field");
    }

    [Fact]
    public void Plus_Operator_With_Field_Returns_Prefixed_String()
    {
        var alias = new Alias("x");

        Assert.Equal("x.FieldName", alias + (IField)new MyField());
    }

    [Fact]
    public void Plus_Operator_With_Null_Field_Throws_ArgumentNullException()
    {
        var alias = new Alias("x");

        Assert.Throws<ArgumentNullException>(() => alias + (IField)null!);
    }

    [Fact]
    public void WithNoLock_Appends_Hint_To_Name_Without_Table()
    {
        var alias = new Alias("x").WithNoLock();

        Assert.Equal("x WITH(NOLOCK)", alias.Name);
        Assert.Equal("x WITH(NOLOCK).", alias.NameDot);
        Assert.Null(alias.Table);
    }

    [Fact]
    public void WithNoLock_Keeps_Table()
    {
        var alias = new Alias("TableName", "x").WithNoLock();

        Assert.Equal("x WITH(NOLOCK)", alias.Name);
        Assert.Equal("x WITH(NOLOCK).", alias.NameDot);
        Assert.Equal("TableName", alias.Table);
    }

    [Fact]
    public void WithNoLock_Works_On_Join_And_Returns_Plain_Alias()
    {
        IAlias join = new LeftJoin("Table", "T1", null);

        var alias = join.WithNoLock();

        Assert.Equal("T1 WITH(NOLOCK)", alias.Name);
        Assert.Equal("Table", alias.Table);
        Assert.IsNotType<LeftJoin>(alias);
    }
}
