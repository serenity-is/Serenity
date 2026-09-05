using System.Threading;

namespace Serenity.Services;

public class ValidateParentBehaviorTests
{
    [TableName("Parents")]
    private class ParentRow : Row<ParentRow.RowFields>, IIdRow, IIsActiveRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public short? IsActive { get => fields.IsActive[this]; set => fields.IsActive[this] = value; }

        public Int16Field IsActiveField => fields.IsActive;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public Int16Field IsActive;
#pragma warning restore CS0649
        }
    }

    [TableName("Children")]
    private class ChildRow : Row<ChildRow.RowFields>, IIdRow, IParentIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [ForeignKey(typeof(ParentRow))]
        public int? ParentId { get => fields.ParentId[this]; set => fields.ParentId[this] = value; }

        public Field ParentIdField => fields.ParentId;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public Int32Field ParentId;
#pragma warning restore CS0649
        }
    }

    private sealed class FakeRowTypeRegistry(params Type[] rowTypes) : IRowTypeRegistry
    {
        public IEnumerable<Type> AllRowTypes => rowTypes;
        public IEnumerable<Type> ByConnectionKey(string connectionKey) => rowTypes;
    }

    private static ValidateParentBehavior CreateBehavior()
    {
        return new ValidateParentBehavior(new FakeRowTypeRegistry(typeof(ParentRow)),
            NullTextLocalizer.Instance);
    }

    private static MockSaveHandler<ChildRow> CreateHandler(MockDbConnection connection,
        bool isCreate, ChildRow row, ChildRow old = null)
    {
        return new MockSaveHandler<ChildRow>
        {
            Row = row,
            Old = old,
            IsCreate = isCreate,
            IsUpdate = !isCreate,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        };
    }

    [Fact]
    public void OnValidateRequest_Sync_ParentNotDeleted_Passes()
    {
        var behavior = CreateBehavior();
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.False(args.IsAsync);
                return new MockDbDataReader();
            });
        var handler = CreateHandler(connection, true, new ChildRow { ParentId = 1 });

        behavior.OnValidateRequest(handler);

        Assert.Single(connection.ExecuteReaderCalls);
        Assert.False(connection.ExecuteReaderCalls[0].IsAsync);
    }

    [Fact]
    public async Task OnValidateRequestAsync_Async_ParentNotDeleted_Passes()
    {
        var behavior = CreateBehavior();
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.True(args.IsAsync);
                return new MockDbDataReader();
            });
        var handler = CreateHandler(connection, true, new ChildRow { ParentId = 1 });

        await behavior.OnValidateRequestAsync(handler, CancellationToken.None);

        Assert.Single(connection.ExecuteReaderCalls);
        Assert.True(connection.ExecuteReaderCalls[0].IsAsync);
    }

    [Fact]
    public void OnValidateRequest_Sync_ParentDeleted_Throws()
    {
        var behavior = CreateBehavior();
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.False(args.IsAsync);
                return new MockDbDataReader(new { Value = 1 });
            });
        var handler = CreateHandler(connection, true, new ChildRow { ParentId = 1 });

        Assert.Throws<ValidationError>(() => behavior.OnValidateRequest(handler));
    }

    [Fact]
    public async Task OnValidateRequestAsync_Async_ParentDeleted_Throws()
    {
        var behavior = CreateBehavior();
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.True(args.IsAsync);
                return new MockDbDataReader(new { Value = 1 });
            });
        var handler = CreateHandler(connection, true, new ChildRow { ParentId = 1 });

        await Assert.ThrowsAsync<ValidationError>(
            () => behavior.OnValidateRequestAsync(handler, CancellationToken.None));
    }

    [Fact]
    public void OnValidateRequest_Sync_UpdateWithSameParent_SkipsQuery()
    {
        var behavior = CreateBehavior();
        using var connection = new MockDbConnection();
        var handler = CreateHandler(connection, false, new ChildRow { ParentId = 1 },
            old: new ChildRow { ParentId = 1 });

        behavior.OnValidateRequest(handler);

        Assert.Empty(connection.ExecuteReaderCalls);
    }

    [Fact]
    public void OnValidateRequest_Sync_NoParentId_SkipsQuery()
    {
        var behavior = CreateBehavior();
        using var connection = new MockDbConnection();
        var handler = CreateHandler(connection, true, new ChildRow { ParentId = null });

        behavior.OnValidateRequest(handler);

        Assert.Empty(connection.ExecuteReaderCalls);
    }
}
