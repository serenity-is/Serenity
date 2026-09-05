using System.Globalization;
using System.Threading;

namespace Serenity.Services;

public class UniqueFieldSaveBehaviorTests
{
    [TableName("UniqueFieldRows")]
    private class UniqueFieldRow : Row<UniqueFieldRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [Unique]
        public string Code { get => fields.Code[this]; set => fields.Code[this] = value; }

        [Unique(IgnoreNulls = true)]
        public string NullableCode { get => fields.NullableCode[this]; set => fields.NullableCode[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public StringField Code;
            public StringField NullableCode;
#pragma warning restore CS0649
        }
    }

    private static UniqueFieldSaveBehavior CreateBehavior(Field target, UniqueFieldRow row)
    {
        var behavior = new UniqueFieldSaveBehavior(NullTextLocalizer.Instance)
        {
            Target = target
        };
        Assert.True(behavior.ActivateFor(row));
        return behavior;
    }

    private static MockSaveHandler<UniqueFieldRow> CreateHandler(bool isCreate,
        UniqueFieldRow row, MockDbConnection connection, UniqueFieldRow old = null)
    {
        return new MockSaveHandler<UniqueFieldRow>
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
    public void OnBeforeSave_Sync_NoDuplicate_Passes()
    {
        var behavior = CreateBehavior(UniqueFieldRow.Fields.Code, new UniqueFieldRow());
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.False(args.IsAsync);
                return new MockDbDataReader();
            });
        var handler = CreateHandler(true, new UniqueFieldRow { Code = "X" }, connection);

        behavior.OnBeforeSave(handler);

        Assert.Single(connection.ExecuteReaderCalls);
        Assert.False(connection.ExecuteReaderCalls[0].IsAsync);
    }

    [Fact]
    public async Task OnBeforeSaveAsync_Async_NoDuplicate_Passes()
    {
        var behavior = CreateBehavior(UniqueFieldRow.Fields.Code, new UniqueFieldRow());
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.True(args.IsAsync);
                return new MockDbDataReader();
            });
        var handler = CreateHandler(true, new UniqueFieldRow { Code = "X" }, connection);

        await behavior.OnBeforeSaveAsync(handler, CancellationToken.None);

        Assert.Single(connection.ExecuteReaderCalls);
        Assert.True(connection.ExecuteReaderCalls[0].IsAsync);
    }

    [Fact]
    public void OnBeforeSave_Sync_Duplicate_Throws()
    {
        var behavior = CreateBehavior(UniqueFieldRow.Fields.Code, new UniqueFieldRow());
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.False(args.IsAsync);
                return new MockDbDataReader(new { Value = 1 });
            });
        var handler = CreateHandler(true, new UniqueFieldRow { Code = "X" }, connection);

        var exception = Assert.Throws<ValidationError>(() => behavior.OnBeforeSave(handler));
        Assert.Equal("UniqueViolation", exception.ErrorCode);
    }

    [Fact]
    public async Task OnBeforeSaveAsync_Async_Duplicate_Throws()
    {
        var behavior = CreateBehavior(UniqueFieldRow.Fields.Code, new UniqueFieldRow());
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.True(args.IsAsync);
                return new MockDbDataReader(new { Value = 1 });
            });
        var handler = CreateHandler(true, new UniqueFieldRow { Code = "X" }, connection);

        var exception = await Assert.ThrowsAsync<ValidationError>(
            () => behavior.OnBeforeSaveAsync(handler, CancellationToken.None));
        Assert.Equal("UniqueViolation", exception.ErrorCode);
    }

    [Fact]
    public void OnBeforeSave_Sync_IgnoreNulls_SkipsQuery_WhenFieldIsNull()
    {
        var behavior = CreateBehavior(UniqueFieldRow.Fields.NullableCode, new UniqueFieldRow());
        using var connection = new MockDbConnection();
        var handler = CreateHandler(true, new UniqueFieldRow { NullableCode = null }, connection);

        behavior.OnBeforeSave(handler);

        Assert.Empty(connection.ExecuteReaderCalls);
    }

    [Fact]
    public async Task OnBeforeSaveAsync_Async_IgnoreNulls_SkipsQuery_WhenFieldIsNull()
    {
        var behavior = CreateBehavior(UniqueFieldRow.Fields.NullableCode, new UniqueFieldRow());
        using var connection = new MockDbConnection();
        var handler = CreateHandler(true, new UniqueFieldRow { NullableCode = null }, connection);

        await behavior.OnBeforeSaveAsync(handler, CancellationToken.None);

        Assert.Empty(connection.ExecuteReaderCalls);
    }

    [Fact]
    public void OnBeforeSave_Sync_Update_SameValue_SkipsQuery()
    {
        var behavior = CreateBehavior(UniqueFieldRow.Fields.Code, new UniqueFieldRow());
        using var connection = new MockDbConnection();
        var handler = CreateHandler(false, new UniqueFieldRow { Code = "X" }, connection,
            old: new UniqueFieldRow { Code = "X" });

        behavior.OnBeforeSave(handler);

        Assert.Empty(connection.ExecuteReaderCalls);
    }

    [Fact]
    public async Task OnBeforeSaveAsync_Async_Update_SameValue_SkipsQuery()
    {
        var behavior = CreateBehavior(UniqueFieldRow.Fields.Code, new UniqueFieldRow());
        using var connection = new MockDbConnection();
        var handler = CreateHandler(false, new UniqueFieldRow { Code = "X" }, connection,
            old: new UniqueFieldRow { Code = "X" });

        await behavior.OnBeforeSaveAsync(handler, CancellationToken.None);

        Assert.Empty(connection.ExecuteReaderCalls);
    }
}
