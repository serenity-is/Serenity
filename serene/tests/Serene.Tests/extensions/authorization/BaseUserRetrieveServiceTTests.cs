namespace Serenity.Extensions;

public class BaseUserRetrieveServiceTTests
{
    [ConnectionKey("Default")]
    private class IntUserRow : Row<IntUserRow.RowFields>, IIdRow, INameRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [NameProperty]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public StringField Name = null!;
        }
    }

    [ConnectionKey("Default")]
    private class Int64UserRow : Row<Int64UserRow.RowFields>, IIdRow, INameRow
    {
        [IdProperty]
        public long? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [NameProperty]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int64Field Id = null!;
            public StringField Name = null!;
        }
    }

    [ConnectionKey("Default")]
    private class GuidUserRow : Row<GuidUserRow.RowFields>, IIdRow, INameRow
    {
        [IdProperty]
        public Guid? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [NameProperty]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public GuidField Id = null!;
            public StringField Name = null!;
        }
    }

    [ConnectionKey("Default")]
    private class StringUserRow : Row<StringUserRow.RowFields>, IIdRow, INameRow
    {
        [IdProperty]
        public string? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [NameProperty]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public StringField Id = null!;
            public StringField Name = null!;
        }
    }

    private class TestService<TRow>(
        ITwoLevelCache cache,
        ISqlConnections sqlConnections,
        Func<TRow, IUserDefinition?> toDefinition)
        : BaseUserRetrieveService<TRow>(cache, sqlConnections)
        where TRow : class, IRow, IIdRow, INameRow, new()
    {
        public bool IsValidUserIdExposed(string? id) => IsValidUserId(id);
        public IUserDefinition? LoadByIdExposed(string id) => LoadById(id);
        public IUserDefinition? LoadByUsernameExposed(string username) => LoadByUsername(username);
        public IUserDefinition? LoadByCriteriaExposed(IDbConnection connection, BaseCriteria criteria) =>
            LoadByCriteria(connection, criteria);
        public string CacheGroupKeyExposed() => GetCacheGroupKey();

        protected override IUserDefinition? ToUserDefinition(TRow row) => toDefinition(row);
    }

    [Fact]
    public void Constructor_Throws_For_Null_SqlConnections()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TestService<IntUserRow>(new TestTwoLevelCache(), null!, _ => null));
    }

    [Fact]
    public void IsValidUserId_Validates_Int_Ids()
    {
        var service = new TestService<IntUserRow>(new TestTwoLevelCache(),
            new NullSqlConnections(), _ => null);

        Assert.True(service.IsValidUserIdExposed("123"));
        Assert.False(service.IsValidUserIdExposed("abc"));
        Assert.False(service.IsValidUserIdExposed(""));
        Assert.False(service.IsValidUserIdExposed(null));
    }

    [Fact]
    public void IsValidUserId_Validates_Int64_Ids()
    {
        var service = new TestService<Int64UserRow>(new TestTwoLevelCache(),
            new NullSqlConnections(), _ => null);

        Assert.True(service.IsValidUserIdExposed("123456789012"));
        Assert.False(service.IsValidUserIdExposed("abc"));
        Assert.False(service.IsValidUserIdExposed(""));
    }

    [Fact]
    public void IsValidUserId_Validates_Guid_Ids()
    {
        var service = new TestService<GuidUserRow>(new TestTwoLevelCache(),
            new NullSqlConnections(), _ => null);

        Assert.True(service.IsValidUserIdExposed(Guid.NewGuid().ToString()));
        Assert.False(service.IsValidUserIdExposed("not-a-guid"));
        Assert.False(service.IsValidUserIdExposed(""));
    }

    [Fact]
    public void IsValidUserId_Accepts_Other_Field_Types()
    {
        var service = new TestService<StringUserRow>(new TestTwoLevelCache(),
            new NullSqlConnections(), _ => null);

        Assert.True(service.IsValidUserIdExposed("abc"));
        Assert.False(service.IsValidUserIdExposed(""));
    }

    [Fact]
    public void LoadById_Uses_Criteria_And_Converts_Row()
    {
        var row = new IntUserRow { Id = 5, Name = "user5" };
        using var connection = new MockDbConnection()
            .InterceptFindRow(_ => new OptionalValue<IRow>(row));

        var service = new TestService<IntUserRow>(new TestTwoLevelCache(),
            new MockSqlConnections { OnNewByKey = _ => connection },
            r => new MockUserDefinition(r.Id!.Value.ToString(), r.Name!));

        var result = service.LoadByIdExposed("5");
        Assert.NotNull(result);
        Assert.Equal("user5", result.Username);
    }

    [Fact]
    public void LoadById_Returns_Null_When_Not_Found()
    {
        using var connection = new MockDbConnection()
            .InterceptFindRow(_ => new OptionalValue<IRow>(null!));

        var service = new TestService<IntUserRow>(new TestTwoLevelCache(),
            new MockSqlConnections { OnNewByKey = _ => connection },
            r => new MockUserDefinition(r.Id!.Value.ToString(), r.Name!));

        Assert.Null(service.LoadByIdExposed("5"));
    }

    [Fact]
    public void LoadByUsername_Uses_Name_Criteria()
    {
        var row = new GuidUserRow { Id = Guid.NewGuid(), Name = "user5" };
        using var connection = new MockDbConnection()
            .InterceptFindRow(_ => new OptionalValue<IRow>(row));

        var service = new TestService<GuidUserRow>(new TestTwoLevelCache(),
            new MockSqlConnections { OnNewByKey = _ => connection },
            r => new MockUserDefinition(r.Id!.Value.ToString(), r.Name!));

        var result = service.LoadByUsernameExposed("user5");
        Assert.NotNull(result);
        Assert.Equal("user5", result.Username);
    }

    [Fact]
    public void LoadByCriteria_Returns_Null_When_No_Row()
    {
        using var connection = new MockDbConnection().InterceptFindRow(_ => new OptionalValue<IRow>(null!));
        var service = new TestService<IntUserRow>(new TestTwoLevelCache(),
            new NullSqlConnections(), _ => null);

        Assert.Null(service.LoadByCriteriaExposed(connection, new Criteria("Id") == 1));
    }

    [Fact]
    public void CacheGroupKey_Is_Cached_And_Expected()
    {
        var service = new TestService<IntUserRow>(new TestTwoLevelCache(),
            new NullSqlConnections(), _ => null);

        Assert.Equal(IntUserRow.Fields.GenerationKey, service.CacheGroupKeyExposed());
        Assert.Same(service.CacheGroupKeyExposed(), service.CacheGroupKeyExposed());
    }
}

