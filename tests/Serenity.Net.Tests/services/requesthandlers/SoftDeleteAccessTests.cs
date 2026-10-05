using Serenity.TestUtils;

namespace Serenity.Services;

public class SoftDeleteAccessTests
{
    private const string ReadPermission = "Test:Read";
    private const string DeletePermission = "Test:Delete";
    private const string UndeletePermission = "Test:Undelete";

    [ReadPermission(ReadPermission)]
    [DeletePermission(DeletePermission)]
    private class SoftDeleteRow : Row<SoftDeleteRow.RowFields>, IIdRow, IIsActiveDeletedRow
    {
        [IdProperty, Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public short? IsActive { get => fields.IsActive[this]; set => fields.IsActive[this] = value; }

        public Int16Field IsActiveField => fields.IsActive;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public Int16Field IsActive = null!;
        }
    }

    [ReadPermission(ReadPermission)]
    [DeletePermission(DeletePermission)]
    [UndeletePermission(UndeletePermission)]
    private class UndeletePermRow : Row<UndeletePermRow.RowFields>, IIdRow, IIsActiveDeletedRow
    {
        [IdProperty, Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public short? IsActive { get => fields.IsActive[this]; set => fields.IsActive[this] = value; }

        public Int16Field IsActiveField => fields.IsActive;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public Int16Field IsActive = null!;
        }
    }

    private static NullRequestContext WithRead(params string[] permissions) =>
        new NullRequestContext().WithPermissions(p =>
            p == ReadPermission || permissions.Contains(p));

    private static string ListWhereClause<TRow>(NullRequestContext context,
        bool includeDeleted) where TRow : class, IRow, new()
    {
        string? where = null;
        using var connection = new MockDbConnection().InterceptExecuteReader(args =>
        {
            where = ((IFilterableQuery)args.Query!).GetWhereClause();
            return new MockDbDataReader();
        });

        new ListRequestHandler<TRow>(context)
            .List(connection, new ListRequest { IncludeDeleted = includeDeleted });

        Assert.NotNull(where);
        return where!;
    }

    private static string RetrieveWhereClause<TRow>(NullRequestContext context)
        where TRow : class, IRow, new()
    {
        string? where = null;
        using var connection = new MockDbConnection().InterceptExecuteReader(args =>
        {
            where = ((IFilterableQuery)args.Query!).GetWhereClause();
            return args.ToMockReader(new { Id = 5, IsActive = (short)-1 });
        });

        new RetrieveRequestHandler<TRow>(context)
            .Retrieve(connection, new RetrieveRequest { EntityId = 5 });

        Assert.NotNull(where);
        return where!;
    }

    [Fact]
    public void List_Ignores_IncludeDeleted_When_User_Cannot_Undelete()
    {
        var where = ListWhereClause<SoftDeleteRow>(WithRead(), includeDeleted: true);

        Assert.Contains("IsActive", where);
    }

    [Fact]
    public void List_Honors_IncludeDeleted_When_User_Can_Undelete()
    {
        var where = ListWhereClause<SoftDeleteRow>(WithRead(DeletePermission), includeDeleted: true);

        Assert.DoesNotContain("IsActive", where);
    }

    [Fact]
    public void List_Excludes_Deleted_By_Default()
    {
        var where = ListWhereClause<SoftDeleteRow>(WithRead(DeletePermission), includeDeleted: false);

        Assert.Contains("IsActive", where);
    }

    [Fact]
    public void List_Prefers_UndeletePermission_Attribute_Over_DeletePermission()
    {
        var where = ListWhereClause<UndeletePermRow>(WithRead(DeletePermission), includeDeleted: true);

        Assert.Contains("IsActive", where);
    }

    [Fact]
    public void List_Uses_UndeletePermission_Attribute_When_Present_And_Granted()
    {
        var where = ListWhereClause<UndeletePermRow>(WithRead(UndeletePermission), includeDeleted: true);

        Assert.DoesNotContain("IsActive", where);
    }

    [Fact]
    public void Retrieve_Hides_Deleted_When_User_Cannot_Undelete()
    {
        var where = RetrieveWhereClause<SoftDeleteRow>(WithRead());

        Assert.Contains("IsActive", where);
    }

    [Fact]
    public void Retrieve_Returns_Deleted_When_User_Can_Undelete()
    {
        var where = RetrieveWhereClause<SoftDeleteRow>(WithRead(DeletePermission));

        Assert.DoesNotContain("IsActive", where);
    }

    [Fact]
    public void Retrieve_Prefers_UndeletePermission_Attribute_Over_DeletePermission()
    {
        var where = RetrieveWhereClause<UndeletePermRow>(WithRead(DeletePermission));

        Assert.Contains("IsActive", where);
    }

    [Fact]
    public void Undelete_Denies_When_UndeletePermission_Attribute_Is_Not_Granted()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, IsActive = (short)-1 }));

        var ex = Assert.Throws<ValidationError>(() =>
            new UndeleteRequestHandler<UndeletePermRow>(WithRead(DeletePermission))
                .Undelete(new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 }));

        Assert.Equal("EntityNotFound", ex.ErrorCode);
    }

    [Fact]
    public void Undelete_Allows_When_UndeletePermission_Attribute_Is_Granted()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, IsActive = (short)-1 }))
            .InterceptExecuteNonQuery(args => (long?)1);

        var response = new UndeleteRequestHandler<UndeletePermRow>(WithRead(UndeletePermission))
            .Undelete(new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 });

        Assert.NotNull(response);
    }
}
