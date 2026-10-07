namespace Serenity.Data;

public class ComparerTests
{
    // ---- ComparerResolver ----

    [Fact]
    public void ComparerResolver_NullOrEmpty_ReturnsNull()
    {
        Assert.Null(ComparerResolver.Parse(null));
        Assert.Null(ComparerResolver.Parse(""));
        Assert.Null(ComparerResolver.Parse("   "));
    }

    [Fact]
    public void ComparerResolver_Parses_StringComparison_Names()
    {
        Assert.Same(StringComparer.Ordinal, ComparerResolver.Parse("Ordinal"));
        Assert.Same(StringComparer.Ordinal, ComparerResolver.Parse("ordinal"));
        Assert.Same(StringComparer.OrdinalIgnoreCase, ComparerResolver.Parse("OrdinalIgnoreCase"));

        var current = ComparerResolver.Parse("CurrentCulture");
        Assert.NotNull(current);
        Assert.NotEqual(0, current!.Compare("a", "A"));
        Assert.Equal(0, current.Compare("a\u00ADb", "ab"));

        var invariantIgnoreCase = ComparerResolver.Parse("InvariantCultureIgnoreCase");
        Assert.NotNull(invariantIgnoreCase);
        Assert.Equal(0, invariantIgnoreCase!.Compare("a", "A"));
    }

    [Fact]
    public void ComparerResolver_Parses_Culture()
    {
        var sensitive = ComparerResolver.Parse("tr-TR");
        Assert.NotNull(sensitive);
        Assert.NotEqual(0, sensitive!.Compare("a", "A"));

        var insensitive = ComparerResolver.Parse("tr-TR:IgnoreCase");
        Assert.NotNull(insensitive);
        Assert.Equal(0, insensitive!.Compare("a", "A"));
    }

    [Fact]
    public void ComparerResolver_Invalid_Throws()
    {
        Assert.ThrowsAny<ArgumentException>(() => ComparerResolver.Parse("tr-TR:CaseSensitive"));
    }

    [Fact]
    public void ComparerResolver_TryParse()
    {
        Assert.False(ComparerResolver.TryParse(null, out _));
        Assert.False(ComparerResolver.TryParse("tr-TR:CaseSensitive", out _));
        Assert.True(ComparerResolver.TryParse("Ordinal", out var comparer));
        Assert.Same(StringComparer.Ordinal, comparer);
    }

    // ---- ComparerAttribute ----

    [Fact]
    public void ComparerAttribute_FromComparison()
    {
        var attr = new ComparerAttribute(StringComparison.OrdinalIgnoreCase);
        Assert.Same(StringComparer.OrdinalIgnoreCase, attr.ToStringComparer());
    }

    [Fact]
    public void ComparerAttribute_FromCulture()
    {
        Assert.NotEqual(0, new ComparerAttribute("tr-TR").ToStringComparer().Compare("a", "A"));
        Assert.Equal(0, new ComparerAttribute("tr-TR", true).ToStringComparer().Compare("a", "A"));
    }

    // ---- SqlSettings ----

    [Fact]
    public void SqlSettings_DefaultComparer_IsCurrentCulture()
    {
        var comparer = SqlSettings.DefaultComparer;
        Assert.NotEqual(0, comparer.Compare("a", "A"));
        Assert.Equal(0, comparer.Compare("a\u00ADb", "ab"));
    }

    [Fact]
    public void SqlSettings_SetLocalComparer_OverridesAndRestores()
    {
        var initial = SqlSettings.DefaultComparer;
        var old = SqlSettings.SetLocalComparer(StringComparer.Ordinal);
        try
        {
            Assert.Same(StringComparer.Ordinal, SqlSettings.DefaultComparer);
        }
        finally
        {
            SqlSettings.SetLocalComparer(old);
        }

        Assert.Same(initial, SqlSettings.DefaultComparer);
    }

    // ---- Dialect ----

    [Fact]
    public void Dialect_WithComparer_ClonesWithoutMutatingSingleton()
    {
        var instance = SqlServer2012Dialect.Instance;
        Assert.Null(instance.Comparer);

        var clone = instance.WithComparer(StringComparer.OrdinalIgnoreCase);

        Assert.NotSame(instance, clone);
        Assert.Same(StringComparer.OrdinalIgnoreCase, clone.Comparer);
        Assert.Null(instance.Comparer);
    }

    private class BareDialect : ISqlDialect
    {
        public bool CanUseOffsetFetch => false;
        public bool CanUseRowNumber => false;
        public bool CanUseSkipKeyword => false;
        public char CloseQuote => ']';
        public string ConcatOperator => "+";
        public string DateFormat => "";
        public string DateTimeFormat => "";
        public bool IsLikeCaseSensitive => false;
        public bool MultipleResultsets => false;
        public bool NeedsExecuteBlockStatement => false;
        public bool NeedsBoolWorkaround => false;
        public string OffsetFormat => "";
        public string OffsetFetchFormat => "";
        public char OpenQuote => '[';
        public string ScopeIdentityExpression => "";
        public string ServerType => "Bare";
        public string SkipKeyword => "";
        public string TakeKeyword => "";
        public string TimeFormat => "";
        public bool UseDateTime2 => false;
        public bool UseReturningIdentity => false;
        public bool UseReturningIntoVar => false;
        public bool UseScopeIdentity => false;
        public bool UseTakeAtEnd => false;
        public bool UseRowNum => false;
        public char ParameterPrefix => '@';

        public string QuoteColumnAlias(string s) => s;
        public string QuoteIdentifier(string s) => s;
        public string QuoteUnicodeString(string s) => s;
        public string UnionKeyword(SqlUnionType unionType) => "UNION";
    }

    [Fact]
    public void Dialect_WithComparer_DefaultThrows_ForUnsupportedDialect()
    {
        ISqlDialect dialect = new BareDialect();

        Assert.Null(dialect.Comparer);
        Assert.Throws<NotSupportedException>(() => dialect.WithComparer(StringComparer.Ordinal));
    }

    // ---- Connection entry ----

    [Fact]
    public void ConnectionStrings_Applies_Comparer()
    {
        var options = new ConnectionStringOptions
        {
            ["Test"] = new ConnectionStringEntry
            {
                ConnectionString = "cs",
                ProviderName = "System.Data.SqlClient",
                Comparer = "OrdinalIgnoreCase"
            }
        };

        var info = new DefaultConnectionStrings(options).TryGetConnectionString("Test");

        Assert.NotNull(info);
        Assert.Same(StringComparer.OrdinalIgnoreCase, info!.Dialect.Comparer);
        Assert.Null(SqlServer2012Dialect.Instance.Comparer);
    }

    [Fact]
    public void ConnectionStrings_InvalidComparer_Throws()
    {
        var options = new ConnectionStringOptions
        {
            ["Test"] = new ConnectionStringEntry
            {
                ConnectionString = "cs",
                ProviderName = "System.Data.SqlClient",
                Comparer = "tr-TR:CaseSensitive"
            }
        };

        var dialects = new DefaultConnectionStrings(options);
        Assert.ThrowsAny<ArgumentException>(() => dialects.TryGetConnectionString("Test"));
    }

    // ---- Field resolution ----

    [Comparer(StringComparison.OrdinalIgnoreCase)]
    private class RowComparerRow : Row<RowComparerRow.RowFields>
    {
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        [Comparer(StringComparison.Ordinal)]
        public string? Code { get => fields.Code[this]; set => fields.Code[this] = value; }

        public RowComparerRow() { }
        public RowComparerRow(RowFields fields) : base(fields) { }

        public class RowFields : RowFieldsBase
        {
            public StringField Name = null!;
            public StringField Code = null!;
        }
    }

    private class NoAttrRow : Row<NoAttrRow.RowFields>
    {
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public NoAttrRow() { }
        public NoAttrRow(RowFields fields) : base(fields) { }

        public class RowFields : RowFieldsBase
        {
            public StringField Name = null!;
        }
    }

    private static RowComparerRow.RowFields InitRowComparerFields()
    {
        var fields = new RowComparerRow.RowFields();
        fields.Initialize(annotations: null, dialect: SqlServer2012Dialect.Instance, userEntityOptions: null);
        return fields;
    }

    [Fact]
    public void Initialize_FieldComparer_Overrides_RowComparer()
    {
        var fields = InitRowComparerFields();

        Assert.Same(StringComparer.OrdinalIgnoreCase, fields.Name.Comparer);
        Assert.Same(StringComparer.Ordinal, fields.Code.Comparer);
    }

    [Fact]
    public void Initialize_Default_Is_SqlSettings_Default()
    {
        var fields = new NoAttrRow.RowFields();
        fields.Initialize(annotations: null, dialect: SqlServer2012Dialect.Instance, userEntityOptions: null);

        Assert.Same(SqlSettings.DefaultComparer, fields.Name.Comparer);
    }

    [Fact]
    public void Initialize_Uses_Dialect_Comparer_WhenNoAttribute()
    {
        var dialect = SqlServer2012Dialect.Instance.WithComparer(StringComparer.OrdinalIgnoreCase);
        var fields = new NoAttrRow.RowFields();
        fields.Initialize(annotations: null, dialect: dialect, userEntityOptions: null);

        Assert.Same(StringComparer.OrdinalIgnoreCase, fields.Name.Comparer);
    }

    [Fact]
    public void Initialize_LocalComparer_IsBakedIn()
    {
        var old = SqlSettings.SetLocalComparer(StringComparer.OrdinalIgnoreCase);
        try
        {
            var fields = new NoAttrRow.RowFields();
            fields.Initialize(annotations: null, dialect: SqlServer2012Dialect.Instance, userEntityOptions: null);
            Assert.Same(StringComparer.OrdinalIgnoreCase, fields.Name.Comparer);

            SqlSettings.SetLocalComparer(StringComparer.Ordinal);
            Assert.Same(StringComparer.OrdinalIgnoreCase, fields.Name.Comparer);
        }
        finally
        {
            SqlSettings.SetLocalComparer(old);
        }
    }

    [Fact]
    public void IndexCompare_Uses_Row_And_Field_Comparers()
    {
        var fields = InitRowComparerFields();
        var row1 = new RowComparerRow(fields) { Name = "a", Code = "a" };
        var row2 = new RowComparerRow(fields) { Name = "A", Code = "A" };

        Assert.Equal(0, fields.Name.IndexCompare(row1, row2));
        Assert.NotEqual(0, fields.Code.IndexCompare(row1, row2));
    }

    [Comparer(StringComparison.OrdinalIgnoreCase)]
    private class InnerComparerRow : Row<InnerComparerRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public StringField Name = null!;
            public RowFields() { Name = new(this, "Name"); }
        }

        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public InnerComparerRow() { }
        public InnerComparerRow(RowFields fields) : base(fields) { }
    }

    private class OuterComparerRow : Row<OuterComparerRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public RowField<InnerComparerRow> Inner = null!;
            public RowFields() { Inner = new(this, "Inner"); }
        }

        public InnerComparerRow? Inner { get => fields.Inner[this]; set => fields.Inner[this] = value; }

        public OuterComparerRow() { }
        public OuterComparerRow(RowFields fields) : base(fields) { }
    }

    [Fact]
    public void RowField_NoComparer_Uses_InnerFieldComparer()
    {
        var outerFields = new OuterComparerRow.RowFields();
        outerFields.Initialize(annotations: null, dialect: SqlServer2012Dialect.Instance, userEntityOptions: null);
        var innerFields = new InnerComparerRow.RowFields();
        innerFields.Initialize(annotations: null, dialect: SqlServer2012Dialect.Instance, userEntityOptions: null);

        Assert.Same(StringComparer.OrdinalIgnoreCase, innerFields.Name.Comparer);

        var row1 = new OuterComparerRow(outerFields) { Inner = new InnerComparerRow(innerFields) { Name = "a" } };
        var row2 = new OuterComparerRow(outerFields) { Inner = new InnerComparerRow(innerFields) { Name = "A" } };

        Assert.Equal(0, outerFields.Inner.IndexCompare(row1, row2));
        Assert.NotEqual(0, outerFields.Inner.IndexCompare(row1, row2, StringComparer.Ordinal));
    }
}
