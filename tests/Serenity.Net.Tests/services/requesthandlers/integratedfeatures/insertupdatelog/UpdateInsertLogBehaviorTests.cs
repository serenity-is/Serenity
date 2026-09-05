using System.Globalization;
using System.Threading;

namespace Serenity.Services;

public class UpdateInsertLogBehaviorTests
{
    [TableName("TestLogRows")]
    private class TestLogRow : Row<TestLogRow.RowFields>, IIdRow,
        IInsertDateRow, IInsertUserIdRow, IUpdateDateRow, IUpdateUserIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public DateTime? InsertDate { get => fields.InsertDate[this]; set => fields.InsertDate[this] = value; }
        public DateTime? UpdateDate { get => fields.UpdateDate[this]; set => fields.UpdateDate[this] = value; }
        public int? InsertUserId { get => fields.InsertUserId[this]; set => fields.InsertUserId[this] = value; }
        public int? UpdateUserId { get => fields.UpdateUserId[this]; set => fields.UpdateUserId[this] = value; }

        public DateTimeField InsertDateField => fields.InsertDate;
        public DateTimeField UpdateDateField => fields.UpdateDate;
        public Field InsertUserIdField => fields.InsertUserId;
        public Field UpdateUserIdField => fields.UpdateUserId;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public DateTimeField InsertDate;
            public DateTimeField UpdateDate;
            public Int32Field InsertUserId;
            public Int32Field UpdateUserId;
#pragma warning restore CS0649
        }
    }

    private const string adminId = "123456";

    private static MockSaveHandler<TestLogRow> CreateHandler(bool isCreate)
    {
        var context = new NullRequestContext(
            userAccessor: new MockUserAccessor(
                getUsername: () => "admin", getIdentifier: () => adminId));
        var handler = new MockSaveHandler<TestLogRow>
        {
            Row = new TestLogRow(),
            IsCreate = isCreate,
            IsUpdate = !isCreate,
            Context = context
        };
        return handler;
    }

    private static UpdateInsertLogBehavior CreateBehavior()
    {
        var behavior = new UpdateInsertLogBehavior();
        Assert.True(behavior.ActivateFor(new TestLogRow()));
        return behavior;
    }

    [Fact]
    public void ActivateFor_ReturnsFalse_ForRowWithoutLogInterfaces()
    {
        var behavior = new UpdateInsertLogBehavior();
        Assert.False(behavior.ActivateFor(new IdNameRowStub()));
    }

    private class IdNameRowStub : Row<IdNameRowStub.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
#pragma warning restore CS0649
        }
    }

    [Fact]
    public void OnSetInternalFields_Sync_Create_SetsInsertDateAndInsertUserId()
    {
        var behavior = CreateBehavior();
        var handler = CreateHandler(isCreate: true);

        behavior.OnSetInternalFields(handler);

        var row = (TestLogRow)handler.Row;
        Assert.NotNull(row.InsertDate);
        Assert.Equal(adminId, Convert.ToString(row.InsertUserId,
            CultureInfo.InvariantCulture));
        Assert.Null(row.UpdateDate);
        Assert.Null(row.UpdateUserId);
    }

    [Fact]
    public async Task OnSetInternalFieldsAsync_Async_Create_SetsInsertDateAndInsertUserId()
    {
        var behavior = CreateBehavior();
        var handler = CreateHandler(isCreate: true);

        await behavior.OnSetInternalFieldsAsync(handler, CancellationToken.None);

        var row = (TestLogRow)handler.Row;
        Assert.NotNull(row.InsertDate);
        Assert.Equal(adminId, Convert.ToString(row.InsertUserId,
            CultureInfo.InvariantCulture));
        Assert.Null(row.UpdateDate);
        Assert.Null(row.UpdateUserId);
    }

    [Fact]
    public void OnSetInternalFields_Sync_Update_SetsUpdateDateAndUpdateUserId()
    {
        var behavior = CreateBehavior();
        var handler = CreateHandler(isCreate: false);
        handler.Old = new TestLogRow();

        behavior.OnSetInternalFields(handler);

        var row = (TestLogRow)handler.Row;
        Assert.Null(row.InsertDate);
        Assert.Null(row.InsertUserId);
        Assert.NotNull(row.UpdateDate);
        Assert.Equal(adminId, Convert.ToString(row.UpdateUserId,
            CultureInfo.InvariantCulture));
    }

    [Fact]
    public async Task OnSetInternalFieldsAsync_Async_Update_SetsUpdateDateAndUpdateUserId()
    {
        var behavior = CreateBehavior();
        var handler = CreateHandler(isCreate: false);
        handler.Old = new TestLogRow();

        await behavior.OnSetInternalFieldsAsync(handler, CancellationToken.None);

        var row = (TestLogRow)handler.Row;
        Assert.Null(row.InsertDate);
        Assert.Null(row.InsertUserId);
        Assert.NotNull(row.UpdateDate);
        Assert.Equal(adminId, Convert.ToString(row.UpdateUserId,
            CultureInfo.InvariantCulture));
    }
}
