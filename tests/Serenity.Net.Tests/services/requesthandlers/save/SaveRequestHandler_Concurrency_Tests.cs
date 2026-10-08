namespace Serenity.Services;

public class SaveRequestHandler_Concurrency_Tests
{
    private static IRequestContext Context() =>
        new NullRequestContext().WithPermissions(_ => true);

    [ReadPermission(SpecialPermissionKeys.Public)]
    private class ConvRow : Row<ConvRow.RowFields>, IIdRow, IConcurrencyVersionRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        [Insertable(false), Updatable(false)]
        public int? Version { get => fields.Version[this]; set => fields.Version[this] = value; }

        Field IConcurrencyVersionRow.ConcurrencyVersionField => fields.Version;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Name;
            public Int32Field Version;

            public RowFields()
            {
                Id = new Int32Field(this, "Id");
                Name = new StringField(this, "Name");
                Version = new Int32Field(this, "Version");
            }
        }
    }

    private class ConvHandler(IRequestContext context) :
        SaveRequestHandler<ConvRow, SaveRequest<ConvRow>, SaveResponse>(context)
    {
    }

    [Fact]
    public void Update_With_Stale_Version_Throws_ConcurrencyConflict()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, Name = "Old", Version = 1 }))
            .OnDbCommandExecuteNonQuery(_ => 0);

        var handler = new ConvHandler(Context());
        var ex = Assert.Throws<ValidationError>(() =>
            handler.Update(new MockUnitOfWork(connection), new SaveRequest<ConvRow>
            {
                EntityId = 5,
                Entity = new ConvRow { Id = 5, Name = "New", Version = 1 }
            }));

        Assert.Equal("ConcurrencyConflict", ex.ErrorCode);
    }

    [Fact]
    public void Update_With_Matching_Version_Succeeds()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, Name = "Old", Version = 1 }))
            .OnDbCommandExecuteNonQuery(_ => 1);

        var handler = new ConvHandler(Context());
        var response = handler.Update(new MockUnitOfWork(connection), new SaveRequest<ConvRow>
        {
            EntityId = 5,
            Entity = new ConvRow { Id = 5, Name = "New", Version = 1 }
        });

        Assert.Equal(5, response.EntityId);
        Assert.Single(connection.ExecuteNonQueryCalls);
    }

    [Fact]
    public void Update_Without_Version_Falls_Back_To_Old_Version_And_Conflicts()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, Name = "Old", Version = 1 }))
            .OnDbCommandExecuteNonQuery(_ => 0);

        var handler = new ConvHandler(Context());
        var ex = Assert.Throws<ValidationError>(() =>
            handler.Update(new MockUnitOfWork(connection), new SaveRequest<ConvRow>
            {
                EntityId = 5,
                Entity = new ConvRow { Id = 5, Name = "New" }
            }));

        Assert.Equal("ConcurrencyConflict", ex.ErrorCode);
    }

    [Fact]
    public void Update_Without_Version_Uses_Old_Version_And_Succeeds_When_Affected()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, Name = "Old", Version = 1 }))
            .OnDbCommandExecuteNonQuery(_ => 1);

        var handler = new ConvHandler(Context());
        var response = handler.Update(new MockUnitOfWork(connection), new SaveRequest<ConvRow>
        {
            EntityId = 5,
            Entity = new ConvRow { Id = 5, Name = "New" }
        });

        Assert.Equal(5, response.EntityId);
    }

    [Fact]
    public void Update_With_IgnoreConcurrencyVersion_Skips_Check()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, Name = "Old", Version = 1 }))
            .OnDbCommandExecuteNonQuery(_ => 1);

        var handler = new ConvHandler(Context());
        var response = handler.Update(new MockUnitOfWork(connection), new SaveRequest<ConvRow>
        {
            EntityId = 5,
            IgnoreConcurrencyVersion = true,
            Entity = new ConvRow { Id = 5, Name = "New", Version = 0 }
        });

        Assert.Equal(5, response.EntityId);
    }
}
