using System.Threading;

namespace Serenity.Services;

public partial class MasterDetailRelationBehaviorTests
{
    [Fact]
    public void Retrieve_Sync_SuppressesPagingLimits()
    {
        using var connection = new MockDbConnection();

        var detailListHandler = new MockListHandler<Int32DetailRow>(handler =>
        {
            Assert.True(handler.Request.IsPagingLimitsSuppressed());
        });

        var handlerFactory = new MockHandlerFactory((_, _) => detailListHandler);

        var master = new Int32MasterRow { ID = 123 };
        var behavior = new MasterDetailRelationBehavior(handlerFactory)
        {
            Target = master.GetFields().DetailList
        };
        Assert.True(behavior.ActivateFor(master));

        behavior.OnReturn(new MockRetrieveHandler<Int32MasterRow> { Row = master });
    }

    [Fact]
    public async Task Retrieve_Async_SuppressesPagingLimits()
    {
        using var connection = new MockDbConnection();

        var detailListHandler = new MockListHandlerAsync<Int32DetailRow>(handler =>
        {
            Assert.True(handler.Request.IsPagingLimitsSuppressed());
        });

        var handlerFactory = new MockHandlerFactory((_, _) => detailListHandler);

        var master = new Int32MasterRow { ID = 123 };
        var behavior = new MasterDetailRelationBehavior(handlerFactory)
        {
            Target = master.GetFields().DetailList
        };
        Assert.True(behavior.ActivateFor(master));

        await behavior.OnReturnAsync(new MockRetrieveHandler<Int32MasterRow> { Row = master },
            CancellationToken.None);
    }

    [Fact]
    public void List_Sync_SuppressesPagingLimits()
    {
        var detailListHandler = new MockListHandler<Int32DetailRow>(handler =>
        {
            Assert.True(handler.Request.IsPagingLimitsSuppressed());
            handler.Response.Entities.Add(new Int32DetailRow { DetailID = 456, MasterID = 1 });
        });

        var handlerFactory = new MockHandlerFactory((_, _) => detailListHandler);

        var master = new Int32MasterRow { ID = 1 };
        var behavior = new MasterDetailRelationBehavior(handlerFactory)
        {
            Target = master.GetFields().DetailList
        };
        Assert.True(behavior.ActivateFor(master));

        var listHandler = new MockListHandler<Int32MasterRow>();
        listHandler.Response.Entities.Add(master);
        behavior.OnReturn((IListRequestHandler)listHandler);
    }

    [Fact]
    public async Task List_Async_SuppressesPagingLimits()
    {
        var detailListHandler = new MockListHandlerAsync<Int32DetailRow>(handler =>
        {
            Assert.True(handler.Request.IsPagingLimitsSuppressed());
            handler.Response.Entities.Add(new Int32DetailRow { DetailID = 456, MasterID = 1 });
        });

        var handlerFactory = new MockHandlerFactory((_, _) => detailListHandler);

        var master = new Int32MasterRow { ID = 1 };
        var behavior = new MasterDetailRelationBehavior(handlerFactory)
        {
            Target = master.GetFields().DetailList
        };
        Assert.True(behavior.ActivateFor(master));

        var listHandler = new MockListHandler<Int32MasterRow>();
        listHandler.Response.Entities.Add(master);
        await behavior.OnReturnAsync((IListRequestHandler)listHandler, CancellationToken.None);
    }

    [Fact]
    public void Save_CheckChangesOnUpdate_Sync_SuppressesPagingLimits()
    {
        var handlerFactory = new MockHandlerFactory((_, intf) =>
        {
            if (intf == typeof(IListRequestProcessor))
            {
                return new MockListHandler<Int32DetailRow>(handler =>
                {
                    Assert.True(handler.Request.IsPagingLimitsSuppressed());
                    handler.Response.Entities.Add(new Int32DetailRow { DetailID = 100, MasterID = 7 });
                });
            }

            throw new InvalidOperationException("Unexpected handler: " + intf);
        });

        using var connection = new MockDbConnection();
        var master = new Int32MasterRow { ID = 7, Name = "M" };
        var behavior = new MasterDetailRelationBehavior(handlerFactory)
        {
            Target = master.GetFields().DetailList
        };
        Assert.True(behavior.ActivateFor(master));

        master.DetailList = [new Int32DetailRow { DetailID = 100, MasterID = 7 }];
        behavior.OnAfterSave(CreateMasterSaveHandler(connection, false, master));
    }

    [Fact]
    public async Task Save_CheckChangesOnUpdate_Async_SuppressesPagingLimits()
    {
        var handlerFactory = new MockHandlerFactory((_, intf) =>
        {
            if (intf == typeof(IListRequestProcessorAsync))
            {
                return new MockListHandlerAsync<Int32DetailRow>(handler =>
                {
                    Assert.True(handler.Request.IsPagingLimitsSuppressed());
                    handler.Response.Entities.Add(new Int32DetailRow { DetailID = 100, MasterID = 7 });
                });
            }

            throw new InvalidOperationException("Unexpected handler: " + intf);
        });

        using var connection = new MockDbConnection();
        var master = new Int32MasterRow { ID = 7, Name = "M" };
        var behavior = new MasterDetailRelationBehavior(handlerFactory)
        {
            Target = master.GetFields().DetailList
        };
        Assert.True(behavior.ActivateFor(master));

        master.DetailList = [new Int32DetailRow { DetailID = 100, MasterID = 7 }];
        await behavior.OnAfterSaveAsync(CreateMasterSaveHandlerAsync(connection, false, master),
            CancellationToken.None);
    }

    [Fact]
    public void List_OnReturn_Fills_Masters_Across_Multiple_Batches()
    {
        const int count = 2500;

        var detailListHandler = new MockListHandler<Int32DetailRow>(handler =>
        {
            handler.Response.Entities.Clear();
            for (var i = 1; i <= count; i++)
                handler.Response.Entities.Add(new Int32DetailRow { DetailID = i, MasterID = i });
        });

        var handlerFactory = new MockHandlerFactory((_, _) => detailListHandler);

        var master = new Int32MasterRow { ID = 1 };
        var behavior = new MasterDetailRelationBehavior(handlerFactory)
        {
            Target = master.GetFields().DetailList
        };
        Assert.True(behavior.ActivateFor(master));

        var listHandler = new MockListHandler<Int32MasterRow>();
        for (var i = 1; i <= count; i++)
            listHandler.Response.Entities.Add(new Int32MasterRow { ID = i });

        behavior.OnReturn((IListRequestHandler)listHandler);

        Assert.Equal(count, listHandler.Response.Entities.Count);
        foreach (var entity in listHandler.Response.Entities)
        {
            var detail = Assert.Single(entity.DetailList!);
            Assert.Equal(entity.ID, detail.MasterID);
        }
    }

    [Fact]
    public async Task List_OnReturnAsync_Fills_Masters_Across_Multiple_Batches()
    {
        const int count = 2500;

        var detailListHandler = new MockListHandlerAsync<Int32DetailRow>(handler =>
        {
            handler.Response.Entities.Clear();
            for (var i = 1; i <= count; i++)
                handler.Response.Entities.Add(new Int32DetailRow { DetailID = i, MasterID = i });
        });

        var handlerFactory = new MockHandlerFactory((_, _) => detailListHandler);

        var master = new Int32MasterRow { ID = 1 };
        var behavior = new MasterDetailRelationBehavior(handlerFactory)
        {
            Target = master.GetFields().DetailList
        };
        Assert.True(behavior.ActivateFor(master));

        var listHandler = new MockListHandlerAsync<Int32MasterRow>();
        for (var i = 1; i <= count; i++)
            listHandler.Response.Entities.Add(new Int32MasterRow { ID = i });

        await behavior.OnReturnAsync((IListRequestHandler)listHandler, CancellationToken.None);

        Assert.Equal(count, listHandler.Response.Entities.Count);
        foreach (var entity in listHandler.Response.Entities)
        {
            var detail = Assert.Single(entity.DetailList!);
            Assert.Equal(entity.ID, detail.MasterID);
        }
    }
}
