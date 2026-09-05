using System.Threading;

namespace Serenity.Services;

public partial class MasterDetailRelationBehaviorTests
{
    private static MockSaveHandler<Int32MasterRow> CreateMasterSaveHandler(
        MockDbConnection connection, bool isCreate, Int32MasterRow row)
    {
        return new MockSaveHandler<Int32MasterRow>
        {
            Row = row,
            Old = isCreate ? null : new Int32MasterRow(),
            IsCreate = isCreate,
            IsUpdate = !isCreate,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        };
    }

    private static MockSaveHandlerAsync<Int32MasterRow> CreateMasterSaveHandlerAsync(
        MockDbConnection connection, bool isCreate, Int32MasterRow row)
    {
        return new MockSaveHandlerAsync<Int32MasterRow>
        {
            Row = row,
            Old = isCreate ? null : new Int32MasterRow(),
            IsCreate = isCreate,
            IsUpdate = !isCreate,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        };
    }

    private static MasterDetailRelationBehavior CreateBehavior(Int32MasterRow row)
    {
        var behavior = new MasterDetailRelationBehavior(new MockHandlerFactory())
        {
            Target = row.GetFields().DetailList
        };
        Assert.True(behavior.ActivateFor(row));
        return behavior;
    }

    [Fact]
    public async Task LoadsDetailsOnRetrieveAsync_Int32Keys()
    {
        using var connection = new MockDbConnection();

        var detailListHandler = new MockListHandlerAsync<Int32DetailRow>(handler =>
        {
            handler.Response.Entities.Add(new Int32DetailRow { DetailID = 456 });
            handler.Response.Entities.Add(new Int32DetailRow { DetailID = 789 });
        });

        var handlerFactory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(Int32DetailRow), rowType);
            Assert.Equal(typeof(IListRequestProcessorAsync), intf);
            return detailListHandler;
        });

        var masterRetrieveHandler = new MockRetrieveHandler<Int32MasterRow>();
        masterRetrieveHandler.Row.ID = 123;
        var behavior = new MasterDetailRelationBehavior(handlerFactory)
        {
            Target = masterRetrieveHandler.Row.GetFields().DetailList
        };
        Assert.True(behavior.ActivateFor(masterRetrieveHandler.Row));
        await behavior.OnReturnAsync(masterRetrieveHandler, CancellationToken.None);

        Assert.Collection(masterRetrieveHandler.Row.DetailList,
            x1 => Assert.Equal(456, x1.DetailID),
            x2 => Assert.Equal(789, x2.DetailID));
    }

    [Fact]
    public void OnAfterSave_Sync_Create_InsertsDetails()
    {
        var saved = new List<SaveRequestType>();
        var handlerFactory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(Int32DetailRow), rowType);
            Assert.Equal(typeof(ISaveRequestProcessor), intf);
            return new MockSaveHandler<Int32DetailRow>(x =>
            {
                saved.Add(x.RequestType);
            });
        });

        using var connection = new MockDbConnection();
        var master = new Int32MasterRow { ID = 7, Name = "M" };
        var behavior = new MasterDetailRelationBehavior(handlerFactory)
        {
            Target = master.GetFields().DetailList
        };
        Assert.True(behavior.ActivateFor(master));

        master.DetailList = [new Int32DetailRow { ProductID = 1, Quantity = 2m }];
        var handler = CreateMasterSaveHandler(connection, true, master);

        behavior.OnAfterSave(handler);

        Assert.Equal([SaveRequestType.Create], saved);
    }

    [Fact]
    public async Task OnAfterSaveAsync_Async_Create_InsertsDetails()
    {
        var saved = new List<SaveRequestType>();
        var handlerFactory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(Int32DetailRow), rowType);
            Assert.Equal(typeof(ISaveRequestProcessorAsync), intf);
            return new MockSaveHandlerAsync<Int32DetailRow>(x =>
            {
                saved.Add(x.RequestType);
            });
        });

        using var connection = new MockDbConnection();
        var master = new Int32MasterRow { ID = 7, Name = "M" };
        var behavior = new MasterDetailRelationBehavior(handlerFactory)
        {
            Target = master.GetFields().DetailList
        };
        Assert.True(behavior.ActivateFor(master));

        master.DetailList = [new Int32DetailRow { ProductID = 1, Quantity = 2m }];
        var handler = CreateMasterSaveHandlerAsync(connection, true, master);

        await behavior.OnAfterSaveAsync(handler, CancellationToken.None);

        Assert.Equal([SaveRequestType.Create], saved);
    }

    [Fact]
    public void OnAfterSave_Sync_Update_DeletesRemovedAndInsertsAdded()
    {
        var calls = new List<(SaveRequestType type, int? id)>();
        var handlerFactory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(Int32DetailRow), rowType);
            if (intf == typeof(IListRequestProcessor))
            {
                // old detail rows
                return new MockListHandler<Int32DetailRow>(x =>
                {
                    x.Response.Entities.Add(new Int32DetailRow { DetailID = 100, MasterID = 7, ProductID = 1, Quantity = 1m });
                    x.Response.Entities.Add(new Int32DetailRow { DetailID = 200, MasterID = 7, ProductID = 2, Quantity = 2m });
                });
            }
            else
            {
                Assert.Equal(typeof(IDeleteRequestProcessor), intf);
                return new MockDeleteHandler<Int32DetailRow>(x =>
                {
                    calls.Add((SaveRequestType.Update, x.Request?.EntityId as int?));
                });
            }
        });

        using var connection = new MockDbConnection();
        var master = new Int32MasterRow { ID = 7, Name = "M" };
        var behavior = new MasterDetailRelationBehavior(handlerFactory)
        {
            Target = master.GetFields().DetailList
        };
        Assert.True(behavior.ActivateFor(master));

        // remove detail 200, keep 100 unchanged
        master.DetailList =
        [
            new Int32DetailRow { DetailID = 100, MasterID = 7, ProductID = 1, Quantity = 1m }
        ];
        var handler = CreateMasterSaveHandler(connection, false, master);

        behavior.OnAfterSave(handler);

        // 200 was removed => deleted, 100 unchanged => no save
        Assert.Single(calls);
        Assert.Equal(200, calls[0].id);
    }

    [Fact]
    public void OnBeforeDelete_Sync_HardDelete_DeletesDetails()
    {
        var deleted = new List<int?>();
        var handlerFactory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(Int32DetailRow), rowType);
            Assert.Equal(typeof(IDeleteRequestProcessor), intf);
            return new MockDeleteHandler<Int32DetailRow>(x =>
            {
                deleted.Add(x.Request?.EntityId as int?);
            });
        });

        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => new MockDbDataReader(new { DetailID = 100 }, new { DetailID = 200 }));
        var master = new Int32MasterRow { ID = 7, Name = "M" };
        var behavior = new MasterDetailRelationBehavior(handlerFactory)
        {
            Target = master.GetFields().DetailList
        };
        Assert.True(behavior.ActivateFor(master));

        var handler = new MockDeleteHandler<Int32MasterRow>
        {
            Row = master,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        };

        behavior.OnBeforeDelete(handler);

        Assert.Equal([100, 200], deleted);
    }

    [Fact]
    public async Task OnBeforeDeleteAsync_Async_HardDelete_DeletesDetails()
    {
        var deleted = new List<int?>();
        var handlerFactory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(Int32DetailRow), rowType);
            Assert.Equal(typeof(IDeleteRequestProcessorAsync), intf);
            return new MockDeleteHandlerAsync<Int32DetailRow>(x =>
            {
                deleted.Add(x.Request?.EntityId as int?);
            });
        });

        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => new MockDbDataReader(new { DetailID = 100 }, new { DetailID = 200 }));
        var master = new Int32MasterRow { ID = 7, Name = "M" };
        var behavior = new MasterDetailRelationBehavior(handlerFactory)
        {
            Target = master.GetFields().DetailList
        };
        Assert.True(behavior.ActivateFor(master));

        var handler = new MockDeleteHandlerAsync<Int32MasterRow>
        {
            Row = master,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        };

        await behavior.OnBeforeDeleteAsync(handler, CancellationToken.None);

        Assert.Equal([100, 200], deleted);
    }
}
