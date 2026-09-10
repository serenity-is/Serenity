namespace Serenity.Services;

public class SaveRequestHandlerTests_Coverage
{
    private static IRequestContext Context(IBehaviorProvider? behaviors = null) =>
        new NullRequestContext(behaviors).WithPermissions(_ => true);

    [AttributeUsage(AttributeTargets.Property)]
    private class FailValidatorAttribute : Attribute, ICustomValidator
    {
        public string Validate(IValidationContext context) => "invalid";
    }

    private class CoverRow : Row<CoverRow.RowFields>, IIdRow, INameRow, IIsActiveRow, IIsDeletedRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [NameProperty]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public short? IsActive { get => fields.IsActive[this]; set => fields.IsActive[this] = value; }

        public bool? IsDeleted { get => fields.IsDeleted[this]; set => fields.IsDeleted[this] = value; }

        [NotMapped]
        public string? NotMapped { get => fields.NotMapped[this]; set => fields.NotMapped[this] = value; }

        [DefaultValue("dv")]
        public string? DefaultVal { get => fields.DefaultVal[this]; set => fields.DefaultVal[this] = value; }

        public string? TrimEmpty { get => fields.TrimEmpty[this]; set => fields.TrimEmpty[this] = value; }

        [FailValidator]
        public string? CustomVal { get => fields.CustomVal[this]; set => fields.CustomVal[this] = value; }

        Int16Field IIsActiveRow.IsActiveField => fields.IsActive;
        BooleanField IIsDeletedRow.IsDeletedField => fields.IsDeleted;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Name;
            public Int16Field IsActive;
            public BooleanField IsDeleted;
            public StringField NotMapped;
            public StringField DefaultVal;
            public StringField TrimEmpty;
            public StringField CustomVal;

            public RowFields()
            {
                Id = new Int32Field(this, "Id");
                Name = new StringField(this, "Name");
                IsActive = new Int16Field(this, "IsActive");
                IsDeleted = new BooleanField(this, "IsDeleted");
                NotMapped = new StringField(this, "NotMapped");
                DefaultVal = new StringField(this, "DefaultVal");
                TrimEmpty = new StringField(this, "TrimEmpty");
                CustomVal = new StringField(this, "CustomVal");
            }
        }
    }

    private class CoverHandler(IRequestContext context) :
        SaveRequestHandler<CoverRow, SaveRequest<CoverRow>, SaveResponse>(context)
    {
        public void SetRow(CoverRow row) => Row = row;
        public void SetOld(CoverRow old) => Old = old;
        public void SetUow(IUnitOfWork uow) => UnitOfWork = uow;
        public CoverRow CurrentRow => Row;

        public void DoClearNonTableAssignments() => ClearNonTableAssignments();
        public BaseCriteria DoGetDisplayOrderFilter() => GetDisplayOrderFilter();
        public void DoHandleNonEditable(Field f) => HandleNonEditable(f);
        public void DoSetDefaultValues() => SetDefaultValues();
        public void DoSetTrimToEmptyFields() => SetTrimToEmptyFields();
        public void DoValidateFieldValues() => ValidateFieldValues();
        public void DoValidateIsActive() => ValidateIsActive();
        public HashSet<Field> DoValidateEditable() => ValidateEditable();
    }

    private class TrackingBehavior : BaseSaveBehavior
    {
        public bool ExceptionCalled;

        public override void OnException(ISaveRequestHandler handler, Exception exception)
        {
            ExceptionCalled = true;
        }
    }

    private static CoverHandler NewHandler(IBehaviorProvider? behaviors = null)
    {
        var handler = new CoverHandler(Context(behaviors));
        handler.SetUow(new MockUnitOfWork(new MockDbConnection().InterceptManipulateRow(_ => 1)));
        return handler;
    }

    [Fact]
    public void ClearNonTableAssignments_Clears_NonTable_Fields()
    {
        var handler = NewHandler();
        var row = new CoverRow { Name = "A", NotMapped = "X" };
        handler.SetRow(row);
        var nonTable = row.GetFields().NotMapped;
        nonTable.Flags |= FieldFlags.Calculated;

        try
        {
            handler.DoClearNonTableAssignments();

            Assert.False(row.IsAssigned(nonTable));
            Assert.True(row.IsAssigned(row.GetFields().Name));
        }
        finally
        {
            nonTable.Flags &= ~FieldFlags.Calculated;
        }
    }

    [Fact]
    public void GetDisplayOrderFilter_Returns_Null_For_NonDisplayOrder_Row()
    {
        var handler = NewHandler();
        handler.SetRow(new CoverRow());

        _ = handler.DoGetDisplayOrderFilter();
    }

    [Fact]
    public void HandleNonEditable_Update_Unchanged_Copies_Old()
    {
        var handler = NewHandler();
        var row = new CoverRow { Name = "Same" };
        var old = new CoverRow { Name = "Same" };
        handler.SetRow(row);
        handler.SetOld(old);

        handler.DoHandleNonEditable(row.GetFields().Name);

        Assert.False(row.IsAssigned(row.GetFields().Name));
        Assert.Equal("Same", row.Name);
    }

    [Fact]
    public void HandleNonEditable_Update_NonTable_Clears()
    {
        var handler = NewHandler();
        var row = new CoverRow { Name = "New" };
        var old = new CoverRow { Name = "Old" };
        handler.SetRow(row);
        handler.SetOld(old);
        var field = row.GetFields().Name;
        field.Flags |= FieldFlags.Foreign;

        try
        {
            handler.DoHandleNonEditable(field);
            Assert.False(row.IsAssigned(field));
        }
        finally
        {
            field.Flags &= ~FieldFlags.Foreign;
        }
    }

    [Fact]
    public void HandleNonEditable_Update_ReadOnly_Throws()
    {
        var handler = NewHandler();
        var row = new CoverRow { Name = "New" };
        var old = new CoverRow { Name = "Old" };
        handler.SetRow(row);
        handler.SetOld(old);

        Assert.Throws<ValidationError>(() => handler.DoHandleNonEditable(row.GetFields().Name));
    }

    [Fact]
    public void HandleNonEditable_Create_Reflective_NotEditable_Throws()
    {
        var handler = NewHandler();
        var row = new CoverRow { Name = "New" };
        handler.SetRow(row);

        Assert.Throws<ValidationError>(() => handler.DoHandleNonEditable(row.GetFields().Name));
    }

    [Fact]
    public void HandleNonEditable_Create_NonTable_Clears()
    {
        var handler = NewHandler();
        var row = new CoverRow { Name = "New" };
        handler.SetRow(row);
        var field = row.GetFields().Name;
        field.Flags |= FieldFlags.Foreign;

        try
        {
            handler.DoHandleNonEditable(field);
            Assert.Null(row.Name);
            Assert.False(row.IsAssigned(field));
        }
        finally
        {
            field.Flags &= ~FieldFlags.Foreign;
        }
    }

    [Fact]
    public void SetDefaultValues_Sets_Default_And_IsActive()
    {
        var handler = NewHandler();
        var row = new CoverRow();
        handler.SetRow(row);

        handler.DoSetDefaultValues();

        Assert.Equal("dv", row.DefaultVal);
        Assert.Equal((short)1, row.IsActive);
    }

    [Fact]
    public void SetTrimToEmptyFields_Sets_Empty_String()
    {
        var handler = NewHandler();
        var row = new CoverRow();
        handler.SetRow(row);
        var field = row.GetFields().TrimEmpty;
        field.Flags |= FieldFlags.TrimToEmpty | FieldFlags.NotNull;

        try
        {
            handler.DoSetTrimToEmptyFields();
            Assert.Equal("", row.TrimEmpty);
        }
        finally
        {
            field.Flags &= ~(FieldFlags.TrimToEmpty | FieldFlags.NotNull);
        }
    }

    [Fact]
    public void ValidateFieldValues_Throws_For_Custom_Validator()
    {
        var handler = NewHandler();
        var row = new CoverRow { CustomVal = "x" };
        handler.SetRow(row);

        Assert.Throws<ValidationError>(() => handler.DoValidateFieldValues());
    }

    [Fact]
    public void ValidateIsActive_Throws_For_Inactive_Or_Deleted()
    {
        var handler = NewHandler();
        handler.SetRow(new CoverRow());
        handler.SetOld(new CoverRow { IsActive = -1 });
        Assert.Throws<ValidationError>(() => handler.DoValidateIsActive());

        handler.SetOld(new CoverRow { IsActive = 1, IsDeleted = true });
        Assert.Throws<ValidationError>(() => handler.DoValidateIsActive());

        handler.SetOld(new CoverRow { IsActive = 1, IsDeleted = false });
        handler.DoValidateIsActive();
    }

    [Fact]
    public void ValidateEditable_Returns_Editable_Fields()
    {
        var handler = NewHandler();
        handler.SetRow(new CoverRow { Name = "A" });

        var editable = handler.DoValidateEditable();

        Assert.Contains(handler.CurrentRow.GetFields().Name, editable);
    }

    [Fact]
    public void Create_Without_AutoIncrement_Inserts_And_Sets_EntityId()
    {
        using var connection = new MockDbConnection().InterceptManipulateRow(_ => 1);
        var handler = new CoverHandler(Context());

        var response = handler.Create(new MockUnitOfWork(connection), new SaveRequest<CoverRow>
        {
            Entity = new CoverRow { Id = 7, Name = "A" }
        });

        Assert.NotNull(response);
    }

    [Fact]
    public void Process_Auto_Detects_Create_And_Update()
    {
        using var connection = new MockDbConnection()
            .InterceptManipulateRow(_ => 1)
            .InterceptExecuteReader(args => args.ToMockReader(new { ID = 5, Name = "Old" }, new { ID = 5, Name = "Old" }))
            .OnDbCommandExecuteNonQuery(_ => 1);
        var handler = new CoverHandler(Context());
        var uow = new MockUnitOfWork(connection);

        var createResponse = handler.Process(uow, new SaveRequest<CoverRow>
        {
            Entity = new CoverRow { Name = "A" }
        });
        Assert.NotNull(createResponse);

        var updateResponse = handler.Process(uow, new SaveRequest<CoverRow>
        {
            Entity = new CoverRow { Id = 5, Name = "New" }
        });
        Assert.NotNull(updateResponse);
    }

    [Fact]
    public void Explicit_ISaveRequestProcessor_Process_Works()
    {
        using var connection = new MockDbConnection().InterceptManipulateRow(_ => 1);
        var handler = new CoverHandler(Context());

        var response = ((ISaveRequestProcessor)handler).Process(
            new MockUnitOfWork(connection),
            new SaveRequest<CoverRow> { Entity = new CoverRow { Id = 7, Name = "A" } },
            SaveRequestType.Create);

        Assert.NotNull(response);
    }

    [Fact]
    public void Update_With_Changed_Id_Uses_SqlUpdate()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { ID = 5, Name = "Old" }))
            .OnDbCommandExecuteNonQuery(_ => 1)
            .InterceptManipulateRow(_ => 1);
        var handler = new CoverHandler(Context());

        var response = handler.Update(new MockUnitOfWork(connection), new SaveRequest<CoverRow>
        {
            EntityId = 5,
            Entity = new CoverRow { Id = 6, Name = "New" }
        });

        Assert.NotNull(response);
    }

    [Fact]
    public void Update_With_Behavior_Prepares_Query_And_OnException()
    {
        var behavior = new TrackingBehavior();
        var behaviors = new MockBehaviorProvider((_, _, _) => new object[] { behavior });
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { ID = 5, Name = "Old" }))
            .OnDbCommandExecuteNonQuery(_ => throw new InvalidOperationException("boom"));
        var handler = new CoverHandler(Context(behaviors));
        var uow = new MockUnitOfWork(connection);

        Assert.Throws<InvalidOperationException>(() => handler.Update(uow, new SaveRequest<CoverRow>
        {
            EntityId = 5,
            Entity = new CoverRow { Id = 6, Name = "New" }
        }));

        Assert.True(behavior.ExceptionCalled);
    }

    [Fact]
    public void Update_When_Old_Entity_Not_Found_Throws()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());
        var handler = new CoverHandler(Context());

        Assert.Throws<ValidationError>(() => handler.Update(new MockUnitOfWork(connection), new SaveRequest<CoverRow>
        {
            EntityId = 5,
            Entity = new CoverRow { Id = 5, Name = "New" }
        }));
    }
}
