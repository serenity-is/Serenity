using System.Threading;
using static Serenity.Services.UpdatableExtensionBehavior_ForeignThisKey_Test;

namespace Serenity.Services;

public class UpdatableExtensionBehavior_Async_Tests
{
    [Fact]
    public async Task ForeignKeyField_CanBeUsedAs_ThisKey_WithManualIdAssigned_Async()
    {
        bool saveDetailCalled = false;
        var handlerFactory = new MockHandlerFactory((rowType, intfType) =>
        {
            Assert.Equal(typeof(DetailRow), rowType);
            if (intfType == typeof(IListRequestProcessorAsync))
            {
                return new MockListHandlerAsync<DetailRow>(x =>
                {
                    x.Response.Entities = [new() { Id = 1357 }];
                });
            }
            else
            {
                Assert.Equal(typeof(ISaveRequestProcessorAsync), intfType);
                return new MockSaveHandlerAsync<DetailRow>(x =>
                {
                    Assert.Equal(SaveRequestType.Update, x.RequestType);
                    Assert.Equal(1357, x.Request?.EntityId);
                    var detail = Assert.IsType<DetailRow>(x.Request.Entity);
                    Assert.Equal(1357, detail.Id);
                    Assert.Equal("TestDetail", detail.Text);
                    saveDetailCalled = true;
                });
            }
        });
        var row = new MainRow()
        {
            DetailId = 1357,
            DetailText = "TestDetail"
        };
        var connection = new MockDbConnection();
        var handler = new MockSaveHandlerAsync<MainRow>()
        {
            Connection = connection,
            Row = row,
            IsUpdate = true,
            IsCreate = false,
            Old = new MainRow()
        };
        var behavior = new UpdatableExtensionBehavior(handlerFactory);
        Assert.True(behavior.ActivateFor(row));
        await behavior.OnBeforeSaveAsync(handler, CancellationToken.None);
        await behavior.OnAfterSaveAsync(handler, CancellationToken.None);
        Assert.True(saveDetailCalled);
    }

    [Fact]
    public async Task ForeignKeyField_CanBeUsedAs_ThisKey_WithoutDetailId_Async()
    {
        bool saveDetailCalled = false;
        var handlerFactory = new MockHandlerFactory((rowType, intfType) =>
        {
            Assert.Equal(typeof(DetailRow), rowType);
            Assert.Equal(typeof(ISaveRequestProcessorAsync), intfType);
            return new MockSaveHandlerAsync<DetailRow>(x =>
            {
                Assert.Equal(SaveRequestType.Create, x.RequestType);
                Assert.Null(x.Request?.EntityId);
                var detail = Assert.IsType<DetailRow>(x.Request.Entity);
                Assert.Null(detail.Id);
                Assert.Equal("TestDetail", detail.Text);
                saveDetailCalled = true;
                x.Response.EntityId = 1357;
            });
        });
        var row = new MainRow()
        {
            DetailText = "TestDetail"
        };
        var connection = new MockDbConnection();
        var handler = new MockSaveHandlerAsync<MainRow>()
        {
            Connection = connection,
            Row = row,
            IsCreate = true,
            IsUpdate = false
        };
        var behavior = new UpdatableExtensionBehavior(handlerFactory);
        Assert.True(behavior.ActivateFor(row));
        await behavior.OnBeforeSaveAsync(handler, CancellationToken.None);
        await behavior.OnAfterSaveAsync(handler, CancellationToken.None);
        Assert.True(saveDetailCalled);
        Assert.Equal(1357, row.DetailId);
    }

    [Fact]
    public async Task OnBeforeDeleteAsync_CascadeDeletes_ExistingDetail()
    {
        bool deleteDetailCalled = false;
        var handlerFactory = new MockHandlerFactory((rowType, intfType) =>
        {
            Assert.Equal(typeof(DetailRow), rowType);
            if (intfType == typeof(IListRequestProcessorAsync))
            {
                return new MockListHandlerAsync<DetailRow>(x =>
                {
                    x.Response.Entities = [new() { Id = 999 }];
                });
            }
            else
            {
                Assert.Equal(typeof(IDeleteRequestProcessorAsync), intfType);
                return new MockDeleteHandlerAsync<DetailRow>(x =>
                {
                    Assert.Equal(999, x.Request?.EntityId);
                    deleteDetailCalled = true;
                });
            }
        });
        var behavior = new UpdatableExtensionBehavior(handlerFactory);
        var row = new CascadeMainRow { Id = 5, DetailId = 5 };
        Assert.True(behavior.ActivateFor(row));

        var handler = new MockDeleteHandler<CascadeMainRow>
        {
            Row = row,
            Connection = new MockDbConnection(),
            UnitOfWork = new MockUnitOfWork(new MockDbConnection())
        };

        await behavior.OnBeforeDeleteAsync(handler, CancellationToken.None);

        Assert.True(deleteDetailCalled);
    }

    [Fact]
    public void OnBeforeDelete_CascadeDeletes_ExistingDetail()
    {
        bool deleteDetailCalled = false;
        var handlerFactory = new MockHandlerFactory((rowType, intfType) =>
        {
            Assert.Equal(typeof(DetailRow), rowType);
            if (intfType == typeof(IListRequestProcessor))
            {
                return new MockListHandler<DetailRow>(x =>
                {
                    x.Response.Entities = [new() { Id = 999 }];
                });
            }
            else
            {
                Assert.Equal(typeof(IDeleteRequestProcessor), intfType);
                return new MockDeleteHandler<DetailRow>(x =>
                {
                    Assert.Equal(999, x.Request?.EntityId);
                    deleteDetailCalled = true;
                });
            }
        });
        var behavior = new UpdatableExtensionBehavior(handlerFactory);
        var row = new CascadeMainRow { Id = 5, DetailId = 5 };
        Assert.True(behavior.ActivateFor(row));

        var handler = new MockDeleteHandler<CascadeMainRow>
        {
            Row = row,
            Connection = new MockDbConnection(),
            UnitOfWork = new MockUnitOfWork(new MockDbConnection())
        };

        behavior.OnBeforeDelete(handler);

        Assert.True(deleteDetailCalled);
    }

    [TableName("CascadeMains")]
    [UpdatableExtension("d", typeof(DetailRow), ThisKey = "DetailId", CascadeDelete = true)]
    public class CascadeMainRow : Row<CascadeMainRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [ForeignKey(typeof(DetailRow)), LeftJoin("d")]
        public int? DetailId { get => fields.DetailId[this]; set => fields.DetailId[this] = value; }

        [Expression("d.Text")]
        public string DetailText { get => fields.DetailText[this]; set => fields.DetailText[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public Int32Field DetailId;
            public StringField DetailText;
#pragma warning restore CS0649
        }
    }
}
