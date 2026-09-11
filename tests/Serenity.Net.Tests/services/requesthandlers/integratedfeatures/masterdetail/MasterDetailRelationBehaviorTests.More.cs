#pragma warning disable CS0649
namespace Serenity.Services;

public partial class MasterDetailRelationBehaviorTests
{
    [TableName("MdNoChangeMasters")]
    private class NoChangeCheckMasterRow : Row<NoChangeCheckMasterRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NotMapped, MinSelectLevel(SelectLevel.Details)]
        [MasterDetailRelation(foreignKey: "MasterID", CheckChangesOnUpdate = false)]
        public List<Int32DetailRow> DetailList { get => fields.DetailList[this]; set => fields.DetailList[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID;
            public RowListField<Int32DetailRow> DetailList;
        }
    }

    [TableName("MdNonUpdatableMasters")]
    private class NonUpdatableMasterRow : Row<NonUpdatableMasterRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NotMapped, Updatable(false), MinSelectLevel(SelectLevel.Details)]
        [MasterDetailRelation(foreignKey: "MasterID")]
        public List<Int32DetailRow> DetailList { get => fields.DetailList[this]; set => fields.DetailList[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID;
            public RowListField<Int32DetailRow> DetailList;
        }
    }

    [TableName("MdSoftDeleteMasters")]
    private class SoftDeleteMasterRow : Row<SoftDeleteMasterRow.RowFields>, IIdRow, IIsDeletedRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public bool? IsDeleted { get => fields.IsDeleted[this]; set => fields.IsDeleted[this] = value; }
        public BooleanField IsDeletedField => fields.IsDeleted;

        [NotMapped, MinSelectLevel(SelectLevel.Details)]
        [MasterDetailRelation(foreignKey: "MasterID")]
        public List<Int32DetailRow> DetailList { get => fields.DetailList[this]; set => fields.DetailList[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID;
            public BooleanField IsDeleted;
            public RowListField<Int32DetailRow> DetailList;
        }
    }

    [TableName("MdAltKeyMasters")]
    private class AltKeyMasterRow : Row<AltKeyMasterRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NotNull]
        public string Code { get => fields.Code[this]; set => fields.Code[this] = value; }

        [NotMapped, MinSelectLevel(SelectLevel.Details)]
        [MasterDetailRelation(foreignKey: "MasterID", MasterKeyField = "Code")]
        public List<Int32DetailRow> DetailList { get => fields.DetailList[this]; set => fields.DetailList[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID;
            public StringField Code;
            public RowListField<Int32DetailRow> DetailList;
        }
    }

    [TableName("MdFilterMasters")]
    private class FilterMasterRow : Row<FilterMasterRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NotMapped, MinSelectLevel(SelectLevel.Details)]
        [MasterDetailRelation(foreignKey: "MasterID", FilterField = "ProductID", FilterValue = 1)]
        public List<Int32DetailRow> DetailList { get => fields.DetailList[this]; set => fields.DetailList[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID;
            public RowListField<Int32DetailRow> DetailList;
        }
    }

    [TableName("MdNullFilterMasters")]
    private class NullFilterMasterRow : Row<NullFilterMasterRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NotMapped, MinSelectLevel(SelectLevel.Details)]
        [MasterDetailRelation(foreignKey: "MasterID", FilterField = "ProductID")]
        public List<Int32DetailRow> DetailList { get => fields.DetailList[this]; set => fields.DetailList[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID;
            public RowListField<Int32DetailRow> DetailList;
        }
    }

    [TableName("MdIncludeMasters")]
    private class IncludeMasterRow : Row<IncludeMasterRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NotMapped, MinSelectLevel(SelectLevel.Details)]
        [MasterDetailRelation(foreignKey: "MasterID",
            IncludeColumns = "ProductID, Quantity",
            IncludeColumnNames = ["DetailID"],
            ColumnsType = typeof(AdditionalColumns))]
        public List<Int32DetailRow> DetailList { get => fields.DetailList[this]; set => fields.DetailList[this] = value; }

        public class AdditionalColumns
        {
            public int? MasterID { get; set; }
            public string? Extra { get; set; }
        }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID;
            public RowListField<Int32DetailRow> DetailList;
        }
    }

    [TableName("MdErrorMasters")]
    private class ErrorMasterRow : Row<ErrorMasterRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NotMapped]
        [MasterDetailRelation(foreignKey: "MasterID")]
        public int? NotAList { get => fields.NotAList[this]; set => fields.NotAList[this] = value; }

        [NotMapped]
        [MasterDetailRelation(foreignKey: "MasterID")]
        public List<int> NotRows { get => fields.NotRows[this]; set => fields.NotRows[this] = value; }

        [NotMapped]
        [MasterDetailRelation(foreignKey: "Nope")]
        public List<Int32DetailRow> MissingFk { get => fields.MissingFk[this]; set => fields.MissingFk[this] = value; }

        [NotMapped]
        [MasterDetailRelation(foreignKey: "MasterID", FilterField = "Nope")]
        public List<Int32DetailRow> MissingFilter { get => fields.MissingFilter[this]; set => fields.MissingFilter[this] = value; }

        [NotMapped]
        [MasterDetailRelation(foreignKey: "MasterID", MasterKeyField = "Nope")]
        public List<Int32DetailRow> MissingMasterKey { get => fields.MissingMasterKey[this]; set => fields.MissingMasterKey[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID;
            public Int32Field NotAList;
            public ListField<int> NotRows;
            public RowListField<Int32DetailRow> MissingFk;
            public RowListField<Int32DetailRow> MissingFilter;
            public RowListField<Int32DetailRow> MissingMasterKey;
        }
    }

    private class NoSelectRetrieveHandler<TRow> : MockRetrieveHandler<TRow> where TRow : IRow, new()
    {
        public override bool AllowSelectField(Field field) => false;
    }

    private static MasterDetailRelationBehavior Activate<TMain>(TMain row, Field target,
        IDefaultHandlerFactory factory) where TMain : IRow
    {
        var behavior = new MasterDetailRelationBehavior(factory) { Target = target };
        Assert.True(behavior.ActivateFor(row));
        return behavior;
    }

    [Fact]
    public void ActivateFor_ReturnsFalse_WhenTargetNull()
    {
        var behavior = new MasterDetailRelationBehavior(new MockHandlerFactory());
        Assert.False(behavior.ActivateFor(new Int32MasterRow()));
    }

    [Fact]
    public void ActivateFor_ReturnsFalse_WhenFieldHasNoAttribute()
    {
        var row = new Int32MasterRow();
        var behavior = new MasterDetailRelationBehavior(new MockHandlerFactory())
        {
            Target = row.GetFields().ID
        };
        Assert.False(behavior.ActivateFor(row));
    }

    [Theory]
    [InlineData("NotAList")]
    [InlineData("NotRows")]
    [InlineData("MissingFk")]
    [InlineData("MissingFilter")]
    [InlineData("MissingMasterKey")]
    public void ActivateFor_Throws_ForInvalidRelationDefinitions(string fieldName)
    {
        var row = new ErrorMasterRow();
        var field = row.GetFields().FindField(fieldName)!;
        var behavior = new MasterDetailRelationBehavior(new MockHandlerFactory()) { Target = field };
        Assert.Throws<ArgumentException>(() => behavior.ActivateFor(row));
    }

    [Fact]
    public void OnReturn_Retrieve_ReturnsEarly_WhenFieldNotAllowed()
    {
        var handler = new NoSelectRetrieveHandler<Int32MasterRow>();
        handler.Row.ID = 1;
        var behavior = Activate(handler.Row, handler.Row.GetFields().DetailList,
            new MockHandlerFactory((_, _) => throw new InvalidOperationException("should not create")));

        behavior.OnReturn(handler);

        Assert.Null(handler.Row.DetailList);
    }

    [Fact]
    public void OnReturn_Retrieve_UsesMasterKeyFieldAndFilterAndIncludeColumns()
    {
        ListRequest? captured = null;
        var factory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(Int32DetailRow), rowType);
            Assert.Equal(typeof(IListRequestProcessor), intf);
            return new MockListHandler<Int32DetailRow>(x => captured = x.Request);
        });

        var include = new IncludeMasterRow { ID = 1 };
        var behavior = Activate(include, include.GetFields().DetailList, factory);
        behavior.OnReturn(new MockRetrieveHandler<IncludeMasterRow> { Row = include });

        Assert.NotNull(captured);
        Assert.Contains("ProductID", captured!.IncludeColumns!);
        Assert.Contains("Quantity", captured.IncludeColumns!);
        Assert.Contains("DetailID", captured.IncludeColumns!);
        Assert.Contains("MasterID", captured.IncludeColumns!);
        Assert.Contains("Extra", captured.IncludeColumns!);
    }

    [Fact]
    public void OnReturn_Retrieve_CustomMasterKeyField_UsesItInCriteria()
    {
        ListRequest? captured = null;
        var factory = new MockHandlerFactory((_, _) =>
            new MockListHandler<Int32DetailRow>(x => captured = x.Request));

        var master = new AltKeyMasterRow { ID = 1, Code = "ABC" };
        var behavior = Activate(master, master.GetFields().DetailList, factory);
        behavior.OnReturn(new MockRetrieveHandler<AltKeyMasterRow> { Row = master });

        var binary = Assert.IsType<BinaryCriteria>(captured!.Criteria);
        Assert.Equal(CriteriaOperator.EQ, binary.Operator);
        Assert.Equal("MasterID", Assert.IsType<Criteria>(binary.LeftOperand).Expression);
        Assert.Equal("ABC", Assert.IsType<ValueCriteria>(binary.RightOperand).Value);
    }

    [Fact]
    public void OnReturn_Retrieve_FilterValue_AddsFilterCriteria()
    {
        ListRequest? captured = null;
        var factory = new MockHandlerFactory((_, _) =>
            new MockListHandler<Int32DetailRow>(x => captured = x.Request));

        var master = new FilterMasterRow { ID = 1 };
        var behavior = Activate(master, master.GetFields().DetailList, factory);
        behavior.OnReturn(new MockRetrieveHandler<FilterMasterRow> { Row = master });

        var and = Assert.IsType<BinaryCriteria>(captured!.Criteria);
        Assert.Equal(CriteriaOperator.AND, and.Operator);
        var filterEq = Assert.IsType<BinaryCriteria>(and.RightOperand);
        Assert.Equal(CriteriaOperator.EQ, filterEq.Operator);
        Assert.Equal("ProductID", Assert.IsType<Criteria>(filterEq.LeftOperand).Expression);
        Assert.Equal(1, Assert.IsType<ValueCriteria>(filterEq.RightOperand).Value);
    }

    [Fact]
    public void OnReturn_Retrieve_NullFilterValue_AddsIsNullCriteria()
    {
        ListRequest? captured = null;
        var factory = new MockHandlerFactory((_, _) =>
            new MockListHandler<Int32DetailRow>(x => captured = x.Request));

        var master = new NullFilterMasterRow { ID = 1 };
        var behavior = Activate(master, master.GetFields().DetailList, factory);
        behavior.OnReturn(new MockRetrieveHandler<NullFilterMasterRow> { Row = master });

        var and = Assert.IsType<BinaryCriteria>(captured!.Criteria);
        Assert.Equal(CriteriaOperator.AND, and.Operator);
        var unary = Assert.IsType<UnaryCriteria>(and.RightOperand);
        Assert.Equal(CriteriaOperator.IsNull, unary.Operator);
        Assert.Equal("ProductID", Assert.IsType<Criteria>(unary.Operand).Expression);
    }

    [Fact]
    public void OnReturn_List_FillsEachMasterDetailList()
    {
        var factory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(Int32DetailRow), rowType);
            Assert.Equal(typeof(IListRequestProcessor), intf);
            return new MockListHandler<Int32DetailRow>(x =>
            {
                x.Response.Entities.Add(new Int32DetailRow { DetailID = 1, MasterID = 7 });
                x.Response.Entities.Add(new Int32DetailRow { DetailID = 2, MasterID = 8 });
            });
        });

        var handler = new MockListHandler<Int32MasterRow>();
        handler.Response.Entities.Add(new Int32MasterRow { ID = 7 });
        handler.Response.Entities.Add(new Int32MasterRow { ID = 8 });
        var behavior = Activate(handler.Row, handler.Row.GetFields().DetailList, factory);

        behavior.OnReturn(handler);

        Assert.Single(handler.Response.Entities[0].DetailList);
        Assert.Equal(1, handler.Response.Entities[0].DetailList[0].DetailID);
        Assert.Single(handler.Response.Entities[1].DetailList);
        Assert.Equal(2, handler.Response.Entities[1].DetailList[0].DetailID);
    }

    [Fact]
    public async Task OnReturnAsync_List_FillsEachMasterDetailList()
    {
        var factory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(Int32DetailRow), rowType);
            Assert.Equal(typeof(IListRequestProcessorAsync), intf);
            return new MockListHandlerAsync<Int32DetailRow>(x =>
            {
                x.Response.Entities.Add(new Int32DetailRow { DetailID = 1, MasterID = 7 });
                x.Response.Entities.Add(new Int32DetailRow { DetailID = 2, MasterID = 8 });
            });
        });

        var handler = new MockListHandlerAsync<Int32MasterRow>();
        handler.Response.Entities.Add(new Int32MasterRow { ID = 7 });
        handler.Response.Entities.Add(new Int32MasterRow { ID = 8 });
        var behavior = Activate(handler.Row, handler.Row.GetFields().DetailList, factory);

        await behavior.OnReturnAsync(handler, TestContext.Current.CancellationToken);

        Assert.Single(handler.Response.Entities[0].DetailList);
        Assert.Equal(2, handler.Response.Entities[1].DetailList[0].DetailID);
    }

    [Fact]
    public void OnReturn_List_ReturnsEarly_WhenNoEntities()
    {
        var handler = new MockListHandler<Int32MasterRow>();
        var behavior = Activate(handler.Row, handler.Row.GetFields().DetailList,
            new MockHandlerFactory((_, _) => throw new InvalidOperationException("should not create")));

        behavior.OnReturn(handler);
    }

    [Fact]
    public void OnAfterSave_Create_ReturnsEarly_ForNullList()
    {
        using var connection = new MockDbConnection();
        var master = new Int32MasterRow { ID = 7, Name = "M" };
        var behavior = Activate(master, master.GetFields().DetailList,
            new MockHandlerFactory((_, _) => throw new InvalidOperationException("should not create")));

        behavior.OnAfterSave(CreateMasterSaveHandler(connection, true, master));
    }

    [Fact]
    public void OnAfterSave_Update_WithNoOldRows_InsertsAll()
    {
        var saved = new List<SaveRequestType>();
        var factory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(Int32DetailRow), rowType);
            if (intf == typeof(IListRequestProcessor))
                return new MockListHandler<Int32DetailRow>();
            return new MockSaveHandler<Int32DetailRow>(x => saved.Add(x.RequestType));
        });

        using var connection = new MockDbConnection();
        var master = new Int32MasterRow { ID = 7, Name = "M" };
        var behavior = Activate(master, master.GetFields().DetailList, factory);
        master.DetailList = [new Int32DetailRow { ProductID = 1, Quantity = 1m }];

        behavior.OnAfterSave(CreateMasterSaveHandler(connection, false, master));

        Assert.Equal([SaveRequestType.Create], saved);
    }

    [Fact]
    public void OnAfterSave_Update_WithEmptyNewList_DeletesOld()
    {
        var deleted = new List<int?>();
        var factory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(Int32DetailRow), rowType);
            if (intf == typeof(IListRequestProcessor))
                return new MockListHandler<Int32DetailRow>(x =>
                {
                    x.Response.Entities.Add(new Int32DetailRow { DetailID = 10 });
                    x.Response.Entities.Add(new Int32DetailRow { DetailID = 20 });
                });
            return new MockDeleteHandler<Int32DetailRow>(x => deleted.Add(x.Request?.EntityId as int?));
        });

        using var connection = new MockDbConnection();
        var master = new Int32MasterRow { ID = 7, Name = "M" };
        var behavior = Activate(master, master.GetFields().DetailList, factory);
        master.DetailList = [];

        behavior.OnAfterSave(CreateMasterSaveHandler(connection, false, master));

        Assert.Equal([10, 20], deleted);
    }

    [Fact]
    public void OnAfterSave_Update_WithChangedDetail_UpdatesIt()
    {
        var calls = new List<(SaveRequestType type, int? id)>();
        var factory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(Int32DetailRow), rowType);
            if (intf == typeof(IListRequestProcessor))
                return new MockListHandler<Int32DetailRow>(x =>
                {
                    x.Response.Entities.Add(new Int32DetailRow
                    {
                        DetailID = 100, MasterID = 7, ProductID = 1, Quantity = 1m
                    });
                });
            return new MockSaveHandler<Int32DetailRow>(x =>
                calls.Add((x.RequestType, (x.Request?.Entity as Int32DetailRow)?.DetailID)));
        });

        using var connection = new MockDbConnection();
        var master = new Int32MasterRow { ID = 7, Name = "M" };
        var behavior = Activate(master, master.GetFields().DetailList, factory);
        master.DetailList =
        [
            new Int32DetailRow { DetailID = 100, MasterID = 7, ProductID = 1, Quantity = 9m }
        ];

        behavior.OnAfterSave(CreateMasterSaveHandler(connection, false, master));

        Assert.Equal([(SaveRequestType.Update, 100)], calls);
    }

    [Fact]
    public void OnAfterSave_Update_WithNewDetail_InsertsIt()
    {
        var calls = new List<(SaveRequestType type, int? id)>();
        var factory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(Int32DetailRow), rowType);
            if (intf == typeof(IListRequestProcessor))
                return new MockListHandler<Int32DetailRow>(x =>
                {
                    x.Response.Entities.Add(new Int32DetailRow
                    {
                        DetailID = 100, MasterID = 7, ProductID = 1, Quantity = 1m
                    });
                });
            return new MockSaveHandler<Int32DetailRow>(x =>
                calls.Add((x.RequestType, (x.Request?.Entity as Int32DetailRow)?.DetailID)));
        });

        using var connection = new MockDbConnection();
        var master = new Int32MasterRow { ID = 7, Name = "M" };
        var behavior = Activate(master, master.GetFields().DetailList, factory);
        master.DetailList =
        [
            new Int32DetailRow { DetailID = 100, MasterID = 7, ProductID = 1, Quantity = 1m },
            new Int32DetailRow { MasterID = 7, ProductID = 2, Quantity = 2m }
        ];

        behavior.OnAfterSave(CreateMasterSaveHandler(connection, false, master));

        Assert.Equal([(SaveRequestType.Create, null)], calls);
    }

    [Fact]
    public void OnAfterSave_Update_NoChangeCheck_UpdatesMatchedRows()
    {
        var calls = new List<(SaveRequestType type, int? id)>();
        var factory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(Int32DetailRow), rowType);
            Assert.Equal(typeof(ISaveRequestProcessor), intf);
            return new MockSaveHandler<Int32DetailRow>(x =>
                calls.Add((x.RequestType, (x.Request?.Entity as Int32DetailRow)?.DetailID)));
        });

        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { DetailID = 100 }, new { DetailID = 200 }));
        var master = new NoChangeCheckMasterRow { ID = 7 };
        var behavior = Activate(master, master.GetFields().DetailList, factory);
        master.DetailList =
        [
            new Int32DetailRow { DetailID = 100, MasterID = 7, ProductID = 1, Quantity = 1m },
            new Int32DetailRow { DetailID = 200, MasterID = 7, ProductID = 2, Quantity = 2m }
        ];

        behavior.OnAfterSave(CreateSaveHandlerFor(connection, false, master));

        Assert.Equal(
            [(SaveRequestType.Update, 100), (SaveRequestType.Update, 200)],
            calls);
    }

    [Fact]
    public async Task OnAfterSaveAsync_Update_NoChangeCheck_UpdatesMatchedRows()
    {
        var calls = new List<(SaveRequestType type, int? id)>();
        var factory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(Int32DetailRow), rowType);
            Assert.Equal(typeof(ISaveRequestProcessorAsync), intf);
            return new MockSaveHandlerAsync<Int32DetailRow>(x =>
                calls.Add((x.RequestType, (x.Request?.Entity as Int32DetailRow)?.DetailID)));
        });

        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { DetailID = 100 }));
        var master = new NoChangeCheckMasterRow { ID = 7 };
        var behavior = Activate(master, master.GetFields().DetailList, factory);
        master.DetailList = [new Int32DetailRow { DetailID = 100, MasterID = 7, ProductID = 1, Quantity = 1m }];

        await behavior.OnAfterSaveAsync(CreateSaveHandlerForAsync(connection, false, master),
            TestContext.Current.CancellationToken);

        Assert.Equal([(SaveRequestType.Update, 100)], calls);
    }

    [Fact]
    public void OnBeforeDelete_ReturnsEarly_WhenFieldNotUpdatable()
    {
        var master = new NonUpdatableMasterRow { ID = 5 };
        var invoked = false;
        var behavior = Activate(master, master.GetFields().DetailList,
            new MockHandlerFactory((_, _) => { invoked = true; throw new InvalidOperationException(); }));
        using var connection = new MockDbConnection();

        behavior.OnBeforeDelete(new MockDeleteHandler<NonUpdatableMasterRow>
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
        var master = new SoftDeleteMasterRow { ID = 5 };
        var invoked = false;
        var behavior = Activate(master, master.GetFields().DetailList,
            new MockHandlerFactory((_, _) => { invoked = true; throw new InvalidOperationException(); }));
        using var connection = new MockDbConnection();

        behavior.OnBeforeDelete(new MockDeleteHandler<SoftDeleteMasterRow>
        {
            Row = master,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        });

        Assert.False(invoked);
    }

    private static MockSaveHandler<TRow> CreateSaveHandlerFor<TRow>(MockDbConnection connection, bool isCreate, TRow row)
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

    private static MockSaveHandlerAsync<TRow> CreateSaveHandlerForAsync<TRow>(MockDbConnection connection, bool isCreate, TRow row)
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
}


