using Serenity.TestUtils;

namespace Serenity.Data.Mapping;

public class MappingAttributesTests
{
    private class UnknownDialect : SqlServer2012Dialect
    {
        public override string ServerType => "Unknown";
    }

    private class TranslatorDialect : SqlServer2012Dialect, ISqlExpressionTranslator
    {
        public string Translate(object expression) => null!;
    }

    private class ConstantExpressionAttribute : BaseExpressionAttribute
    {
        public override string Translate(ISqlDialect dialect) => "C";
    }

    [ConnectionKey("LocalKey")]
    private class ConnKeySource
    {
    }

    private class NoConnKeySource
    {
    }

    private class NoTableNameType
    {
    }

    [TableName("NoIdentity")]
    private class NoIdentityType
    {
        public int? Id { get; set; }
    }

    [TableName("MultiKeyOneIdentity")]
    private class MultiKeyOneIdentityType
    {
        [PrimaryKey]
        public int? A { get; set; }

        [Identity]
        public int? B { get; set; }
    }

    [TableName("MultiIdentity")]
    private class MultiIdentityType
    {
        [Identity]
        public int? A { get; set; }

        [Identity]
        public int? B { get; set; }
    }

    [Fact]
    public void AuditedAttribute_Value()
    {
        Assert.True(new AuditedAttribute().Value);
        Assert.False(new AuditedAttribute(false).Value);
    }

    [Fact]
    public void ClientSideAttribute_Sets_Flags()
    {
#pragma warning disable CS0618
        var attr = new ClientSideAttribute();
        Assert.Equal(FieldFlags.ClientSide, attr.Add);
#pragma warning restore CS0618
    }

    [Fact]
    public void ColumnAttribute_Validates_And_Sets_Name()
    {
        Assert.Equal("A", new ColumnAttribute("A").Name);
        Assert.Throws<ArgumentNullException>(() => new ColumnAttribute(null!));
        Assert.Throws<ArgumentNullException>(() => new ColumnAttribute(""));
    }

    [Fact]
    public void ConnectionKeyAttribute_String_Constructor()
    {
        var attr = new ConnectionKeyAttribute("K");
        Assert.Equal("K", attr.Value);
        Assert.Null(attr.SourceType);
        Assert.Throws<ArgumentNullException>(() => new ConnectionKeyAttribute((string)null!));
    }

    [Fact]
    public void ConnectionKeyAttribute_Type_Constructor()
    {
        var attr = new ConnectionKeyAttribute(typeof(ConnKeySource));
        Assert.Equal("LocalKey", attr.Value);
        Assert.Equal(typeof(ConnKeySource), attr.SourceType);

        Assert.Throws<ArgumentNullException>(() => new ConnectionKeyAttribute((Type)null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ConnectionKeyAttribute(typeof(NoConnKeySource)));
    }

    [Fact]
    public void DataAuditLogAttribute_Can_Be_Created()
    {
        Assert.NotNull(new DataAuditLogAttribute());
    }

    [Fact]
    public void DatabaseAliasAttribute_Value()
    {
        Assert.Equal("db", new DatabaseAliasAttribute("db").Value);
    }

    [Fact]
    public void EmitFieldTypeAttribute_Validates_And_Sets_Type()
    {
        Assert.Equal(typeof(StringField), new EmitFieldTypeAttribute(typeof(StringField)).FieldType);
        Assert.Throws<ArgumentNullException>(() => new EmitFieldTypeAttribute(null!));
    }

    [Fact]
    public void EmitNameAttribute_Validates_And_Sets_Name()
    {
        Assert.Equal("N", new EmitNameAttribute("N").Name);
        Assert.Throws<ArgumentNullException>(() => new EmitNameAttribute(null!));
    }

    [Fact]
    public void ExpressionAttribute_Validates_And_Translates()
    {
        var attr = new ExpressionAttribute("T0.A");
        Assert.Equal("T0.A", attr.Value);
        Assert.Equal("T0.A", attr.Translate(MySqlDialect.Instance));
        Assert.Throws<ArgumentNullException>(() => new ExpressionAttribute(null!));

        var dialectAttr = new ExpressionAttribute("T0.A", ServerType.MySql);
        Assert.Equal("MySql", dialectAttr.Dialect);
    }

    [Fact]
    public void ExpressionAttribute_ToString_And_Format()
    {
        var attr = new ExpressionAttribute("T0.A");
        Assert.Equal("T0.A", attr.ToString(MySqlDialect.Instance));

        attr.Format = "({0})";
        Assert.Equal("(T0.A)", attr.ToString(MySqlDialect.Instance));
    }

    [Fact]
    public void NegateDialect_Get_Set_Behavior()
    {
        var attr = new ExpressionAttribute("T0.A") { Dialect = "MySql" };
        Assert.False(attr.NegateDialect);

        attr.NegateDialect = true;
        Assert.True(attr.NegateDialect);
        Assert.Equal("!MySql", attr.Dialect);

        attr.NegateDialect = true;
        Assert.Equal("!MySql", attr.Dialect);

        attr.NegateDialect = false;
        Assert.Equal("MySql", attr.Dialect);

        var noDialect = new ExpressionAttribute("T0.A");
        Assert.False(noDialect.NegateDialect);
        noDialect.NegateDialect = false;
        Assert.Null(noDialect.Dialect);
    }

    [Theory]
    [InlineData(typeof(SqlServer2012Dialect), "SYSDATETIMEOFFSET")]
    [InlineData(typeof(OracleDialect), "SYSTIMESTAMP")]
    [InlineData(typeof(PostgresDialect), "now")]
    [InlineData(typeof(MySqlDialect), "CURRENT_TIMESTAMP")]
    public void SqlDateTimeOffsetAttribute_Translates(Type dialectType, string expected)
    {
        var dialect = (ISqlDialect)Activator.CreateInstance(dialectType)!;
        Assert.Equal(expected, new SqlDateTimeOffsetAttribute().Translate(dialect));
    }

    [Theory]
    [InlineData(typeof(FirebirdDialect), "LOCALTIMESTAMP")]
    [InlineData(typeof(SqliteDialect), "DATETIME('now', 'localtime')")]
    [InlineData(typeof(SqlServer2012Dialect), "SYSDATETIME()")]
    [InlineData(typeof(MySqlDialect), "CURRENT_TIMESTAMP")]
    public void SqlNowAttribute_Translates(Type dialectType, string expected)
    {
        var dialect = (ISqlDialect)Activator.CreateInstance(dialectType)!;
        Assert.Equal(expected, new SqlNowAttribute().Translate(dialect));
    }

    [Theory]
    [InlineData(typeof(FirebirdDialect), "DATEDIFF(second, timestamp '1/1/1970 00:00:00', current_timestamp)")]
    [InlineData(typeof(MySqlDialect), "UTC_TIMESTAMP")]
    [InlineData(typeof(OracleDialect), "SYS_EXTRACT_UTC(SYSTIMESTAMP)")]
    [InlineData(typeof(PostgresDialect), "TIMEZONE('utc', now())")]
    [InlineData(typeof(SqliteDialect), "DATETIME('now')")]
    [InlineData(typeof(SqlServer2012Dialect), "SYSUTCDATETIME()")]
    [InlineData(typeof(UnknownDialect), "CURRENT_TIMESTAMP")]
    public void SqlUtcNowAttribute_Translates(Type dialectType, string expected)
    {
        var dialect = (ISqlDialect)Activator.CreateInstance(dialectType)!;
        Assert.Equal(expected, new SqlUtcNowAttribute().Translate(dialect));
    }

    [Fact]
    public void TableNameAttribute_Validates_And_Sets()
    {
        var attr = new TableNameAttribute("T");
        Assert.Equal("T", attr.Name);
        Assert.Null(attr.Dialect);
        Assert.Throws<ArgumentNullException>(() => new TableNameAttribute(null!));
        Assert.Throws<ArgumentNullException>(() => new TableNameAttribute(""));

        var dialectAttr = new TableNameAttribute("T", ServerType.Sqlite);
        Assert.Equal("Sqlite", dialectAttr.Dialect);
        Assert.False(dialectAttr.NegateDialect);
        dialectAttr.NegateDialect = true;
        Assert.True(dialectAttr.NegateDialect);
        dialectAttr.NegateDialect = false;
        Assert.Equal("Sqlite", dialectAttr.Dialect);
    }

    [Fact]
    public void TwoLevelCachedAttribute_Constructors()
    {
        Assert.NotNull(new TwoLevelCachedAttribute());
        Assert.Equal(new[] { "a", "b" }, new TwoLevelCachedAttribute("a", "b").GenerationKeys);
        Assert.Equal(new[] { typeof(string) }, new TwoLevelCachedAttribute(typeof(string)).LinkedRows);
    }

    [Fact]
    public void UniqueAttribute_Defaults_And_Properties()
    {
        var attr = new UniqueAttribute();
        Assert.Equal(FieldFlags.Unique, attr.Add);
        Assert.True(attr.CheckBeforeSave);

        attr.Name = "U";
        attr.IgnoreDeleted = true;
        attr.IgnoreNulls = true;
        attr.ErrorMessage = "e";

        Assert.Equal("U", attr.Name);
        Assert.True(attr.IgnoreDeleted);
        Assert.True(attr.IgnoreNulls);
        Assert.Equal("e", attr.ErrorMessage);
    }

    [Fact]
    public void UniqueConstraintAttribute_Validates_And_Sets()
    {
        var attr = new UniqueConstraintAttribute("A", "B");
        Assert.Equal(new[] { "A", "B" }, attr.Fields);
        Assert.True(attr.CheckBeforeSave);

        Assert.Throws<ArgumentNullException>(() => new UniqueConstraintAttribute());
        Assert.Throws<ArgumentNullException>(() => new UniqueConstraintAttribute((string[])null!));

        attr.Name = "C";
        attr.IgnoreDeleted = true;
        attr.ErrorMessage = "e";
        Assert.Equal("C", attr.Name);
        Assert.True(attr.IgnoreDeleted);
        Assert.Equal("e", attr.ErrorMessage);
    }

    [Fact]
    public void UpdatableExtensionAttribute_Validates_And_Sets()
    {
        var attr = new UpdatableExtensionAttribute("x", typeof(StringField));
        Assert.Equal("x", attr.Alias);
        Assert.Equal(typeof(StringField), attr.RowType);

        Assert.Throws<ArgumentNullException>(() => new UpdatableExtensionAttribute("", typeof(StringField)));
        Assert.Throws<ArgumentNullException>(() => new UpdatableExtensionAttribute("x", null!));

        attr.ThisKey = "a";
        attr.OtherKey = "b";
        attr.FilterField = "c";
        attr.FilterValue = 1;
        attr.PresenceField = "d";
        attr.PresenceValue = 2;
        attr.CascadeDelete = true;

        Assert.Equal("a", attr.ThisKey);
        Assert.Equal("b", attr.OtherKey);
        Assert.Equal("c", attr.FilterField);
        Assert.Equal(1, attr.FilterValue);
        Assert.Equal("d", attr.PresenceField);
        Assert.Equal(2, attr.PresenceValue);
        Assert.True(attr.CascadeDelete);
    }

    [Fact]
    public void InnerJoinAttribute_Constructors_And_Properties()
    {
        var attr = new InnerJoinAttribute("a");
        Assert.Equal("a", attr.Alias);
        Assert.Null(attr.ToTable);

        var full = new InnerJoinAttribute("a", "T", "c");
        Assert.Equal("T", full.ToTable);
        Assert.Equal("c", full.OnCriteria);

        var dialectAttr = new InnerJoinAttribute("a", ServerType.MySql);
        Assert.Equal("MySql", dialectAttr.Dialect);

        var fullDialect = new InnerJoinAttribute("a", "T", "c", ServerType.Sqlite);
        Assert.Equal("Sqlite", fullDialect.Dialect);

        full.PropertyPrefix = "p";
        full.TitlePrefix = "t";
        full.RowType = typeof(StringField);
        Assert.Equal("p", full.PropertyPrefix);
        Assert.Equal("t", full.TitlePrefix);
        Assert.Equal(typeof(StringField), full.RowType);

        dialectAttr.NegateDialect = true;
        Assert.Equal("!MySql", dialectAttr.Dialect);
        dialectAttr.NegateDialect = false;
        Assert.Equal("MySql", dialectAttr.Dialect);
    }

    [Fact]
    public void OuterApplyAttribute_Constructors_And_Properties()
    {
        var attr = new OuterApplyAttribute("a", "SELECT 1");
        Assert.Equal("a", attr.Alias);
        Assert.Equal("SELECT 1", attr.InnerQuery);
        Assert.Equal("SELECT 1", ((ISqlJoin)attr).OnCriteria);
        Assert.Null(((ISqlJoin)attr).ToTable);

        var dialectAttr = new OuterApplyAttribute("a", "SELECT 1", ServerType.MySql);
        Assert.Equal("MySql", dialectAttr.Dialect);

        attr.PropertyPrefix = "p";
        attr.TitlePrefix = "t";
        attr.RowType = typeof(StringField);
        Assert.Equal("p", attr.PropertyPrefix);
        Assert.Equal("t", attr.TitlePrefix);
        Assert.Equal(typeof(StringField), attr.RowType);

        dialectAttr.NegateDialect = true;
        Assert.Equal("!MySql", dialectAttr.Dialect);
        dialectAttr.NegateDialect = false;
        Assert.Equal("MySql", dialectAttr.Dialect);
    }

    [Fact]
    public void ForeignKeyAttribute_String_Constructor()
    {
        var attr = new ForeignKeyAttribute("T", "F");
        Assert.Equal("T", attr.Table);
        Assert.Equal("F", attr.Field);
        Assert.Null(attr.RowType);

        Assert.Throws<ArgumentNullException>(() => new ForeignKeyAttribute((string)null!, "F"));
        Assert.Throws<ArgumentNullException>(() => new ForeignKeyAttribute("T", null!));

        var dialectAttr = new ForeignKeyAttribute("T", "F", ServerType.MySql);
        Assert.Equal("MySql", dialectAttr.Dialect);
        Assert.False(dialectAttr.NegateDialect);
        dialectAttr.NegateDialect = true;
        Assert.Equal("!MySql", dialectAttr.Dialect);
        dialectAttr.NegateDialect = false;
        Assert.Equal("MySql", dialectAttr.Dialect);
    }

    [Fact]
    public void ForeignKeyAttribute_Type_Constructor()
    {
        var attr = new ForeignKeyAttribute(typeof(CityRow));
        Assert.Null(attr.Table);
        Assert.Equal("CityId", attr.Field);
        Assert.Equal(typeof(CityRow), attr.RowType);

        var explicitField = new ForeignKeyAttribute(typeof(CityRow), "CityName");
        Assert.Equal("CityName", explicitField.Field);

        var dialectAttr = new ForeignKeyAttribute(typeof(CityRow), "CityName", ServerType.Sqlite);
        Assert.Equal("Sqlite", dialectAttr.Dialect);

        Assert.Throws<ArgumentNullException>(() => new ForeignKeyAttribute((Type)null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ForeignKeyAttribute(typeof(NoTableNameType)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ForeignKeyAttribute(typeof(NoIdentityType)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ForeignKeyAttribute(typeof(MultiIdentityType)));

        var multi = new ForeignKeyAttribute(typeof(MultiKeyOneIdentityType));
        Assert.Equal("B", multi.Field);
    }

    [Fact]
    public void BaseExpressionAttribute_Static_ToString_Converts_Values()
    {
        var dialect = SqlServer2012Dialect.Instance;

        Assert.Throws<ArgumentNullException>(() => BaseExpressionAttribute.ToString("x", null!));
        Assert.NotNull(BaseExpressionAttribute.ToString((object?)null, dialect));
        Assert.Equal("abc", BaseExpressionAttribute.ToString("abc", dialect));
        Assert.Equal("5", BaseExpressionAttribute.ToString(5, dialect));
        Assert.Equal("1", BaseExpressionAttribute.ToString(true, dialect));
        Assert.Equal("1.5", BaseExpressionAttribute.ToString(1.5d, dialect));
        Assert.Equal("2.5", BaseExpressionAttribute.ToString(2.5m, dialect));
        Assert.Equal("9", BaseExpressionAttribute.ToString(9L, dialect));
        Assert.NotNull(BaseExpressionAttribute.ToString(DateTime.Now, dialect));
        Assert.Equal("C", BaseExpressionAttribute.ToString(typeof(ConstantExpressionAttribute), dialect));
        Assert.Equal("T0.A", BaseExpressionAttribute.ToString(
            new object[] { typeof(ExpressionAttribute), "T0.A" }, dialect));
        Assert.Equal("1.5", BaseExpressionAttribute.ToString(1.5f, dialect));
    }

    [Fact]
    public void BaseExpressionAttribute_ToString_Uses_Translator_When_Available()
    {
        var attr = new ExpressionAttribute("T0.A");

        Assert.Equal("T0.A", attr.ToString(new TranslatorDialect()));
        Assert.Equal("T0.A", attr.ToString(MySqlDialect.Instance));
    }
}
