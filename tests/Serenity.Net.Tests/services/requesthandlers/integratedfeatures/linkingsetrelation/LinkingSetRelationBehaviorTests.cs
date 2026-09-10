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

    private static MockSaveHandler<TRow> CreateSaveHandler<TRow>(MockDbConnection connection, bool isCreate, TRow row)
        where TRow : IRow, new()
    {
        return new MockSaveHandler<TRow>
        {
            Row = row,
            Old = isCreate ? null! : new TRow(),
            IsCreate = isCreate,
            IsUpdate = !isCreate,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        };
    }

    private static MockSaveHandlerAsync<TRow> CreateSaveHandlerAsync<TRow>(MockDbConnection connection, bool isCreate, TRow row)
        where TRow : IRow, new()
    {
        return new MockSaveHandlerAsync<TRow>
        {
            Row = row,
            Old = isCreate ? null! : new TRow(),
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

    [TableName("LkFilteredLinks")]
    private class LkFilteredLinkRow : Row<LkFilteredLinkRow.RowFields>, IIdRow
    {
        [Identity]
        public long? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public int? MasterID { get => fields.MasterID[this]; set => fields.MasterID[this] = value; }
        public int? ItemID { get => fields.ItemID[this]; set => fields.ItemID[this] = value; }
        public int? KindID { get => fields.KindID[this]; set => fields.KindID[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int64Field ID;
            public Int32Field MasterID;
            public Int32Field ItemID;
            public Int32Field KindID;
#pragma warning restore CS0649
        }
    }

    [TableName("LkFilteredMains")]
    private class LkFilteredMainRow : Row<LkFilteredMainRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NotMapped, MinSelectLevel(SelectLevel.Details)]
        [LinkingSetRelation(typeof(LkFilteredLinkRow), "MasterID", "ItemID", FilterField = "KindID", FilterValue = 1)]
        public List<int> SelectedItems { get => fields.SelectedItems[this]; set => fields.SelectedItems[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public ListField<int> SelectedItems;
#pragma warning restore CS0649
        }
    }

    [TableName("LkNullFilterMains")]
    private class LkNullFilterMainRow : Row<LkNullFilterMainRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NotMapped, MinSelectLevel(SelectLevel.Details)]
        [LinkingSetRelation(typeof(LkFilteredLinkRow), "MasterID", "ItemID", FilterField = "KindID")]
        public List<int> SelectedItems { get => fields.SelectedItems[this]; set => fields.SelectedItems[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public ListField<int> SelectedItems;
#pragma warning restore CS0649
        }
    }

    [TableName("LkOrderMains")]
    private class LkOrderMainRow : Row<LkOrderMainRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NotMapped, MinSelectLevel(SelectLevel.Details)]
        [LinkingSetRelation(typeof(LkLinkRow), "MasterID", "ItemID", PreserveOrder = true)]
        public List<int> SelectedItems { get => fields.SelectedItems[this]; set => fields.SelectedItems[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public ListField<int> SelectedItems;
#pragma warning restore CS0649
        }
    }

    [TableName("LkNoEqualityMains")]
    private class LkNoEqualityMainRow : Row<LkNoEqualityMainRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NotMapped, MinSelectLevel(SelectLevel.Details)]
        [LinkingSetRelation(typeof(LkLinkRow), "MasterID", "ItemID", HandleEqualityFilter = false)]
        public List<int> SelectedItems { get => fields.SelectedItems[this]; set => fields.SelectedItems[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public ListField<int> SelectedItems;
#pragma warning restore CS0649
        }
    }

    [TableName("LkNonUpdatableMains")]
    private class LkNonUpdatableMainRow : Row<LkNonUpdatableMainRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NotMapped, Updatable(false), MinSelectLevel(SelectLevel.Details)]
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

    [TableName("LkSoftDeleteMains")]
    private class LkSoftDeleteMainRow : Row<LkSoftDeleteMainRow.RowFields>, IIdRow, IIsDeletedRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public bool? IsDeleted { get => fields.IsDeleted[this]; set => fields.IsDeleted[this] = value; }
        public BooleanField IsDeletedField => fields.IsDeleted;

        [NotMapped, MinSelectLevel(SelectLevel.Details)]
        [LinkingSetRelation(typeof(LkLinkRow), "MasterID", "ItemID")]
        public List<int> SelectedItems { get => fields.SelectedItems[this]; set => fields.SelectedItems[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public BooleanField IsDeleted;
            public ListField<int> SelectedItems;
#pragma warning restore CS0649
        }
    }

    [TableName("LkNotIdMains")]
    private class LkNotIdMainRow : Row<LkNotIdMainRow.RowFields>
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

    [TableName("LkWrongListMains")]
    private class LkWrongListMainRow : Row<LkWrongListMainRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NotMapped, MinSelectLevel(SelectLevel.Details)]
        [LinkingSetRelation(typeof(LkLinkRow), "MasterID", "ItemID")]
        public int? SelectedItems { get => fields.SelectedItems[this]; set => fields.SelectedItems[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public Int32Field SelectedItems;
#pragma warning restore CS0649
        }
    }

    [TableName("LkNoIdLinks")]
    private class LkNoIdLinkRow : Row<LkNoIdLinkRow.RowFields>
    {
        public int? MasterID { get => fields.MasterID[this]; set => fields.MasterID[this] = value; }
        public int? ItemID { get => fields.ItemID[this]; set => fields.ItemID[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field MasterID;
            public Int32Field ItemID;
#pragma warning restore CS0649
        }
    }

    [TableName("LkErrorMains")]
    private class LkErrorMainRow : Row<LkErrorMainRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NotMapped, MinSelectLevel(SelectLevel.Details)]
        [LinkingSetRelation(typeof(string), "MasterID", "ItemID")]
        public List<int> BadRowType { get => fields.BadRowType[this]; set => fields.BadRowType[this] = value; }

        [NotMapped, MinSelectLevel(SelectLevel.Details)]
        [LinkingSetRelation(typeof(LkNoIdLinkRow), "MasterID", "ItemID")]
        public List<int> NotIdRowType { get => fields.NotIdRowType[this]; set => fields.NotIdRowType[this] = value; }

        [NotMapped, MinSelectLevel(SelectLevel.Details)]
        [LinkingSetRelation(typeof(LkLinkRow), "Nope", "ItemID")]
        public List<int> MissingThisKey { get => fields.MissingThisKey[this]; set => fields.MissingThisKey[this] = value; }

        [NotMapped, MinSelectLevel(SelectLevel.Details)]
        [LinkingSetRelation(typeof(LkLinkRow), "MasterID", "Nope")]
        public List<int> MissingItemKey { get => fields.MissingItemKey[this]; set => fields.MissingItemKey[this] = value; }

        [NotMapped, MinSelectLevel(SelectLevel.Details)]
        [LinkingSetRelation(typeof(LkLinkRow), "MasterID", "ItemID", FilterField = "Nope")]
        public List<int> MissingFilterField { get => fields.MissingFilterField[this]; set => fields.MissingFilterField[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public ListField<int> BadRowType;
            public ListField<int> NotIdRowType;
            public ListField<int> MissingThisKey;
            public ListField<int> MissingItemKey;
            public ListField<int> MissingFilterField;
#pragma warning restore CS0649
        }
    }

    private static LinkingSetRelationBehavior CreateBehavior<TMain>(TMain row, Field target,
        IDefaultHandlerFactory? factory = null) where TMain : IRow
    {
        var behavior = new LinkingSetRelationBehavior(factory ?? new MockHandlerFactory()) { Target = target };
        Assert.True(behavior.ActivateFor(row));
        return behavior;
    }

    [Fact]
    public void ActivateFor_ReturnsFalse_WhenTargetNull()
    {
        var behavior = new LinkingSetRelationBehavior(new MockHandlerFactory());
        Assert.False(behavior.ActivateFor(new LkMainRow()));
    }

    [Fact]
    public void ActivateFor_ReturnsFalse_WhenFieldHasNoAttribute()
    {
        var row = new LkMainRow();
        var behavior = new LinkingSetRelationBehavior(new MockHandlerFactory())
        {
            Target = row.GetFields().ID
        };
        Assert.False(behavior.ActivateFor(row));
    }

    [Fact]
    public void ActivateFor_Throws_WhenRowIsNotIdRow()
    {
        var row = new LkNotIdMainRow();
        var behavior = new LinkingSetRelationBehavior(new MockHandlerFactory())
        {
            Target = row.GetFields().SelectedItems
        };
        Assert.Throws<ArgumentException>(() => behavior.ActivateFor(row));
    }

    [Fact]
    public void ActivateFor_Throws_WhenPropertyIsNotGenericList()
    {
        var row = new LkWrongListMainRow();
        var behavior = new LinkingSetRelationBehavior(new MockHandlerFactory())
        {
            Target = row.GetFields().SelectedItems
        };
        Assert.Throws<ArgumentException>(() => behavior.ActivateFor(row));
    }

    [Theory]
    [InlineData("BadRowType")]
    [InlineData("NotIdRowType")]
    [InlineData("MissingThisKey")]
    [InlineData("MissingItemKey")]
    [InlineData("MissingFilterField")]
    public void ActivateFor_Throws_ForInvalidRelationDefinitions(string fieldName)
    {
        var row = new LkErrorMainRow();
        var field = row.GetFields().FindField(fieldName)!;
        var behavior = new LinkingSetRelationBehavior(new MockHandlerFactory()) { Target = field };
        Assert.Throws<ArgumentException>(() => behavior.ActivateFor(row));
    }

    [Fact]
    public void ActivateFor_WithFilterValue_EnablesFilteredQueries()
    {
        var row = new LkFilteredMainRow { ID = 1 };
        var deleted = new List<object?>();
        var factory = new MockHandlerFactory((_, intf) =>
            new MockDeleteHandler<LkFilteredLinkRow>(x => deleted.Add(x.Request?.EntityId)));

        var behavior = CreateBehavior(row, row.GetFields().SelectedItems, factory);
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(new { ID = 100L }));

        behavior.OnBeforeDelete(new MockDeleteHandler<LkFilteredMainRow>
        {
            Row = row,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        });

        Assert.Equal([100L], deleted);
        Assert.Contains("KindID", connection.ExecuteReaderCalls[0].Query!.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void ActivateFor_WithNullFilterValue_UsesIsNull()
    {
        var row = new LkNullFilterMainRow { ID = 1 };
        var factory = new MockHandlerFactory((_, _) =>
            new MockDeleteHandler<LkFilteredLinkRow>());
        var behavior = CreateBehavior(row, row.GetFields().SelectedItems, factory);
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());

        behavior.OnBeforeDelete(new MockDeleteHandler<LkNullFilterMainRow>
        {
            Row = row,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        });

        Assert.Contains("KindID", connection.ExecuteReaderCalls[0].Query!.ToString(), StringComparison.Ordinal);
        Assert.Contains("IS NULL", connection.ExecuteReaderCalls[0].Query!.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void OnPrepareQuery_ReturnsEarly_WhenNoEqualityFilter()
    {
        var row = new LkMainRow { ID = 1 };
        var behavior = CreateBehavior(row, row.GetFields().SelectedItems);
        var handler = new MockListHandler<LkMainRow> { Row = row };
        var query = new SqlQuery();

        behavior.OnPrepareQuery(handler, query);

        Assert.DoesNotContain("__ls", query.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void OnPrepareQuery_ReturnsEarly_WhenEqualityFilterNotHandled()
    {
        var row = new LkNoEqualityMainRow { ID = 1 };
        var behavior = CreateBehavior(row, row.GetFields().SelectedItems);
        var handler = new MockListHandler<LkNoEqualityMainRow> { Row = row };
        handler.Request.EqualityFilter = new Dictionary<string, object?>
        {
            [nameof(LkNoEqualityMainRow.SelectedItems)] = 5
        };
        var query = new SqlQuery();

        behavior.OnPrepareQuery(handler, query);

        Assert.DoesNotContain("__ls", query.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void OnPrepareQuery_ReturnsEarly_ForEmptyValue()
    {
        var row = new LkMainRow { ID = 1 };
        var behavior = CreateBehavior(row, row.GetFields().SelectedItems);
        var handler = new MockListHandler<LkMainRow> { Row = row };
        handler.Request.EqualityFilter = new Dictionary<string, object?>
        {
            [nameof(LkMainRow.SelectedItems)] = ""
        };
        var query = new SqlQuery();

        behavior.OnPrepareQuery(handler, query);

        Assert.DoesNotContain("__ls", query.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void OnPrepareQuery_IgnoresUnknownEqualityFilter()
    {
        var row = new LkMainRow { ID = 1 };
        var behavior = CreateBehavior(row, row.GetFields().SelectedItems);
        var handler = new MockListHandler<LkMainRow> { Row = row };
        handler.Request.EqualityFilter = new Dictionary<string, object?> { ["Other"] = 5 };
        var query = new SqlQuery();

        behavior.OnPrepareQuery(handler, query);

        Assert.DoesNotContain("__ls", query.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void OnPrepareQuery_WithScalarValue_AddsExistsSubquery()
    {
        var row = new LkMainRow { ID = 1 };
        var behavior = CreateBehavior(row, row.GetFields().SelectedItems);
        var handler = new MockListHandler<LkMainRow> { Row = row };
        handler.Request.EqualityFilter = new Dictionary<string, object?>
        {
            [nameof(LkMainRow.SelectedItems)] = 5
        };
        var query = new SqlQuery();

        behavior.OnPrepareQuery(handler, query);

        Assert.Contains("__ls", query.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task OnPrepareQueryAsync_WithEnumerableValue_AddsExistsSubquery()
    {
        var row = new LkMainRow { ID = 1 };
        var behavior = CreateBehavior(row, row.GetFields().SelectedItems);
        var handler = new MockListHandler<LkMainRow> { Row = row };
        handler.Request.EqualityFilter = new Dictionary<string, object?>
        {
            [nameof(LkMainRow.SelectedItems)] = new[] { 5, 6 }
        };
        var query = new SqlQuery();

        await behavior.OnPrepareQueryAsync(handler, query, TestContext.Current.CancellationToken);

        Assert.Contains("__ls", query.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void OnReturn_List_SetsSelectedItemsPerRow()
    {
        var handlerFactory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(LkLinkRow), rowType);
            Assert.Equal(typeof(IListRequestProcessor), intf);
            return new MockListHandler<LkLinkRow>(x =>
            {
                x.Response.Entities.Add(new LkLinkRow { MasterID = 5, ItemID = 11 });
                x.Response.Entities.Add(new LkLinkRow { MasterID = 5, ItemID = 22 });
                x.Response.Entities.Add(new LkLinkRow { MasterID = 6, ItemID = 33 });
            });
        });

        var handler = new MockListHandler<LkMainRow>();
        handler.Row.ID = 5;
        handler.Response.Entities.Add(new LkMainRow { ID = 5 });
        handler.Response.Entities.Add(new LkMainRow { ID = 6 });

        var behavior = CreateBehavior(handler.Row, handler.Row.GetFields().SelectedItems, handlerFactory);
        behavior.OnReturn(handler);

        Assert.Equal([11, 22], handler.Response.Entities[0].SelectedItems);
        Assert.Equal([33], handler.Response.Entities[1].SelectedItems);
    }

    [Fact]
    public async Task OnReturnAsync_List_SetsSelectedItemsPerRow()
    {
        var handlerFactory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(LkLinkRow), rowType);
            Assert.Equal(typeof(IListRequestProcessorAsync), intf);
            return new MockListHandlerAsync<LkLinkRow>(x =>
            {
                x.Response.Entities.Add(new LkLinkRow { MasterID = 5, ItemID = 11 });
                x.Response.Entities.Add(new LkLinkRow { MasterID = 6, ItemID = 33 });
            });
        });

        var handler = new MockListHandlerAsync<LkMainRow>();
        handler.Row.ID = 5;
        handler.Response.Entities.Add(new LkMainRow { ID = 5 });
        handler.Response.Entities.Add(new LkMainRow { ID = 6 });

        var behavior = CreateBehavior(handler.Row, handler.Row.GetFields().SelectedItems, handlerFactory);
        await behavior.OnReturnAsync(handler, TestContext.Current.CancellationToken);

        Assert.Equal([11], handler.Response.Entities[0].SelectedItems);
        Assert.Equal([33], handler.Response.Entities[1].SelectedItems);
    }

    [Fact]
    public void OnReturn_List_ReturnsEarly_WhenNoEntities()
    {
        var handler = new MockListHandler<LkMainRow>();
        var behavior = CreateBehavior(handler.Row, handler.Row.GetFields().SelectedItems,
            new MockHandlerFactory((_, _) => throw new InvalidOperationException("should not create")));

        behavior.OnReturn(handler);
    }

    private class NoSelectListHandler<TRow> : MockListHandler<TRow> where TRow : IRow, new()
    {
        public override bool AllowSelectField(Field field) => false;
    }

    [Fact]
    public void OnReturn_List_ReturnsEarly_WhenFieldNotAllowed()
    {
        var handler = new NoSelectListHandler<LkMainRow>();
        handler.Response.Entities.Add(new LkMainRow { ID = 1 });
        var behavior = CreateBehavior(handler.Row, handler.Row.GetFields().SelectedItems,
            new MockHandlerFactory((_, _) => throw new InvalidOperationException("should not create")));

        behavior.OnReturn(handler);
    }

    private static MockHandlerFactory SaveDeleteFactory(List<int?> saved, List<object?> deleted, bool async)
    {
        return new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(LkLinkRow), rowType);
            if (intf == typeof(ISaveRequestProcessor))
                return new MockSaveHandler<LkLinkRow>(x => saved.Add(((LkLinkRow)x.Request.Entity!).ItemID));
            if (intf == typeof(ISaveRequestProcessorAsync))
                return new MockSaveHandlerAsync<LkLinkRow>(x => saved.Add(((LkLinkRow)x.Request.Entity!).ItemID));
            if (intf == typeof(IDeleteRequestProcessor))
                return new MockDeleteHandler<LkLinkRow>(x => deleted.Add(x.Request?.EntityId));
            return new MockDeleteHandlerAsync<LkLinkRow>(x => deleted.Add(x.Request?.EntityId));
        });
    }

    [Fact]
    public void OnAfterSave_Update_WithNoOldRows_InsertsAll()
    {
        var saved = new List<int?>();
        var deleted = new List<object?>();
        var factory = SaveDeleteFactory(saved, deleted, false);
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());
        var master = new LkMainRow { ID = 5, SelectedItems = [11, 22] };
        var behavior = CreateBehavior(master, master.GetFields().SelectedItems, factory);

        behavior.OnAfterSave(CreateSaveHandler(connection, false, master));

        Assert.Equal([11, 22], saved);
        Assert.Empty(deleted);
    }

    [Fact]
    public async Task OnAfterSaveAsync_Update_WithNoOldRows_InsertsAll()
    {
        var saved = new List<int?>();
        var deleted = new List<object?>();
        var factory = SaveDeleteFactory(saved, deleted, true);
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());
        var master = new LkMainRow { ID = 5, SelectedItems = [11, 22] };
        var behavior = CreateBehavior(master, master.GetFields().SelectedItems, factory);

        await behavior.OnAfterSaveAsync(CreateSaveHandlerAsync(connection, false, master),
            TestContext.Current.CancellationToken);

        Assert.Equal([11, 22], saved);
        Assert.Empty(deleted);
    }

    [Fact]
    public void OnAfterSave_Update_WithEmptyNewList_DeletesOldRows()
    {
        var saved = new List<int?>();
        var deleted = new List<object?>();
        var factory = SaveDeleteFactory(saved, deleted, false);
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(new { ID = 100L, ItemID = 11 }));
        var master = new LkMainRow { ID = 5, SelectedItems = [] };
        var behavior = CreateBehavior(master, master.GetFields().SelectedItems, factory);

        behavior.OnAfterSave(CreateSaveHandler(connection, false, master));

        Assert.Equal([100L], deleted);
        Assert.Empty(saved);
    }

    [Fact]
    public void OnAfterSave_Update_DeletesRemovedAndInsertsNew()
    {
        var saved = new List<int?>();
        var deleted = new List<object?>();
        var factory = SaveDeleteFactory(saved, deleted, false);
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(new { ID = 100L, ItemID = 11 }));
        var master = new LkMainRow { ID = 5, SelectedItems = [22] };
        var behavior = CreateBehavior(master, master.GetFields().SelectedItems, factory);

        behavior.OnAfterSave(CreateSaveHandler(connection, false, master));

        Assert.Equal([100L], deleted);
        Assert.Equal([22], saved);
    }

    [Fact]
    public void OnAfterSave_Update_PreserveOrder_RebuildsWhenOrderAndMembershipChange()
    {
        var saved = new List<int?>();
        var deleted = new List<object?>();
        var factory = SaveDeleteFactory(saved, deleted, false);
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(
                new { ID = 100L, ItemID = 11 },
                new { ID = 200L, ItemID = 22 }));
        var master = new LkOrderMainRow { ID = 5, SelectedItems = [33, 22] };
        var behavior = CreateBehavior(master, master.GetFields().SelectedItems, factory);

        behavior.OnAfterSave(CreateSaveHandler(connection, false, master));

        Assert.Equal([100L, 200L], deleted);
        Assert.Equal([33, 22], saved);
    }

    [Fact]
    public void OnAfterSave_Create_IgnoresNullList()
    {
        using var connection = new MockDbConnection();
        var master = new LkMainRow { ID = 5 };
        var behavior = CreateBehavior(master, master.GetFields().SelectedItems,
            new MockHandlerFactory((_, _) => throw new InvalidOperationException("should not create")));

        behavior.OnAfterSave(CreateSaveHandler(connection, true, master));
    }

    [Fact]
    public void OnBeforeDelete_ReturnsEarly_WhenFieldNotUpdatable()
    {
        var master = new LkNonUpdatableMainRow { ID = 5 };
        var invoked = false;
        var behavior = CreateBehavior(master, master.GetFields().SelectedItems,
            new MockHandlerFactory((_, _) => { invoked = true; throw new InvalidOperationException(); }));
        using var connection = new MockDbConnection();

        behavior.OnBeforeDelete(new MockDeleteHandler<LkNonUpdatableMainRow>
        {
            Row = master,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        });

        Assert.False(invoked);
    }

    [Fact]
    public void OnBeforeDelete_ReturnsEarly_ForSoftDeleteRows()
    {
        var master = new LkSoftDeleteMainRow { ID = 5 };
        var invoked = false;
        var behavior = CreateBehavior(master, master.GetFields().SelectedItems,
            new MockHandlerFactory((_, _) => { invoked = true; throw new InvalidOperationException(); }));
        using var connection = new MockDbConnection();

        behavior.OnBeforeDelete(new MockDeleteHandler<LkSoftDeleteMainRow>
        {
            Row = master,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        });

        Assert.False(invoked);
    }
}
