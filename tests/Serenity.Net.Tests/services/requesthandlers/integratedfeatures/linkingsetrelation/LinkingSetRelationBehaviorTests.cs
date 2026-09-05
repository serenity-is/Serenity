using System.Threading;

namespace Serenity.Services;

public class LinkingSetRelationBehaviorTests
{
    [TableName("LkMains")]
    private class LkMainRow : Row<LkMainRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NotMapped, MinSelectLevel(SelectLevel.Details)]
        [LinkingSetRelation(typeof(LkLinkRow), "MasterID", "ItemID")]
        public List<int> SelectedItems { get => fields.SelectedItems[this]; set => fields.SelectedItems[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public ListField<int> SelectedItems;
#pragma warning restore CS0649
        }
    }

    [TableName("LkLinks")]
    private class LkLinkRow : Row<LkLinkRow.RowFields>, IIdRow
    {
        [Identity]
        public long? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NotNull]
        public int? MasterID { get => fields.MasterID[this]; set => fields.MasterID[this] = value; }

        [NotNull]
        public int? ItemID { get => fields.ItemID[this]; set => fields.ItemID[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int64Field ID;
            public Int32Field MasterID;
            public Int32Field ItemID;
#pragma warning restore CS0649
        }
    }

    private static LinkingSetRelationBehavior CreateBehavior(LkMainRow row)
    {
        var behavior = new LinkingSetRelationBehavior(new MockHandlerFactory())
        {
            Target = row.GetFields().SelectedItems
        };
        Assert.True(behavior.ActivateFor(row));
        return behavior;
    }

    private static MockSaveHandler<LkMainRow> CreateSaveHandler(MockDbConnection connection, bool isCreate, LkMainRow row)
    {
        return new MockSaveHandler<LkMainRow>
        {
            Row = row,
            Old = isCreate ? null : new LkMainRow(),
            IsCreate = isCreate,
            IsUpdate = !isCreate,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        };
    }

    private static MockSaveHandlerAsync<LkMainRow> CreateSaveHandlerAsync(MockDbConnection connection, bool isCreate, LkMainRow row)
    {
        return new MockSaveHandlerAsync<LkMainRow>
        {
            Row = row,
            Old = isCreate ? null : new LkMainRow(),
            IsCreate = isCreate,
            IsUpdate = !isCreate,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        };
    }

    [Fact]
    public void OnReturn_Retrieve_LoadsSelectedItems()
    {
        var handlerFactory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(LkLinkRow), rowType);
            Assert.Equal(typeof(IListRequestProcessor), intf);
            return new MockListHandler<LkLinkRow>(x =>
            {
                x.Response.Entities.Add(new LkLinkRow { ItemID = 11 });
                x.Response.Entities.Add(new LkLinkRow { ItemID = 22 });
            });
        });

        var retrieve = new MockRetrieveHandler<LkMainRow>();
        retrieve.Row.ID = 5;
        var behavior = new LinkingSetRelationBehavior(handlerFactory)
        {
            Target = retrieve.Row.GetFields().SelectedItems
        };
        Assert.True(behavior.ActivateFor(retrieve.Row));
        behavior.OnReturn(retrieve);

        Assert.Equal([11, 22], retrieve.Row.SelectedItems);
    }

    [Fact]
    public async Task OnReturnAsync_Retrieve_LoadsSelectedItems()
    {
        var handlerFactory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(LkLinkRow), rowType);
            Assert.Equal(typeof(IListRequestProcessorAsync), intf);
            return new MockListHandlerAsync<LkLinkRow>(x =>
            {
                x.Response.Entities.Add(new LkLinkRow { ItemID = 11 });
                x.Response.Entities.Add(new LkLinkRow { ItemID = 22 });
            });
        });

        var retrieve = new MockRetrieveHandler<LkMainRow>();
        retrieve.Row.ID = 5;
        var behavior = new LinkingSetRelationBehavior(handlerFactory)
        {
            Target = retrieve.Row.GetFields().SelectedItems
        };
        Assert.True(behavior.ActivateFor(retrieve.Row));
        await behavior.OnReturnAsync(retrieve, CancellationToken.None);

        Assert.Equal([11, 22], retrieve.Row.SelectedItems);
    }

    [Fact]
    public void OnAfterSave_Sync_Create_InsertsLinks()
    {
        var savedItems = new List<int?>();
        var handlerFactory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(LkLinkRow), rowType);
            Assert.Equal(typeof(ISaveRequestProcessor), intf);
            return new MockSaveHandler<LkLinkRow>(x =>
            {
                Assert.Equal(SaveRequestType.Create, x.RequestType);
                var link = Assert.IsType<LkLinkRow>(x.Request.Entity);
                savedItems.Add(link.ItemID);
                Assert.Equal(5, link.MasterID);
            });
        });

        using var connection = new MockDbConnection();
        var master = new LkMainRow { ID = 5, SelectedItems = [11, 22] };
        var behavior = new LinkingSetRelationBehavior(handlerFactory)
        {
            Target = master.GetFields().SelectedItems
        };
        Assert.True(behavior.ActivateFor(master));

        behavior.OnAfterSave(CreateSaveHandler(connection, true, master));

        Assert.Equal([11, 22], savedItems);
    }

    [Fact]
    public async Task OnAfterSaveAsync_Async_Create_InsertsLinks()
    {
        var savedItems = new List<int?>();
        var handlerFactory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(LkLinkRow), rowType);
            Assert.Equal(typeof(ISaveRequestProcessorAsync), intf);
            return new MockSaveHandlerAsync<LkLinkRow>(x =>
            {
                Assert.Equal(SaveRequestType.Create, x.RequestType);
                var link = Assert.IsType<LkLinkRow>(x.Request.Entity);
                savedItems.Add(link.ItemID);
                Assert.Equal(5, link.MasterID);
            });
        });

        using var connection = new MockDbConnection();
        var master = new LkMainRow { ID = 5, SelectedItems = [11, 22] };
        var behavior = new LinkingSetRelationBehavior(handlerFactory)
        {
            Target = master.GetFields().SelectedItems
        };
        Assert.True(behavior.ActivateFor(master));

        await behavior.OnAfterSaveAsync(CreateSaveHandlerAsync(connection, true, master), CancellationToken.None);

        Assert.Equal([11, 22], savedItems);
    }

    [Fact]
    public void OnBeforeDelete_Sync_DeletesLinks()
    {
        var deleted = new List<long?>();
        var handlerFactory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(LkLinkRow), rowType);
            Assert.Equal(typeof(IDeleteRequestProcessor), intf);
            return new MockDeleteHandler<LkLinkRow>(x =>
            {
                deleted.Add(x.Request?.EntityId as long?);
            });
        });

        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => new MockDbDataReader(new { ID = 100L }, new { ID = 200L }));
        var master = new LkMainRow { ID = 5 };
        var behavior = new LinkingSetRelationBehavior(handlerFactory)
        {
            Target = master.GetFields().SelectedItems
        };
        Assert.True(behavior.ActivateFor(master));

        behavior.OnBeforeDelete(new MockDeleteHandler<LkMainRow>
        {
            Row = master,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        });

        Assert.Equal([100L, 200L], deleted);
    }

    [Fact]
    public async Task OnBeforeDeleteAsync_Async_DeletesLinks()
    {
        var deleted = new List<long?>();
        var handlerFactory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(LkLinkRow), rowType);
            Assert.Equal(typeof(IDeleteRequestProcessorAsync), intf);
            return new MockDeleteHandlerAsync<LkLinkRow>(x =>
            {
                deleted.Add(x.Request?.EntityId as long?);
            });
        });

        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => new MockDbDataReader(new { ID = 100L }, new { ID = 200L }));
        var master = new LkMainRow { ID = 5 };
        var behavior = new LinkingSetRelationBehavior(handlerFactory)
        {
            Target = master.GetFields().SelectedItems
        };
        Assert.True(behavior.ActivateFor(master));

        await behavior.OnBeforeDeleteAsync(new MockDeleteHandlerAsync<LkMainRow>
        {
            Row = master,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        }, CancellationToken.None);

        Assert.Equal([100L, 200L], deleted);
    }
}
