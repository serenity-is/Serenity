using System.Globalization;
using System.Threading;

namespace Serenity.Services;

public class UniqueConstraintBehaviorTests
{
    [TableName("UniqueRows")]
    [UniqueConstraint("Name")]
    private class UniqueNameRow : Row<UniqueNameRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public StringField Name;
#pragma warning restore CS0649
        }
    }

    private static UniqueConstraintSaveBehavior CreateBehavior()
    {
        return new UniqueConstraintSaveBehavior(NullTextLocalizer.Instance);
    }

    [Fact]
    public void OnBeforeSave_Sync_NoDuplicate_Passes()
    {
        var behavior = CreateBehavior();
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.False(args.IsAsync);
                return new MockDbDataReader();
            });
        var handler = new MockSaveHandler<UniqueNameRow>
        {
            Row = new UniqueNameRow { Name = "Test" },
            IsCreate = true,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        };
        Assert.True(behavior.ActivateFor(new UniqueNameRow { Name = "Test" }));

        behavior.OnBeforeSave(handler);

        Assert.Single(connection.ExecuteReaderCalls);
        Assert.False(connection.ExecuteReaderCalls[0].IsAsync);
    }

    [Fact]
    public async Task OnBeforeSaveAsync_Async_NoDuplicate_Passes()
    {
        var behavior = CreateBehavior();
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.True(args.IsAsync);
                return new MockDbDataReader();
            });
        var handler = new MockSaveHandler<UniqueNameRow>
        {
            Row = new UniqueNameRow { Name = "Test" },
            IsCreate = true,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        };
        Assert.True(behavior.ActivateFor(new UniqueNameRow { Name = "Test" }));

        await behavior.OnBeforeSaveAsync(handler, CancellationToken.None);

        Assert.Single(connection.ExecuteReaderCalls);
        Assert.True(connection.ExecuteReaderCalls[0].IsAsync);
    }

    [Fact]
    public void OnBeforeSave_Sync_Duplicate_Throws()
    {
        var behavior = CreateBehavior();
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.False(args.IsAsync);
                return new MockDbDataReader(new { Value = 1 });
            });
        var handler = new MockSaveHandler<UniqueNameRow>
        {
            Row = new UniqueNameRow { Name = "Test" },
            IsCreate = true,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        };
        Assert.True(behavior.ActivateFor(new UniqueNameRow { Name = "Test" }));

        var exception = Assert.Throws<ValidationError>(() => behavior.OnBeforeSave(handler));
        Assert.Equal("UniqueViolation", exception.ErrorCode);
    }

    [Fact]
    public async Task OnBeforeSaveAsync_Async_Duplicate_Throws()
    {
        var behavior = CreateBehavior();
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.True(args.IsAsync);
                return new MockDbDataReader(new { Value = 1 });
            });
        var handler = new MockSaveHandler<UniqueNameRow>
        {
            Row = new UniqueNameRow { Name = "Test" },
            IsCreate = true,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        };
        Assert.True(behavior.ActivateFor(new UniqueNameRow { Name = "Test" }));

        var exception = await Assert.ThrowsAsync<ValidationError>(
            () => behavior.OnBeforeSaveAsync(handler, CancellationToken.None));
        Assert.Equal("UniqueViolation", exception.ErrorCode);
    }
}
