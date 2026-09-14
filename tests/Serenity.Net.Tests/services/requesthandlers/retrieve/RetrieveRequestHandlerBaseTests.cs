namespace Serenity.Services;


public class RetrieveRequestHandlerBaseTests
{
    [ReadPermission("Test.RetrievePermission")]
    private class RetrievePermissionRow : Row<RetrievePermissionRow.RowFields>
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null;
        }
    }

    [TableName("RetrieveBase")]
    private class RetrieveBaseRow : Row<RetrieveBaseRow.RowFields>
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [NameProperty]
        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        [Column("external_name")]
        public int? External { get => fields.External[this]; set => fields.External[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null;
            public StringField Name = null;
            public Int32Field External = null;
        }

        public RetrieveBaseRow()
        {
        }

        public RetrieveBaseRow(RowFields fields)
            : base(fields)
        {
        }
    }

    private static NullRequestContext Context(Func<string, bool>? hasPermission = null) =>
        new NullRequestContext().WithPermissions(hasPermission ?? (_ => true));

    private class TestHandler(IRequestContext context) : RetrieveRequestHandlerBase<RetrieveBaseRow, RetrieveRequest, RetrieveResponse<RetrieveBaseRow>>(context)
    {
        public void Init(IDbConnection connection, SqlQuery query, RetrieveRequest request, RetrieveBaseRow.RowFields fields)
        {
            Connection = connection;
            Query = query;
            Row = new RetrieveBaseRow(fields);
            Request = request;
            Response = new RetrieveResponse<RetrieveBaseRow>();
        }

        public bool Allow(Field field) => AllowSelectField(field);
        public bool Should(Field field) => ShouldSelectField(field);
        public bool Included(Field field) => IsIncluded(field);
        public bool Included(string column) => IsIncluded(column);
        public void Select(SqlQuery query, Field field) => SelectField(query, field);
        public void SelectAll(SqlQuery query) => SelectFields(query);
        public void Validate() => ValidatePermissions();
        public SqlQuery Build() => CreateQuery();
        public ITwoLevelCache CacheValue => Cache;
    }

    private class PermissionHandler(IRequestContext context) : RetrieveRequestHandlerBase<RetrievePermissionRow, RetrieveRequest, RetrieveResponse<RetrievePermissionRow>>(context)
    {
        public void Validate() => ValidatePermissions();
    }

    private static TestHandler Handler(IRequestContext? context = null, RetrieveRequest? request = null,
        IDbConnection? connection = null, SqlQuery? query = null)
    {
        var handler = new TestHandler(context ?? Context());
        handler.Init(connection ?? new MockDbConnection(), query ?? new SqlQuery(),
            request ?? new RetrieveRequest { EntityId = 1 }, NewFields());
        return handler;
    }

    private static RetrieveBaseRow.RowFields NewFields()
    {
        var fields = new RetrieveBaseRow.RowFields();
        fields.Initialize(annotations: null, dialect: SqlSettings.DefaultDialect);
        foreach (var field in new Field[] { fields.Id, fields.Name, fields.External })
        {
            field.Flags = FieldFlags.None;
            field.MinSelectLevel = SelectLevel.Auto;
            field.ReadPermission = null;
            field.IsLookup = false;
        }
        return fields;
    }

    [Fact]
    public void Constructor_NullContext_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new TestHandler(null!));
    }

    [Fact]
    public void Uninitialized_ConnectionAndQuery_Throw()
    {
        var handler = new TestHandler(Context());

        Assert.Throws<InvalidOperationException>(() => handler.Connection);
        Assert.Throws<InvalidOperationException>(() => handler.Query);
    }

    [Fact]
    public void AllowSelectField_RespectsNeverAndPermissions()
    {
        var handler = Handler(Context(_ => true));
        var fields = handler.Row.GetFields();

        Assert.True(handler.Allow(fields.Id));

        fields.Id.MinSelectLevel = SelectLevel.Never;
        Assert.False(handler.Allow(fields.Id));

        fields.Id.MinSelectLevel = SelectLevel.Auto;
        fields.Id.ReadPermission = "Some.Permission";
        Assert.False(Handler(Context(_ => false)).Allow(fields.Id));
        Assert.True(handler.Allow(fields.Id));
    }

    [Fact]
    public void ShouldSelectField_Auto_NotMapped_BecomesDetails()
    {
        var handler = Handler(request: new RetrieveRequest { ColumnSelection = RetrieveColumnSelection.Details });
        var fields = handler.Row.GetFields();
        fields.Id.Flags = FieldFlags.NotMapped;

        Assert.True(handler.Should(fields.Id));
    }

    [Fact]
    public void ShouldSelectField_Auto_Lookup_BecomesLookup()
    {
        var handler = Handler(request: new RetrieveRequest { ColumnSelection = RetrieveColumnSelection.Lookup });
        var fields = handler.Row.GetFields();
        fields.Id.IsLookup = true;

        Assert.True(handler.Should(fields.Id));
    }

    [Fact]
    public void ShouldSelectField_Auto_Foreign_BecomesDetails()
    {
        var handler = Handler(request: new RetrieveRequest { ColumnSelection = RetrieveColumnSelection.List });
        var fields = handler.Row.GetFields();
        fields.Name.Flags = FieldFlags.Foreign;

        Assert.False(handler.Should(fields.Name));

        fields.Name.Flags = FieldFlags.None;
        Assert.True(handler.Should(fields.Name));
    }

    [Fact]
    public void ShouldSelectField_AlwaysAndNever()
    {
        var handler = Handler();
        var fields = handler.Row.GetFields();

        fields.Id.MinSelectLevel = SelectLevel.Always;
        Assert.True(handler.Should(fields.Id));

        fields.Id.MinSelectLevel = SelectLevel.Never;
        Assert.False(handler.Should(fields.Id));
    }

    [Fact]
    public void ShouldSelectField_ExcludeColumns_ByNameAndProperty()
    {
        var handler = Handler();
        var fields = handler.Row.GetFields();

        handler.Request.ExcludeColumns = ["external_name"];
        Assert.False(handler.Should(fields.External));

        handler.Request.ExcludeColumns = ["External"];
        Assert.False(handler.Should(fields.External));
    }

    [Fact]
    public void ShouldSelectField_IncludeColumns_ByNameAndProperty()
    {
        var handler = Handler(request: new RetrieveRequest { ColumnSelection = RetrieveColumnSelection.List });
        var fields = handler.Row.GetFields();
        fields.External.MinSelectLevel = SelectLevel.Details;

        Assert.False(handler.Should(fields.External));

        handler.Request.IncludeColumns = ["external_name"];
        Assert.True(handler.Should(fields.External));

        handler.Request.IncludeColumns = ["External"];
        Assert.True(handler.Should(fields.External));
    }

    [Theory]
    [InlineData(RetrieveColumnSelection.List, true)]
    [InlineData(RetrieveColumnSelection.KeyOnly, true)]
    [InlineData(RetrieveColumnSelection.Details, true)]
    [InlineData(RetrieveColumnSelection.None, false)]
    [InlineData(RetrieveColumnSelection.IdOnly, true)]
    [InlineData(RetrieveColumnSelection.Lookup, false)]
    public void ShouldSelectField_ColumnSelection(RetrieveColumnSelection selection, bool expected)
    {
        var handler = Handler(request: new RetrieveRequest { ColumnSelection = selection });
        var fields = handler.Row.GetFields();

        Assert.Equal(expected, handler.Should(fields.Id));
    }

    [Fact]
    public void ShouldSelectField_DefaultSelection_WhenUnknown()
    {
        var handler = Handler(request: new RetrieveRequest { ColumnSelection = (RetrieveColumnSelection)99 });
        var fields = handler.Row.GetFields();

        Assert.False(handler.Should(fields.Id));
    }

    [Fact]
    public void IsIncluded_ByFieldAndColumn()
    {
        var handler = Handler();
        var fields = handler.Row.GetFields();

        Assert.False(handler.Included(fields.Id));
        Assert.False(handler.Included("Id"));

        handler.Request.IncludeColumns = ["Id"];
        Assert.True(handler.Included(fields.Id));
        Assert.True(handler.Included("Id"));
    }

    [Fact]
    public void IsIncluded_PropertyName()
    {
        var handler = Handler();
        var fields = handler.Row.GetFields();
        handler.Request.IncludeColumns = ["External"];

        Assert.True(handler.Included(fields.External));
    }

    [Fact]
    public void SelectField_SelectsColumn()
    {
        var handler = Handler();
        var query = new SqlQuery();
        var fields = handler.Row.GetFields();

        handler.Select(query, fields.Id);

        Assert.Single(((ISqlQueryExtensible)query).Columns);
    }

    [Fact]
    public void SelectFields_SkipsNotMappedAndExcluded()
    {
        var handler = Handler(request: new RetrieveRequest { ColumnSelection = RetrieveColumnSelection.Details });
        var query = new SqlQuery();
        var fields = handler.Row.GetFields();
        fields.Name.Flags = FieldFlags.NotMapped;
        fields.External.Flags = FieldFlags.NotMapped;

        handler.SelectAll(query);

        Assert.Single(((ISqlQueryExtensible)query).Columns);
    }

    [Fact]
    public void ValidatePermissions_NoAttribute_DoesNotThrow()
    {
        var handler = Handler();
        handler.Validate();
    }

    [Fact]
    public void ValidatePermissions_WithAttribute_Validates()
    {
        new PermissionHandler(Context(_ => true)).Validate();
        Assert.Throws<ValidationError>(() => new PermissionHandler(Context(_ => false)).Validate());
    }

    [Fact]
    public void CreateQuery_BuildsWhereOnId()
    {
        var connection = new MockDbConnection();
        var handler = Handler(connection: connection, request: new RetrieveRequest { EntityId = 42 });

        var query = handler.Build();

        Assert.Contains("RetrieveBase", query.ToString());
    }

    [Fact]
    public void Cache_ComesFromContext()
    {
        var context = Context();
        var handler = Handler(context);
        Assert.Same(context.Cache, handler.CacheValue);
    }

    [Fact]
    public void ExplicitInterface_Members_Forward()
    {
        var handler = Handler();
        IRetrieveRequestHandler iface = handler;

        Assert.Same(handler.Row, iface.Row);
        Assert.Same(handler.Request, iface.Request);
        Assert.Same(handler.Response, iface.Response);
        Assert.True(iface.AllowSelectField(NewFields().Id));
        Assert.True(iface.ShouldSelectField(NewFields().Id));
    }
}
