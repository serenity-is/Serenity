using System.Collections;

namespace Serenity.Services;

public partial class LocalizationBehaviorTests
{
    [TableName("LocPermMains")]
    [LocalizationRow(typeof(LocPermLangRow), MappedIdField = "MasterId")]
    [ReadPermission(SpecialPermissionKeys.Public)]
    private class LocPermMainRow : Row<LocPermMainRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [Updatable(false)]
        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        [Insertable(false)]
        public string Code { get => fields.Code[this]; set => fields.Code[this] = value; }

        [UpdatePermission("SomePermission")]
        public string Restricted { get => fields.Restricted[this]; set => fields.Restricted[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public StringField Name = null!;
            public StringField Code = null!;
            public StringField Restricted = null!;
        }
    }

    [TableName("LocPermLang")]
    [ReadPermission(SpecialPermissionKeys.Public)]
    private class LocPermLangRow : Row<LocPermLangRow.RowFields>, IIdRow, ILocalizationRow
    {
        [Identity]
        public long? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public int? MasterId { get => fields.MasterId[this]; set => fields.MasterId[this] = value; }

        public string LanguageId { get => fields.LanguageId[this]; set => fields.LanguageId[this] = value; }

        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public string Code { get => fields.Code[this]; set => fields.Code[this] = value; }

        public string Restricted { get => fields.Restricted[this]; set => fields.Restricted[this] = value; }

        public StringField CultureIdField => fields.LanguageId;

        public class RowFields : RowFieldsBase
        {
            public Int64Field Id = null!;
            public Int32Field MasterId = null!;
            public StringField LanguageId = null!;
            public StringField Name = null!;
            public StringField Code = null!;
            public StringField Restricted = null!;
        }
    }

    private static MockSaveHandler<TRow> CreatePermSaveHandler<TRow>(MockDbConnection connection, bool isCreate,
        TRow row, IDictionary localizations, IRequestContext context)
        where TRow : class, IRow, new()
    {
        return new MockSaveHandler<TRow>
        {
            Row = row,
            Old = isCreate ? null : new TRow(),
            IsCreate = isCreate,
            IsUpdate = !isCreate,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection),
            Context = context,
            Request = new SaveRequest<TRow>
            {
                Entity = row,
                Localizations = (Dictionary<string, TRow>)localizations
            }
        };
    }

    private static MockHandlerFactory PermSaveFactory(Action<MockSaveHandler<LocPermLangRow>> onSave = null)
    {
        return new MockHandlerFactory((rowType, intfType) =>
        {
            Assert.Equal(typeof(LocPermLangRow), rowType);
            Assert.Equal(typeof(ISaveRequestProcessor), intfType);
            return new MockSaveHandler<LocPermLangRow>(onSave ?? (_ => { }));
        });
    }

    private static Dictionary<string, LocPermMainRow> PermLocalization(LocPermMainRow row)
    {
        return new Dictionary<string, LocPermMainRow> { ["en"] = row };
    }

    [Fact]
    public void OnAfterSave_Throws_WhenNonUpdatableFieldChanged()
    {
        var behavior = new LocalizationBehavior(new MockHandlerFactory((_, _) =>
            throw new InvalidOperationException("should not save")));
        var callCount = 0;
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
                ++callCount == 1
                    ? args.ToMockReader(new { Id = 123L })
                    : args.ToMockReader(new { Id = 123L, Name = "Old" }));
        var row = new LocPermMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));
        var handler = CreatePermSaveHandler(connection, false, row,
            PermLocalization(new LocPermMainRow { Name = "New" }),
            new NullRequestContext().AsSysAdmin());

        var exception = Assert.Throws<ValidationError>(() => behavior.OnAfterSave(handler));
        Assert.Equal("ReadOnly", exception.ErrorCode);
    }

    [Fact]
    public void OnAfterSave_Ignores_WhenNonUpdatableFieldUnchanged()
    {
        bool saved = false;
        var behavior = new LocalizationBehavior(PermSaveFactory(_ => saved = true));
        var callCount = 0;
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
                ++callCount == 1
                    ? args.ToMockReader(new { Id = 123L })
                    : args.ToMockReader(new { Id = 123L, Name = "Old" }));
        var row = new LocPermMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));
        var handler = CreatePermSaveHandler(connection, false, row,
            PermLocalization(new LocPermMainRow { Name = "Old" }),
            new NullRequestContext().AsSysAdmin());

        behavior.OnAfterSave(handler);

        Assert.True(saved);
    }

    [Fact]
    public void OnAfterSave_Throws_WhenNonInsertableFieldOnCreate()
    {
        var behavior = new LocalizationBehavior(new MockHandlerFactory((_, _) =>
            throw new InvalidOperationException("should not save")));
        using var connection = new MockDbConnection();
        var row = new LocPermMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));
        var handler = CreatePermSaveHandler(connection, true, row,
            PermLocalization(new LocPermMainRow { Code = "X" }),
            new NullRequestContext().AsSysAdmin());

        var exception = Assert.Throws<ValidationError>(() => behavior.OnAfterSave(handler));
        Assert.Equal("ReadOnly", exception.ErrorCode);
        Assert.Empty(connection.ExecuteReaderCalls);
    }

    [Fact]
    public void OnAfterSave_Throws_WhenFieldUpdatePermissionMissing()
    {
        var behavior = new LocalizationBehavior(new MockHandlerFactory((_, _) =>
            throw new InvalidOperationException("should not save")));
        var callCount = 0;
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
                ++callCount == 1
                    ? args.ToMockReader(new { Id = 123L })
                    : args.ToMockReader(new { Id = 123L, Restricted = "Old" }));
        var row = new LocPermMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));
        var handler = CreatePermSaveHandler(connection, false, row,
            PermLocalization(new LocPermMainRow { Restricted = "New" }),
            new NullRequestContext().AsGuest());

        var exception = Assert.Throws<ValidationError>(() => behavior.OnAfterSave(handler));
        Assert.Equal("ReadOnly", exception.ErrorCode);
    }

    [Fact]
    public void OnAfterSave_Allows_WhenFieldUpdatePermissionGranted()
    {
        LocPermLangRow savedRow = null;
        var behavior = new LocalizationBehavior(PermSaveFactory(x => savedRow = (LocPermLangRow)x.Request.Entity));
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 123L }));
        var row = new LocPermMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));
        var handler = CreatePermSaveHandler(connection, false, row,
            PermLocalization(new LocPermMainRow { Restricted = "New" }),
            new NullRequestContext().AsSysAdmin());

        behavior.OnAfterSave(handler);

        Assert.NotNull(savedRow);
        Assert.Equal("New", savedRow.Restricted);
    }

    [Fact]
    public async Task OnAfterSaveAsync_Throws_WhenNonUpdatableFieldChanged()
    {
        var behavior = new LocalizationBehavior(new MockHandlerFactory((_, _) =>
            throw new InvalidOperationException("should not save")));
        var callCount = 0;
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
                ++callCount == 1
                    ? args.ToMockReader(new { Id = 123L })
                    : args.ToMockReader(new { Id = 123L, Name = "Old" }));
        var row = new LocPermMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));
        var handler = CreatePermSaveHandler(connection, false, row,
            PermLocalization(new LocPermMainRow { Name = "New" }),
            new NullRequestContext().AsSysAdmin());

        var exception = await Assert.ThrowsAsync<ValidationError>(
            () => behavior.OnAfterSaveAsync(handler, TestContext.Current.CancellationToken));
        Assert.Equal("ReadOnly", exception.ErrorCode);
    }
}
