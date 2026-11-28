using System.Security.Claims;

namespace Serenity.Services;

public partial class DeleteRequestHandlerTests
{
    // a user (7) impersonated by an administrator (3), who is responsible for the delete
    private static NullRequestContext ImpersonatedDeleteContext() =>
        new NullRequestContext(userAccessor: new MockUserAccessor(() => new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.Name, "nancy"), new Claim(ClaimTypes.NameIdentifier, "7")], "Testing")
            {
                Actor = new ClaimsIdentity([new Claim(ClaimTypes.Name, "admin"), new Claim(ClaimTypes.NameIdentifier, "3")], "Testing")
            }))).WithPermissions(_ => true);

    private static MockDbConnection AuditDeleteConnection(object row, List<InterceptExecuteNonQueryArgs> updates) =>
        new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(row))
            .InterceptExecuteNonQuery(args =>
            {
                updates.Add(args);
                return 1;
            });

    private static object? ParameterOf(List<InterceptExecuteNonQueryArgs> updates, string column)
    {
        var update = Assert.Single(updates);
        // the parameters are in the order of the columns of the update, e.g. SET [DeleteDate] = @p1, [DeleteUserId] = @p2
        var set = update.CommandText[(update.CommandText.IndexOf("SET", StringComparison.Ordinal) + 3)..];
        var assignment = set.Split(',', StringSplitOptions.TrimEntries)
            .First(x => x.StartsWith("[" + column + "]", StringComparison.Ordinal));
        var name = assignment[(assignment.IndexOf('@') + 1)..].Split(' ')[0];
        return update.Parameters!.First(x => x.Key.TrimStart('@') == name).Value;
    }

    [Fact]
    public void SoftDelete_Writes_The_Impersonator_As_The_DeleteUser()
    {
        var updates = new List<InterceptExecuteNonQueryArgs>();
        using var connection = AuditDeleteConnection(new { Id = 5, IsDeleted = false, DeleteDate = (DateTime?)null, DeleteUserId = (long?)null }, updates);

        new DeleteRequestHandler<SoftDeleteLogRow>(ImpersonatedDeleteContext())
            .Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 });

        Assert.Equal(3L, Convert.ToInt64(ParameterOf(updates, "DeleteUserId")));
    }

    [Fact]
    public void SoftDelete_Writes_The_Impersonator_As_The_UpdateUser()
    {
        var updates = new List<InterceptExecuteNonQueryArgs>();
        using var connection = AuditDeleteConnection(new { Id = 5, IsDeleted = false, UpdateDate = (DateTime?)null, UpdateUserId = (long?)null }, updates);

        new DeleteRequestHandler<SoftUpdateLogRow>(ImpersonatedDeleteContext())
            .Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 });

        Assert.Equal(3L, Convert.ToInt64(ParameterOf(updates, "UpdateUserId")));
    }

    [Fact]
    public void DeleteLogOnly_Writes_The_Impersonator_As_The_DeleteUser()
    {
        var updates = new List<InterceptExecuteNonQueryArgs>();
        using var connection = AuditDeleteConnection(new { Id = 5, DeleteDate = (DateTime?)null, DeleteUserId = (long?)null }, updates);

        new DeleteRequestHandler<DeleteLogOnlyRow>(ImpersonatedDeleteContext())
            .Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 });

        Assert.Equal(3L, Convert.ToInt64(ParameterOf(updates, "DeleteUserId")));
    }

    [Fact]
    public async Task DeleteAsync_Writes_The_Impersonator_As_The_DeleteUser()
    {
        var updates = new List<InterceptExecuteNonQueryArgs>();
        using var connection = AuditDeleteConnection(new { Id = 5, IsDeleted = false, DeleteDate = (DateTime?)null, DeleteUserId = (long?)null }, updates);

        await new DeleteRequestHandlerAsync<SoftDeleteLogRow>(ImpersonatedDeleteContext())
            .DeleteAsync(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 }, TestContext.Current.CancellationToken);

        Assert.Equal(3L, Convert.ToInt64(ParameterOf(updates, "DeleteUserId")));
    }

    [Fact]
    public async Task DeleteAsync_DeleteLogOnly_Writes_The_Impersonator_As_The_DeleteUser()
    {
        var updates = new List<InterceptExecuteNonQueryArgs>();
        using var connection = AuditDeleteConnection(new { Id = 5, DeleteDate = (DateTime?)null, DeleteUserId = (long?)null }, updates);

        await new DeleteRequestHandlerAsync<DeleteLogOnlyRow>(ImpersonatedDeleteContext())
            .DeleteAsync(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 }, TestContext.Current.CancellationToken);

        Assert.Equal(3L, Convert.ToInt64(ParameterOf(updates, "DeleteUserId")));
    }
}
