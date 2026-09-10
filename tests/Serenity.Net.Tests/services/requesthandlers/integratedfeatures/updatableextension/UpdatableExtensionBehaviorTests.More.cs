namespace Serenity.Services;

public class UpdatableExtensionBehaviorTests_More
{
    [TableName("ExtRows")]
    public class ExtRow : Row<ExtRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [NotNull]
        public string Text { get => fields.Text[this]; set => fields.Text[this] = value; }
        public int? Kind { get => fields.Kind[this]; set => fields.Kind[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public StringField Text;
            public Int32Field Kind;
#pragma warning restore CS0649
        }
    }

    [TableName("NonIdExtRows")]
    public class NonIdExtRow : Row<NonIdExtRow.RowFields>
    {
        public string Text { get => fields.Text[this]; set => fields.Text[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public StringField Text;
#pragma warning restore CS0649
        }
    }

    [TableName("FilterExtRows")]
    public class FilterExtRow : Row<FilterExtRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public int? DetailId { get => fields.DetailId[this]; set => fields.DetailId[this] = value; }
        public string Text { get => fields.Text[this]; set => fields.Text[this] = value; }
        public int? Kind { get => fields.Kind[this]; set => fields.Kind[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public Int32Field DetailId;
            public StringField Text;
            public Int32Field Kind;
#pragma warning restore CS0649
        }
    }

    [TableName("NoAttrMains")]
    public class NoAttrMainRow : Row<NoAttrMainRow.RowFields>, IIdRow
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

    [TableName("BadRowTypeMains")]
    [UpdatableExtension("d", typeof(string))]
    public class BadRowTypeMainRow : Row<BadRowTypeMainRow.RowFields>, IIdRow
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

    [TableName("NotIdThisKeyMains")]
    [UpdatableExtension("d", typeof(ExtRow))]
    public class NotIdThisKeyMainRow : Row<NotIdThisKeyMainRow.RowFields>
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [Expression("d.Text")]
        public string DetailText { get => fields.DetailText[this]; set => fields.DetailText[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public StringField DetailText;
#pragma warning restore CS0649
        }
    }

    [TableName("MissingThisKeyMains")]
    [UpdatableExtension("d", typeof(ExtRow), ThisKey = "Nope")]
    public class MissingThisKeyMainRow : Row<MissingThisKeyMainRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [Expression("d.Text")]
        public string DetailText { get => fields.DetailText[this]; set => fields.DetailText[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public StringField DetailText;
#pragma warning restore CS0649
        }
    }

    [TableName("MissingOtherKeyMains")]
    [UpdatableExtension("d", typeof(NonIdExtRow), ThisKey = "DetailId")]
    public class MissingOtherKeyMainRow : Row<MissingOtherKeyMainRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
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

    [TableName("MissingFilterMains")]
    [UpdatableExtension("d", typeof(FilterExtRow), ThisKey = "DetailId", FilterField = "Nope")]
    public class MissingFilterMainRow : Row<MissingFilterMainRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [ForeignKey(typeof(FilterExtRow)), LeftJoin("d")]
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

    [TableName("MissingPresenceMains")]
    [UpdatableExtension("d", typeof(FilterExtRow), ThisKey = "DetailId", PresenceField = "Nope")]
    public class MissingPresenceMainRow : Row<MissingPresenceMainRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [ForeignKey(typeof(FilterExtRow)), LeftJoin("d")]
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

    [TableName("NoMatchMains")]
    [UpdatableExtension("d", typeof(ExtRow), ThisKey = "DetailId")]
    public class NoMatchMainRow : Row<NoMatchMainRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [ForeignKey(typeof(ExtRow)), LeftJoin("d")]
        public int? DetailId { get => fields.DetailId[this]; set => fields.DetailId[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public Int32Field DetailId;
#pragma warning restore CS0649
        }
    }

    [TableName("MismatchMains")]
    [UpdatableExtension("d", typeof(ExtRow), ThisKey = "DetailId")]
    public class MismatchMainRow : Row<MismatchMainRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [ForeignKey(typeof(ExtRow)), LeftJoin("d")]
        public int? DetailId { get => fields.DetailId[this]; set => fields.DetailId[this] = value; }
        [Expression("d.Text")]
        public int? DetailInt { get => fields.DetailInt[this]; set => fields.DetailInt[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public Int32Field DetailId;
            public Int32Field DetailInt;
#pragma warning restore CS0649
        }
    }

    [TableName("IdKeyMains")]
    [UpdatableExtension("d", typeof(ExtRow))]
    public class IdKeyMainRow : Row<IdKeyMainRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [Expression("d.Text")]
        public string DetailText { get => fields.DetailText[this]; set => fields.DetailText[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public StringField DetailText;
#pragma warning restore CS0649
        }
    }

    [TableName("FilterMains")]
    [UpdatableExtension("d", typeof(FilterExtRow), ThisKey = "DetailId", FilterField = "Kind")]
    public class FilterMainRow : Row<FilterMainRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [ForeignKey(typeof(FilterExtRow)), LeftJoin("d")]
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

    [TableName("FilterValueMains")]
    [UpdatableExtension("d", typeof(FilterExtRow), ThisKey = "DetailId", FilterField = "Kind", FilterValue = 1)]
    public class FilterValueMainRow : Row<FilterValueMainRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [ForeignKey(typeof(FilterExtRow)), LeftJoin("d")]
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

    [TableName("PresenceMains")]
    [UpdatableExtension("d", typeof(FilterExtRow), ThisKey = "DetailId", PresenceField = "Kind", PresenceValue = 1)]
    public class PresenceMainRow : Row<PresenceMainRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [ForeignKey(typeof(FilterExtRow)), LeftJoin("d")]
        public int? DetailId { get => fields.DetailId[this]; set => fields.DetailId[this] = value; }
        public int? Kind { get => fields.Kind[this]; set => fields.Kind[this] = value; }
        [Expression("d.Text")]
        public string DetailText { get => fields.DetailText[this]; set => fields.DetailText[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public Int32Field DetailId;
            public Int32Field Kind;
            public StringField DetailText;
#pragma warning restore CS0649
        }
    }

    [TableName("PresenceBoolMains")]
    [UpdatableExtension("d", typeof(FilterExtRow), ThisKey = "DetailId", PresenceField = "Kind", PresenceValue = true)]
    public class PresenceBoolMainRow : Row<PresenceBoolMainRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [ForeignKey(typeof(FilterExtRow)), LeftJoin("d")]
        public int? DetailId { get => fields.DetailId[this]; set => fields.DetailId[this] = value; }
        public int? Kind { get => fields.Kind[this]; set => fields.Kind[this] = value; }
        [Expression("d.Text")]
        public string DetailText { get => fields.DetailText[this]; set => fields.DetailText[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public Int32Field DetailId;
            public Int32Field Kind;
            public StringField DetailText;
#pragma warning restore CS0649
        }
    }

    private static MockSaveHandler<TRow> SaveHandler<TRow>(MockDbConnection connection, TRow row)
        where TRow : IRow, new()
    {
        return new MockSaveHandler<TRow>
        {
            Row = row,
            Old = new TRow(),
            IsUpdate = true,
            IsCreate = false,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        };
    }

    [Fact]
    public void ActivateFor_ReturnsFalse_WhenNoAttributes()
    {
        Assert.False(new UpdatableExtensionBehavior(new MockHandlerFactory()).ActivateFor(new NoAttrMainRow()));
    }

    [Fact]
    public void ActivateFor_Throws_WhenRowTypeInvalid()
    {
        var behavior = new UpdatableExtensionBehavior(new MockHandlerFactory());
        Assert.Throws<ArgumentException>(() => behavior.ActivateFor(new BadRowTypeMainRow { Id = 1 }));
    }

    [Fact]
    public void ActivateFor_Throws_WhenThisKeyMissingAndNotIdRow()
    {
        var behavior = new UpdatableExtensionBehavior(new MockHandlerFactory());
        Assert.Throws<ArgumentException>(() => behavior.ActivateFor(new NotIdThisKeyMainRow { Id = 1 }));
    }

    [Fact]
    public void ActivateFor_Throws_WhenThisKeyFieldMissing()
    {
        var behavior = new UpdatableExtensionBehavior(new MockHandlerFactory());
        Assert.Throws<ArgumentException>(() => behavior.ActivateFor(new MissingThisKeyMainRow { Id = 1 }));
    }

    [Fact]
    public void ActivateFor_Throws_WhenOtherKeyMissing()
    {
        var behavior = new UpdatableExtensionBehavior(new MockHandlerFactory());
        Assert.Throws<ArgumentException>(() => behavior.ActivateFor(new MissingOtherKeyMainRow { Id = 1 }));
    }

    [Fact]
    public void ActivateFor_Throws_WhenFilterFieldMissing()
    {
        var behavior = new UpdatableExtensionBehavior(new MockHandlerFactory());
        Assert.Throws<ArgumentException>(() => behavior.ActivateFor(new MissingFilterMainRow { Id = 1 }));
    }

    [Fact]
    public void ActivateFor_Throws_WhenPresenceFieldMissing()
    {
        var behavior = new UpdatableExtensionBehavior(new MockHandlerFactory());
        Assert.Throws<ArgumentException>(() => behavior.ActivateFor(new MissingPresenceMainRow { Id = 1 }));
    }

    [Fact]
    public void ActivateFor_Throws_WhenNoMappingsMatched()
    {
        var behavior = new UpdatableExtensionBehavior(new MockHandlerFactory());
        Assert.Throws<ArgumentException>(() => behavior.ActivateFor(new NoMatchMainRow { Id = 1 }));
    }

    [Fact]
    public void ActivateFor_Throws_WhenMappedFieldTypesMismatch()
    {
        var behavior = new UpdatableExtensionBehavior(new MockHandlerFactory());
        Assert.Throws<ArgumentException>(() => behavior.ActivateFor(new MismatchMainRow { Id = 1 }));
    }

    [Fact]
    public void OnBeforeSave_Skips_WhenNoMappedFieldsAssigned()
    {
        var behavior = new UpdatableExtensionBehavior(new MockHandlerFactory((_, _) =>
            throw new InvalidOperationException("should not create")));
        var row = new IdKeyMainRow { Id = 1 };
        Assert.True(behavior.ActivateFor(row));

        behavior.OnBeforeSave(SaveHandler(new MockDbConnection(), row));
        behavior.OnAfterSave(SaveHandler(new MockDbConnection(), row));
    }

    [Fact]
    public void OnBeforeSave_ThenOnAfterSave_UsesIdKey()
    {
        var calls = new List<SaveRequestType>();
        var behavior = new UpdatableExtensionBehavior(new MockHandlerFactory((_, intf) =>
        {
            if (intf == typeof(IListRequestProcessor))
                return new MockListHandler<ExtRow>();
            return new MockSaveHandler<ExtRow>(x => calls.Add(x.RequestType));
        }));
        var row = new IdKeyMainRow { Id = 7, DetailText = "T" };
        Assert.True(behavior.ActivateFor(row));
        using var connection = new MockDbConnection();

        var handler = SaveHandler(connection, row);
        behavior.OnBeforeSave(handler);
        Assert.Empty(calls); // deferred until after save when ThisKey is Id

        behavior.OnAfterSave(handler);
        Assert.Equal([SaveRequestType.Create], calls);
    }

    [Fact]
    public void GetExistingID_Throws_WhenMultipleExtensionRows()
    {
        var behavior = new UpdatableExtensionBehavior(new MockHandlerFactory((_, intf) =>
            new MockListHandler<UpdatableExtensionBehavior_ForeignThisKey_Test.DetailRow>(x =>
                x.Response.Entities = [new() { Id = 1 }, new() { Id = 2 }])));
        var row = new UpdatableExtensionBehavior_ForeignThisKey_Test.MainRow { DetailId = 1357, DetailText = "T" };
        Assert.True(behavior.ActivateFor(row));

        Assert.Throws<Exception>(() => behavior.OnBeforeSave(SaveHandler(new MockDbConnection(), row)));
    }

    [Fact]
    public void ApplyFilter_WithNullFilterValue_UsesIsNull()
    {
        ListRequest? captured = null;
        var behavior = new UpdatableExtensionBehavior(new MockHandlerFactory((_, intf) =>
        {
            if (intf == typeof(IListRequestProcessor))
                return new MockListHandler<FilterExtRow>(x => captured = x.Request);
            return new MockSaveHandler<FilterExtRow>();
        }));
        var row = new FilterMainRow { Id = 1, DetailId = 1357, DetailText = "T" };
        Assert.True(behavior.ActivateFor(row));

        behavior.OnBeforeSave(SaveHandler(new MockDbConnection(), row));

        var and = Assert.IsType<BinaryCriteria>(captured!.Criteria);
        Assert.Equal(CriteriaOperator.AND, and.Operator);
        var unary = Assert.IsType<UnaryCriteria>(and.RightOperand);
        Assert.Equal(CriteriaOperator.IsNull, unary.Operator);
    }

    [Fact]
    public void ApplyFilter_WithFilterValue_UsesEquality()
    {
        ListRequest? captured = null;
        var behavior = new UpdatableExtensionBehavior(new MockHandlerFactory((_, intf) =>
        {
            if (intf == typeof(IListRequestProcessor))
                return new MockListHandler<FilterExtRow>(x => captured = x.Request);
            return new MockSaveHandler<FilterExtRow>();
        }));
        var row = new FilterValueMainRow { Id = 1, DetailId = 1357, DetailText = "T" };
        Assert.True(behavior.ActivateFor(row));

        behavior.OnBeforeSave(SaveHandler(new MockDbConnection(), row));

        var and = Assert.IsType<BinaryCriteria>(captured!.Criteria);
        var eq = Assert.IsType<BinaryCriteria>(and.RightOperand);
        Assert.Equal(CriteriaOperator.EQ, eq.Operator);
        Assert.Equal(1, Assert.IsType<ValueCriteria>(eq.RightOperand).Value);
    }

    [Fact]
    public void CheckPresenceValue_NonBool_SkipsSaveOnMismatch()
    {
        var saved = false;
        var behavior = new UpdatableExtensionBehavior(new MockHandlerFactory((_, intf) =>
        {
            if (intf == typeof(IListRequestProcessor))
                return new MockListHandler<FilterExtRow>();
            return new MockSaveHandler<FilterExtRow>(_ => saved = true);
        }));
        var row = new PresenceMainRow { Id = 1, DetailId = 1357, DetailText = "T" };
        Assert.True(behavior.ActivateFor(row));

        behavior.OnBeforeSave(SaveHandler(new MockDbConnection(), row));

        Assert.False(saved);
    }

    [Fact]
    public void CheckPresenceValue_Bool_SkipsWhenNull()
    {
        var saved = false;
        var behavior = new UpdatableExtensionBehavior(new MockHandlerFactory((_, intf) =>
        {
            if (intf == typeof(IListRequestProcessor))
                return new MockListHandler<FilterExtRow>();
            return new MockSaveHandler<FilterExtRow>(_ => saved = true);
        }));
        var row = new PresenceBoolMainRow { Id = 1, DetailId = 1357, DetailText = "T" };
        Assert.True(behavior.ActivateFor(row));

        behavior.OnBeforeSave(SaveHandler(new MockDbConnection(), row));

        Assert.False(saved);
    }

    [Fact]
    public void OnBeforeDelete_Skips_WhenNotCascade()
    {
        var behavior = new UpdatableExtensionBehavior(new MockHandlerFactory((_, _) =>
            throw new InvalidOperationException("should not create")));
        var row = new IdKeyMainRow { Id = 1, DetailText = "T" };
        Assert.True(behavior.ActivateFor(row));

        behavior.OnBeforeDelete(new MockDeleteHandler<IdKeyMainRow>
        {
            Row = row,
            Connection = new MockDbConnection(),
            UnitOfWork = new MockUnitOfWork(new MockDbConnection())
        });
    }

    [Fact]
    public void OnBeforeDelete_Skips_WhenThisKeyNull()
    {
        var behavior = new UpdatableExtensionBehavior(new MockHandlerFactory((_, _) =>
            throw new InvalidOperationException("should not create")));
        var row = new UpdatableExtensionBehavior_Async_Tests.CascadeMainRow { Id = 5 };
        Assert.True(behavior.ActivateFor(row));

        behavior.OnBeforeDelete(new MockDeleteHandler<UpdatableExtensionBehavior_Async_Tests.CascadeMainRow>
        {
            Row = row,
            Connection = new MockDbConnection(),
            UnitOfWork = new MockUnitOfWork(new MockDbConnection())
        });
    }

    [Fact]
    public void OnBeforeDelete_Skips_WhenNoExistingDetail()
    {
        var behavior = new UpdatableExtensionBehavior(new MockHandlerFactory((_, intf) =>
            new MockListHandler<UpdatableExtensionBehavior_ForeignThisKey_Test.DetailRow>()));
        var row = new UpdatableExtensionBehavior_Async_Tests.CascadeMainRow { Id = 5, DetailId = 5 };
        Assert.True(behavior.ActivateFor(row));

        behavior.OnBeforeDelete(new MockDeleteHandler<UpdatableExtensionBehavior_Async_Tests.CascadeMainRow>
        {
            Row = row,
            Connection = new MockDbConnection(),
            UnitOfWork = new MockUnitOfWork(new MockDbConnection())
        });
    }
}
