namespace Serenity.Services;

public class SaveRequestHandlerTests_NoOpUpdate
{
    private static IRequestContext Context() => new NullRequestContext().WithPermissions(_ => true);

    [ReadPermission(SpecialPermissionKeys.Public)]
    private class NoOpRow : Row<NoOpRow.RowFields>, IIdRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Name;

            public RowFields()
            {
                Id = new Int32Field(this, "Id");
                Name = new StringField(this, "Name");
            }
        }
    }

    private class SyncHandler(IRequestContext context) :
        SaveRequestHandler<NoOpRow, SaveRequest<NoOpRow>, SaveResponse>(context)
    {
    }

    private class AsyncHandler(IRequestContext context) :
        SaveRequestHandlerAsync<NoOpRow, SaveRequest<NoOpRow>, SaveResponse>(context)
    {
    }

    [Fact]
    public void Update_With_No_Assigned_Fields_Returns_EntityId_From_Old()
    {
        bool executed = false;
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, Name = "Old" }))
            .OnDbCommandExecuteNonQuery(_ =>
            {
                executed = true;
                return 1;
            });

        var response = new SyncHandler(Context()).Update(new MockUnitOfWork(connection),
            new SaveRequest<NoOpRow> { EntityId = 5, Entity = new NoOpRow() });

        Assert.False(executed);
        Assert.Equal(5, response.EntityId);
    }

    [Fact]
    public async Task UpdateAsync_With_No_Assigned_Fields_Returns_EntityId_From_Old()
    {
        bool executed = false;
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, Name = "Old" }))
            .OnDbCommandExecuteNonQuery(_ =>
            {
                executed = true;
                return 1;
            });

        var response = await new AsyncHandler(Context()).UpdateAsync(new MockUnitOfWork(connection),
            new SaveRequest<NoOpRow> { EntityId = 5, Entity = new NoOpRow() },
            TestContext.Current.CancellationToken);

        Assert.False(executed);
        Assert.Equal(5, response.EntityId);
    }
}
