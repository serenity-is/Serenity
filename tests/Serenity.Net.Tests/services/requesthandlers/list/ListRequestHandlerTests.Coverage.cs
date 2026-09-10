namespace Serenity.Services;

public partial class ListRequestHandlerTests
{
    [ReadPermission(ReadPermission)]
    private class CovRow : Row<CovRow.RowFields>, IIdRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [NameProperty, QuickSearch]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        [QuickSearch(SearchType.Contains, isExplicit: true)]
        public string? ExplicitQ { get => fields.ExplicitQ[this]; set => fields.ExplicitQ[this] = value; }

        [QuickSearch(SearchType.Equals, 1)]
        public int? IntQ { get => fields.IntQ[this]; set => fields.IntQ[this] = value; }

        [QuickSearch(SearchType.Equals, 1)]
        public short? ShortQ { get => fields.ShortQ[this]; set => fields.ShortQ[this] = value; }

        [QuickSearch(SearchType.Equals, 1)]
        public long? LongQ { get => fields.LongQ[this]; set => fields.LongQ[this] = value; }

        [QuickSearch(SearchType.StartsWith)]
        public string? StartsQ { get => fields.StartsQ[this]; set => fields.StartsQ[this] = value; }

        [QuickSearch(SearchType.FullTextContains)]
        public string? FullTextQ { get => fields.FullTextQ[this]; set => fields.FullTextQ[this] = value; }

        [SortOrder(2)]
        public string? Sorted { get => fields.Sorted[this]; set => fields.Sorted[this] = value; }

        [Sortable(false)]
        public string? Unsortable { get => fields.Unsortable[this]; set => fields.Unsortable[this] = value; }

        [NotMapped]
        public string? NotMappedF { get => fields.NotMappedF[this]; set => fields.NotMappedF[this] = value; }

        [SetFieldFlags(FieldFlags.DenyFiltering)]
        public string? DenyFilterF { get => fields.DenyFilterF[this]; set => fields.DenyFilterF[this] = value; }

        [MinSelectLevel(SelectLevel.Never)]
        public string? NeverF { get => fields.NeverF[this]; set => fields.NeverF[this] = value; }

        [MinSelectLevel(SelectLevel.Always)]
        public string? AlwaysF { get => fields.AlwaysF[this]; set => fields.AlwaysF[this] = value; }

        [MinSelectLevel(SelectLevel.Explicit)]
        public string? ExplicitF { get => fields.ExplicitF[this]; set => fields.ExplicitF[this] = value; }

        [MinSelectLevel(SelectLevel.Details)]
        public string? DetailsF { get => fields.DetailsF[this]; set => fields.DetailsF[this] = value; }

        [SetFieldFlags(FieldFlags.Foreign)]
        public string? ForeignF { get => fields.ForeignF[this]; set => fields.ForeignF[this] = value; }

        public string? NormalF { get => fields.NormalF[this]; set => fields.NormalF[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public StringField Name;
            public StringField ExplicitQ;
            public Int32Field IntQ;
            public Int16Field ShortQ;
            public Int64Field LongQ;
            public StringField StartsQ;
            public StringField FullTextQ;
            public StringField Sorted;
            public StringField Unsortable;
            public StringField NotMappedF;
            public StringField DenyFilterF;
            public StringField NeverF;
            public StringField AlwaysF;
            public StringField ExplicitF;
            public StringField DetailsF;
            public StringField ForeignF;
            public StringField NormalF;
#pragma warning restore CS0649
        }
    }

    private class NoQuickSearchRow : Row<NoQuickSearchRow.RowFields>, IIdRow, INameRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [NameProperty]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public StringField Name;
#pragma warning restore CS0649
        }
    }

    private class CovListHandler : ListRequestHandler<CovRow>
    {
        public CovListHandler(IRequestContext context) : base(context) { }

        public void Setup(IDbConnection connection, ListRequest? request = null)
        {
            Connection = connection;
            Row = new CovRow();
            Query = new SqlQuery().Dialect(connection.GetDialect());
            Request = request ?? new ListRequest();
        }

        public void SetLookupAccessMode(bool value) => lookupAccessMode = value;

        public bool CallAllowSelect(Field field) => AllowSelectField(field);

        public bool CallShouldSelect(Field field) => ShouldSelectField(field);

        public bool CallIsIncluded(Field field) => IsIncluded(field);

        public bool CallIsIncludedByColumn(string column) => IsIncluded(column);

        public IEnumerable<Field> CallGetQuickSearchFields(string? containsField) =>
            GetQuickSearchFields(containsField);

        public SortBy[]? CallGetNativeSort() => GetNativeSort();

        public Field[]? CallGetDistinctFields() => GetDistinctFields();

        public void CallIgnoreEqualityFilter(string field) => IgnoreEqualityFilter(field);

        public BaseCriteria CallAddFieldContains(Field field, string text, long? id,
            SearchType searchType, bool numericOnly, out bool orFalse)
        {
            BaseCriteria criteria = Criteria.Empty;
            bool result = false;
            AddFieldContainsCriteria(field, text, id, searchType, numericOnly,
                ref criteria, ref result);
            orFalse = result;
            return criteria;
        }

        public void CallApplyFieldContains(Field field, string text, long? id,
            ref BaseCriteria criteria, ref bool orFalse) =>
            ApplyFieldContainsText(field, text, id, ref criteria, ref orFalse);

        public void CallApplyFieldEquality(SqlQuery query, Field field, object? value) =>
            ApplyFieldEqualityFilter(query, field, value);

        public void CallApplySortBy(SqlQuery query, SortBy sortBy) => ApplySortBy(query, sortBy);

        public void CallApplyEquality(SqlQuery query) => ApplyEqualityFilter(query);
    }

    private class SkippingListHandler : ListRequestHandler<NoQuickSearchRow>
    {
        public SkippingListHandler(IRequestContext context) : base(context) { }

        protected override NoQuickSearchRow ProcessEntity(NoQuickSearchRow row) => null!;
    }

    private sealed class SyncExceptionBehavior : IListBehaviorSync, IListExceptionBehavior
    {
        public bool ExceptionCalled;

        public void OnException(IListRequestHandler handler, Exception exception)
        {
            ExceptionCalled = true;
        }
    }

    private static IRequestContext CovContext() =>
        new NullRequestContext().WithPermissions(_ => true);

    private static MockDbConnection CovConnection() =>
        new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());

    [Fact]
    public void ShouldSelect_Respects_ColumnSelection_KeyOnly()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn, new ListRequest { ColumnSelection = ColumnSelection.KeyOnly });

        Assert.True(handler.CallShouldSelect(CovRow.Fields.Id));
        Assert.False(handler.CallShouldSelect(CovRow.Fields.NormalF));
    }

    [Fact]
    public void ShouldSelect_Respects_ColumnSelection_IdOnly()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn, new ListRequest { ColumnSelection = ColumnSelection.IdOnly });

        Assert.True(handler.CallShouldSelect(CovRow.Fields.Id));
        Assert.False(handler.CallShouldSelect(CovRow.Fields.NormalF));
    }

    [Fact]
    public void ShouldSelect_Respects_ColumnSelection_None()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn, new ListRequest { ColumnSelection = ColumnSelection.None });

        Assert.False(handler.CallShouldSelect(CovRow.Fields.Id));
        Assert.False(handler.CallShouldSelect(CovRow.Fields.NormalF));
    }

    [Fact]
    public void ShouldSelect_Respects_ColumnSelection_Lookup()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn, new ListRequest { ColumnSelection = ColumnSelection.Lookup });

        Assert.True(handler.CallShouldSelect(CovRow.Fields.Name));
        Assert.False(handler.CallShouldSelect(CovRow.Fields.NormalF));
    }

    [Fact]
    public void ShouldSelect_Respects_ColumnSelection_Details()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn, new ListRequest { ColumnSelection = ColumnSelection.Details });

        Assert.True(handler.CallShouldSelect(CovRow.Fields.NormalF));
        Assert.True(handler.CallShouldSelect(CovRow.Fields.DetailsF));
    }

    [Fact]
    public void ShouldSelect_Always_And_Never_Levels()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn, new ListRequest { ColumnSelection = ColumnSelection.None });

        Assert.True(handler.CallShouldSelect(CovRow.Fields.AlwaysF));
        Assert.False(handler.CallShouldSelect(CovRow.Fields.NeverF));
    }

    [Fact]
    public void ShouldSelect_Exclude_And_Include_Columns()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn, new ListRequest
        {
            ColumnSelection = ColumnSelection.None,
            IncludeColumns = [CovRow.Fields.NormalF.Name],
            ExcludeColumns = [CovRow.Fields.AlwaysF.Name]
        });

        Assert.True(handler.CallShouldSelect(CovRow.Fields.NormalF));
        Assert.True(handler.CallShouldSelect(CovRow.Fields.AlwaysF));
    }

    [Fact]
    public void ShouldSelect_NotMapped_Auto_Is_Explicit()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn, new ListRequest());

        Assert.False(handler.CallShouldSelect(CovRow.Fields.NotMappedF));

        handler.Setup(conn, new ListRequest { IncludeColumns = [CovRow.Fields.NotMappedF.Name] });
        Assert.True(handler.CallShouldSelect(CovRow.Fields.NotMappedF));
    }

    [Fact]
    public void ShouldSelect_Foreign_Auto_Is_Details()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn, new ListRequest { ColumnSelection = ColumnSelection.Details });
        Assert.True(handler.CallShouldSelect(CovRow.Fields.ForeignF));

        handler.Setup(conn, new ListRequest { ColumnSelection = ColumnSelection.List });
        Assert.False(handler.CallShouldSelect(CovRow.Fields.ForeignF));
    }

    [Fact]
    public void AllowSelect_Returns_False_For_Never()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn);

        Assert.False(handler.CallAllowSelect(CovRow.Fields.NeverF));
        Assert.True(handler.CallAllowSelect(CovRow.Fields.NormalF));
    }

    [Fact]
    public void AllowSelect_Uses_LookupAccessMode()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn);
        handler.SetLookupAccessMode(true);

        Assert.False(handler.CallAllowSelect(CovRow.Fields.NormalF));
        Assert.True(handler.CallAllowSelect(CovRow.Fields.Name));
    }

    [Fact]
    public void IsIncluded_Checks_IncludeColumns()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn, new ListRequest { IncludeColumns = [CovRow.Fields.NormalF.Name] });

        Assert.True(handler.CallIsIncluded(CovRow.Fields.NormalF));
        Assert.True(handler.CallIsIncludedByColumn(CovRow.Fields.NormalF.Name));
        Assert.False(handler.CallIsIncluded(CovRow.Fields.Name));
    }

    [Fact]
    public void GetNativeSort_Uses_SortOrders()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn);

        var sort = handler.CallGetNativeSort();
        Assert.NotNull(sort);
        Assert.Contains(sort, x => x.Field == CovRow.Fields.Sorted.Name);
    }

    [Fact]
    public void GetNativeSort_Falls_Back_To_NameField()
    {
        using var conn = CovConnection();
        var handler = new ListRequestHandler<NoQuickSearchRow>(CovContext());
        handler.List(conn, new ListRequest());
        Assert.NotNull(handler);
    }

    [Fact]
    public void GetQuickSearchFields_Returns_NonExplicit_QuickSearch_Fields()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn);

        var fields = handler.CallGetQuickSearchFields(null).ToList();
        Assert.Contains(fields, x => x.Name == CovRow.Fields.Name.Name);
        Assert.DoesNotContain(fields, x => x.Name == CovRow.Fields.ExplicitQ.Name);
    }

    [Fact]
    public void GetQuickSearchFields_Uses_ContainsField()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn);

        var fields = handler.CallGetQuickSearchFields(CovRow.Fields.NormalF.Name).ToList();
        Assert.Equal(CovRow.Fields.NormalF.Name, Assert.Single(fields).Name);
    }

    [Fact]
    public void GetQuickSearchFields_Throws_For_Invalid_ContainsField()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn);

        Assert.Throws<ArgumentOutOfRangeException>(() => handler.CallGetQuickSearchFields("NoSuchField"));
        Assert.Throws<ArgumentOutOfRangeException>(() => handler.CallGetQuickSearchFields(CovRow.Fields.NeverF.Name));
    }

    [Fact]
    public void AddFieldContains_NumericOnly_With_Null_Id_Sets_OrFalse()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn);

        handler.CallAddFieldContains(CovRow.Fields.IntQ, "abc", null,
            SearchType.Equals, true, out var orFalse);

        Assert.True(orFalse);
    }

    [Fact]
    public void AddFieldContains_Int32_Out_Of_Range_Sets_OrFalse()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn);

        var criteria = handler.CallAddFieldContains(CovRow.Fields.IntQ, "99999999999",
            long.MaxValue, SearchType.Equals, false, out var orFalse);

        Assert.True(orFalse);
        Assert.True(criteria.IsEmpty);
    }

    [Fact]
    public void AddFieldContains_Int32_Valid_Adds_Criteria()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn);

        var criteria = handler.CallAddFieldContains(CovRow.Fields.IntQ, "5", 5,
            SearchType.Equals, false, out var orFalse);

        Assert.False(orFalse);
        Assert.False(criteria.IsEmpty);
    }

    [Fact]
    public void AddFieldContains_Int16_And_Int64_Branches()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn);

        var c16 = handler.CallAddFieldContains(CovRow.Fields.ShortQ, "5", 5,
            SearchType.Equals, false, out _);
        Assert.False(c16.IsEmpty);

        handler.CallAddFieldContains(CovRow.Fields.ShortQ, "5", long.MaxValue,
            SearchType.Equals, false, out var orFalse16);
        Assert.True(orFalse16);

        var c64 = handler.CallAddFieldContains(CovRow.Fields.LongQ, "5", 5,
            SearchType.Equals, false, out _);
        Assert.False(c64.IsEmpty);

        handler.CallAddFieldContains(CovRow.Fields.LongQ, "abc", null,
            SearchType.Equals, false, out var orFalse64);
        Assert.True(orFalse64);
    }

    [Fact]
    public void AddFieldContains_String_Equals_Branch()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn);

        var criteria = handler.CallAddFieldContains(CovRow.Fields.Name, "x", null,
            SearchType.Equals, false, out _);
        Assert.False(criteria.IsEmpty);
    }

    [Fact]
    public void AddFieldContains_Other_SearchTypes()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn);

        Assert.False(handler.CallAddFieldContains(CovRow.Fields.Name, "x", null,
            SearchType.Contains, false, out _).IsEmpty);
        Assert.False(handler.CallAddFieldContains(CovRow.Fields.Name, "x", null,
            SearchType.StartsWith, false, out _).IsEmpty);
        Assert.False(handler.CallAddFieldContains(CovRow.Fields.Name, "x", null,
            SearchType.FullTextContains, false, out _).IsEmpty);
    }

    [Fact]
    public void AddFieldContains_Throws_For_Invalid_SearchType()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn);

        Assert.Throws<ArgumentOutOfRangeException>(() => handler.CallAddFieldContains(
            CovRow.Fields.Name, "x", null, (SearchType)999, false, out _));
    }

    [Fact]
    public void ApplyFieldContains_Auto_Detects_Int_As_Equals()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn);

        BaseCriteria criteria = Criteria.Empty;
        bool orFalse = false;
        handler.CallApplyFieldContains(CovRow.Fields.IntQ, "5", 5, ref criteria, ref orFalse);

        Assert.False(orFalse);
        Assert.False(criteria.IsEmpty);
    }

    [Fact]
    public void ApplyEqualityFilter_Applies_Value()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn, new ListRequest
        {
            EqualityFilter = new Dictionary<string, object?> { [CovRow.Fields.NormalF.Name] = "x" }
        });

        var query = new SqlQuery().Dialect(conn.GetDialect()).From(new CovRow());
        handler.CallApplyEquality(query);
    }

    [Fact]
    public void ApplyEqualityFilter_Skips_Empty_And_Ignored()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn, new ListRequest
        {
            EqualityFilter = new Dictionary<string, object?>
            {
                [CovRow.Fields.NormalF.Name] = "",
                [CovRow.Fields.Name.Name] = "x"
            }
        });
        handler.CallIgnoreEqualityFilter(CovRow.Fields.Name.Name);

        var query = new SqlQuery().Dialect(conn.GetDialect()).From(new CovRow());
        handler.CallApplyEquality(query);
    }

    [Fact]
    public void ApplyEqualityFilter_Throws_For_Missing_Field()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn, new ListRequest
        {
            EqualityFilter = new Dictionary<string, object?> { ["NoSuchField"] = "x" }
        });

        var query = new SqlQuery().Dialect(conn.GetDialect()).From(new CovRow());
        Assert.Throws<ArgumentOutOfRangeException>(() => handler.CallApplyEquality(query));
    }

    [Fact]
    public void ApplyFieldEquality_Throws_For_DenyFiltering_And_NotMapped()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn);

        var query = new SqlQuery().Dialect(conn.GetDialect()).From(new CovRow());
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            handler.CallApplyFieldEquality(query, CovRow.Fields.DenyFilterF, "x"));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            handler.CallApplyFieldEquality(query, CovRow.Fields.NotMappedF, "x"));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            handler.CallApplyFieldEquality(query, CovRow.Fields.NeverF, "x"));
    }

    [Fact]
    public void ApplyFieldEquality_Applies_Enumerable_And_Null()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn);

        var query = new SqlQuery().Dialect(conn.GetDialect()).From(new CovRow());
        handler.CallApplyFieldEquality(query, CovRow.Fields.IntQ, new List<object> { 1, 2 });
        handler.CallApplyFieldEquality(query, CovRow.Fields.Name, null);
    }

    [Fact]
    public void ApplySortBy_Skips_NotMapped_And_Unsortable()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn);

        var query = new SqlQuery().Dialect(conn.GetDialect()).From(new CovRow());
        handler.CallApplySortBy(query, new SortBy(CovRow.Fields.NotMappedF.Name, false));
        handler.CallApplySortBy(query, new SortBy(CovRow.Fields.Unsortable.Name, false));
        handler.CallApplySortBy(query, new SortBy(CovRow.Fields.NormalF.Name, false));
    }

    [Fact]
    public void GetDistinctFields_Returns_Valid_Fields()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn, new ListRequest
        {
            DistinctFields = [new SortBy(CovRow.Fields.NormalF.Name, false)]
        });

        var fields = handler.CallGetDistinctFields();
        Assert.NotNull(fields);
        Assert.Equal(CovRow.Fields.NormalF.Name, Assert.Single(fields).Name);
    }

    [Fact]
    public void GetDistinctFields_Returns_Empty_For_Invalid()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn, new ListRequest
        {
            DistinctFields = [new SortBy("NoSuchField", false)]
        });

        var fields = handler.CallGetDistinctFields();
        Assert.Empty(fields!);
    }

    [Fact]
    public void GetDistinctFields_Returns_Null_When_Not_Requested()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());
        handler.Setup(conn, new ListRequest());

        Assert.Null(handler.CallGetDistinctFields());
    }

    [Fact]
    public void ContainsText_Applies_Query()
    {
        using var conn = CovConnection();
        var handler = new CovListHandler(CovContext());

        handler.List(conn, new ListRequest { ContainsText = "abc" });
    }

    [Fact]
    public void Process_Skips_Entity_When_ProcessEntity_Returns_Null()
    {
        using var conn = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 1, Name = "x" }));
        var handler = new SkippingListHandler(CovContext());

        var response = handler.List(conn, new ListRequest());

        Assert.NotNull(response);
        Assert.Empty(response.Entities);
    }

    [Fact]
    public void Process_With_Invalid_Distinct_Fields_Marks_Values_Null()
    {
        using var conn = CovConnection();
        var handler = new ListRequestHandler<CovRow>(CovContext());

        var response = handler.List(conn, new ListRequest
        {
            DistinctFields = [new SortBy("NoSuchField", false)]
        });

        Assert.NotNull(response);
        Assert.Null(response.Values);
    }

    [Fact]
    public void Process_With_Valid_Distinct_Fields_Populates_Values()
    {
        using var conn = CovConnection();
        var handler = new ListRequestHandler<CovRow>(CovContext());

        var response = handler.List(conn, new ListRequest
        {
            DistinctFields = [new SortBy(CovRow.Fields.NormalF.Name, false)]
        });

        Assert.NotNull(response.Values);
    }

    [Fact]
    public void Process_With_Criteria_And_IncludeDeleted()
    {
        using var conn = CovConnection();
        var handler = new ListRequestHandler<CovRow>(CovContext());

        handler.List(conn, new ListRequest
        {
            Criteria = new Criteria(CovRow.Fields.NormalF.Name) == "x",
            IncludeDeleted = true
        });
    }

    [Fact]
    public void Process_Invokes_Exception_Behavior_On_Error()
    {
        var behavior = new SyncExceptionBehavior();
        var behaviors = new MockBehaviorProvider((_, _, _) => new object[] { behavior });
        using var conn = new MockDbConnection()
            .InterceptExecuteReader(_ => throw new InvalidOperationException("boom"));
        var handler = new ListRequestHandler<CovRow>(
            new NullRequestContext(behaviors).WithPermissions(_ => true));

        Assert.Throws<InvalidOperationException>(() => handler.List(conn, new ListRequest()));
        Assert.True(behavior.ExceptionCalled);
    }

    [Fact]
    public void ListRequestHandler_WithRequestType_Works()
    {
        using var conn = CovConnection();
        var handler = new ListRequestHandler<CovRow, ListRequest>(CovContext());

        Assert.NotNull(handler.List(conn, new ListRequest()));
    }
}
