using Serenity.TestUtils;

namespace Serenity.Services;

public class FailClosedPermissionTests
{
    private class UnprotectedRow : Row<UnprotectedRow.RowFields>, IIdRow
    {
        [IdProperty, Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
        }
    }

    [ReadPermission(SpecialPermissionKeys.Public)]
    private class ProtectedRow : Row<ProtectedRow.RowFields>, IIdRow
    {
        [IdProperty, Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
        }
    }

    private static IRequestContext AllowAll() =>
        new NullRequestContext().WithPermissions(_ => true);

    private static MockDbConnection ConnectionWithRow() =>
        new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5 }));

    [Fact]
    public void List_Denies_Row_Without_Permission_Attributes()
    {
        using var conn = new MockDbConnection().InterceptExecuteReader(args => args.ToMockReader());

        var ex = Assert.Throws<ValidationError>(() =>
            new ListRequestHandler<UnprotectedRow>(AllowAll()).List(conn, new ListRequest()));

        Assert.Equal("AccessDenied", ex.ErrorCode);
    }

    [Fact]
    public void Retrieve_Denies_Row_Without_Permission_Attributes()
    {
        using var conn = ConnectionWithRow();

        var ex = Assert.Throws<ValidationError>(() =>
            new RetrieveRequestHandler<UnprotectedRow>(AllowAll())
                .Retrieve(conn, new RetrieveRequest { EntityId = 5 }));

        Assert.Equal("AccessDenied", ex.ErrorCode);
    }

    [Fact]
    public void Save_Create_Denies_Row_Without_Permission_Attributes()
    {
        using var conn = new MockDbConnection();

        var ex = Assert.Throws<ValidationError>(() =>
            new SaveRequestHandler<UnprotectedRow>(AllowAll())
                .Create(new MockUnitOfWork(conn), new() { Entity = new UnprotectedRow() }));

        Assert.Equal("AccessDenied", ex.ErrorCode);
    }

    [Fact]
    public void Save_Update_Denies_Row_Without_Permission_Attributes()
    {
        using var conn = ConnectionWithRow();

        var ex = Assert.Throws<ValidationError>(() =>
            new SaveRequestHandler<UnprotectedRow>(AllowAll())
                .Update(new MockUnitOfWork(conn), new() { EntityId = 5, Entity = new UnprotectedRow { Id = 5 } }));

        Assert.Equal("EntityNotFound", ex.ErrorCode);
    }

    [Fact]
    public void Delete_Denies_Row_Without_Permission_Attributes()
    {
        using var conn = ConnectionWithRow();

        var ex = Assert.Throws<ValidationError>(() =>
            new DeleteRequestHandler<UnprotectedRow>(AllowAll())
                .Delete(new MockUnitOfWork(conn), new DeleteRequest { EntityId = 5 }));

        Assert.Equal("EntityNotFound", ex.ErrorCode);
    }

    [Fact]
    public void Undelete_Denies_Row_Without_Permission_Attributes()
    {
        using var conn = ConnectionWithRow();

        var ex = Assert.Throws<ValidationError>(() =>
            new UndeleteRequestHandler<UnprotectedRow>(AllowAll())
                .Undelete(new MockUnitOfWork(conn), new UndeleteRequest { EntityId = 5 }));

        Assert.Equal("EntityNotFound", ex.ErrorCode);
    }

    [Fact]
    public void Explicit_Public_Permission_Allows_Access()
    {
        using var conn = ConnectionWithRow();

        var response = new RetrieveRequestHandler<ProtectedRow>(AllowAll())
            .Retrieve(conn, new RetrieveRequest { EntityId = 5 });

        Assert.NotNull(response);
    }

    [Fact]
    public void Explicit_LoggedIn_Permission_Requires_Login()
    {
        using var conn = ConnectionWithRow();

        Assert.Throws<ValidationError>(() =>
            new RetrieveRequestHandler<LoggedInRow>(new NullRequestContext().WithPermissions(_ => false))
                .Retrieve(conn, new RetrieveRequest { EntityId = 5 }));
    }

    [ReadPermission(SpecialPermissionKeys.LoggedIn)]
    private class LoggedInRow : Row<LoggedInRow.RowFields>, IIdRow
    {
        [IdProperty, Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
        }
    }
}
