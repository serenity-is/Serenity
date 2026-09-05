using System.Collections;
using System.Threading;

namespace Serenity.Services;

public class LocalizationBehaviorTests
{
    [TableName("LocMains")]
    [LocalizationRow(typeof(LocMainLangRow), MappedIdField = "MasterId")]
    private class LocMainRow : Row<LocMainRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public string Description { get => fields.Description[this]; set => fields.Description[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public StringField Name;
            public StringField Description;
#pragma warning restore CS0649
        }
    }

    [TableName("LocMainLang")]
    private class LocMainLangRow : Row<LocMainLangRow.RowFields>, IIdRow, ILocalizationRow
    {
        [Identity]
        public long? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public int? MasterId { get => fields.MasterId[this]; set => fields.MasterId[this] = value; }

        public string LanguageId { get => fields.LanguageId[this]; set => fields.LanguageId[this] = value; }

        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public string Description { get => fields.Description[this]; set => fields.Description[this] = value; }

        public StringField CultureIdField => fields.LanguageId;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int64Field Id;
            public Int32Field MasterId;
            public StringField LanguageId;
            public StringField Name;
            public StringField Description;
#pragma warning restore CS0649
        }
    }

    private static LocalizationBehavior CreateBehavior(LocMainRow row)
    {
        var behavior = new LocalizationBehavior(new MockHandlerFactory());
        Assert.True(behavior.ActivateFor(row));
        return behavior;
    }

    private static MockSaveHandler<LocMainRow> CreateSaveHandler(MockDbConnection connection, bool isCreate, LocMainRow row, IDictionary localizations)
    {
        return new MockSaveHandler<LocMainRow>
        {
            Row = row,
            Old = isCreate ? null : new LocMainRow(),
            IsCreate = isCreate,
            IsUpdate = !isCreate,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection),
            Request = new SaveRequest<LocMainRow>
            {
                Entity = row,
                Localizations = (Dictionary<string, LocMainRow>)localizations
            }
        };
    }

    private static MockSaveHandlerAsync<LocMainRow> CreateSaveHandlerAsync(MockDbConnection connection, bool isCreate, LocMainRow row, IDictionary localizations)
    {
        return new MockSaveHandlerAsync<LocMainRow>
        {
            Row = row,
            Old = isCreate ? null : new LocMainRow(),
            IsCreate = isCreate,
            IsUpdate = !isCreate,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection),
            Request = new SaveRequest<LocMainRow>
            {
                Entity = row,
                Localizations = (Dictionary<string, LocMainRow>)localizations
            }
        };
    }

    private static Dictionary<string, LocMainRow> OneLocalization(string culture, string description)
    {
        return new Dictionary<string, LocMainRow>
        {
            [culture] = new LocMainRow { Description = description }
        };
    }

    private static MockHandlerFactory SaveFactory(bool async, Action<MockSaveHandler<LocMainLangRow>> onSave, Action<MockSaveHandlerAsync<LocMainLangRow>> onSaveAsync)
    {
        return new MockHandlerFactory((rowType, intfType) =>
        {
            Assert.Equal(typeof(LocMainLangRow), rowType);
            if (async)
            {
                Assert.Equal(typeof(ISaveRequestProcessorAsync), intfType);
                return new MockSaveHandlerAsync<LocMainLangRow>(onSaveAsync);
            }
            else
            {
                Assert.Equal(typeof(ISaveRequestProcessor), intfType);
                return new MockSaveHandler<LocMainLangRow>(onSave);
            }
        });
    }

    [Fact]
    public void OnAfterSave_Sync_Create_SavesLocalization()
    {
        bool saved = false;
        var behavior = new LocalizationBehavior(SaveFactory(false,
            onSave: x =>
            {
                Assert.Equal(SaveRequestType.Create, x.RequestType);
                Assert.Null(x.Request?.EntityId);
                var lang = Assert.IsType<LocMainLangRow>(x.Request.Entity);
                Assert.Equal("en", lang.LanguageId);
                Assert.Equal("Hello", lang.Description);
                saved = true;
            },
            onSaveAsync: null));

        using var connection = new MockDbConnection();
        var row = new LocMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));
        var handler = CreateSaveHandler(connection, true, row, OneLocalization("en", "Hello"));

        behavior.OnAfterSave(handler);

        Assert.True(saved);
    }

    [Fact]
    public async Task OnAfterSaveAsync_Async_Create_SavesLocalization()
    {
        bool saved = false;
        var behavior = new LocalizationBehavior(SaveFactory(true,
            onSave: null,
            onSaveAsync: x =>
            {
                Assert.Equal(SaveRequestType.Create, x.RequestType);
                Assert.Null(x.Request?.EntityId);
                var lang = Assert.IsType<LocMainLangRow>(x.Request.Entity);
                Assert.Equal("en", lang.LanguageId);
                Assert.Equal("Hello", lang.Description);
                saved = true;
            }));

        using var connection = new MockDbConnection();
        var row = new LocMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));
        var handler = CreateSaveHandlerAsync(connection, true, row, OneLocalization("en", "Hello"));

        await behavior.OnAfterSaveAsync(handler, CancellationToken.None);

        Assert.True(saved);
    }

    [Fact]
    public void OnAfterSave_Sync_UpdateWithExistingRow_UpdatesLocalization()
    {
        bool updated = false;
        var behavior = new LocalizationBehavior(SaveFactory(false,
            onSave: x =>
            {
                Assert.Equal(SaveRequestType.Update, x.RequestType);
                var lang = Assert.IsType<LocMainLangRow>(x.Request.Entity);
                Assert.Equal((long?)123L, lang.Id);
                Assert.Equal("Hello", lang.Description);
                updated = true;
            },
            onSaveAsync: null));

        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.False(args.IsAsync);
                // GetOldLocalizationRowId select returns the existing local row id
                return new MockDbDataReader(new { Id = 123L });
            });
        var row = new LocMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));
        var handler = CreateSaveHandler(connection, false, row, OneLocalization("en", "Hello"));

        behavior.OnAfterSave(handler);

        Assert.True(updated);
        Assert.Single(connection.ExecuteReaderCalls);
    }

    [Fact]
    public async Task OnAfterSaveAsync_Async_UpdateWithExistingRow_UpdatesLocalization()
    {
        bool updated = false;
        var behavior = new LocalizationBehavior(SaveFactory(true,
            onSave: null,
            onSaveAsync: x =>
            {
                Assert.Equal(SaveRequestType.Update, x.RequestType);
                var lang = Assert.IsType<LocMainLangRow>(x.Request.Entity);
                Assert.Equal((long?)123L, lang.Id);
                Assert.Equal("Hello", lang.Description);
                updated = true;
            }));

        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.True(args.IsAsync);
                return new MockDbDataReader(new { Id = 123L });
            });
        var row = new LocMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));
        var handler = CreateSaveHandlerAsync(connection, false, row, OneLocalization("en", "Hello"));

        await behavior.OnAfterSaveAsync(handler, CancellationToken.None);

        Assert.True(updated);
        Assert.Single(connection.ExecuteReaderCalls);
    }

    [Fact]
    public void OnAfterSave_Sync_UpdateWithExistingRow_DeletesLocalization_WhenEmpty()
    {
        bool deleted = false;
        var behavior = new LocalizationBehavior(new MockHandlerFactory((rowType, intfType) =>
        {
            Assert.Equal(typeof(LocMainLangRow), rowType);
            Assert.Equal(typeof(IDeleteRequestProcessor), intfType);
            return new MockDeleteHandler<LocMainLangRow>(x =>
            {
                Assert.Equal(123L, x.Request?.EntityId);
                deleted = true;
            });
        }));

        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => new MockDbDataReader(new { Id = 123L }));
        var row = new LocMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));
        // empty description => not anyNonEmpty => delete old row
        var handler = CreateSaveHandler(connection, false, row,
            new Dictionary<string, LocMainRow> { ["en"] = new LocMainRow { Description = null } });

        behavior.OnAfterSave(handler);

        Assert.True(deleted);
    }

    [Fact]
    public void OnBeforeDelete_Sync_DeletesLocalizations()
    {
        var deleted = new List<object>();
        var behavior = new LocalizationBehavior(new MockHandlerFactory((rowType, intfType) =>
        {
            Assert.Equal(typeof(LocMainLangRow), rowType);
            Assert.Equal(typeof(IDeleteRequestProcessor), intfType);
            return new MockDeleteHandler<LocMainLangRow>(x =>
            {
                deleted.Add(x.Request?.EntityId);
            });
        }));

        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => new MockDbDataReader(new { Id = 11L }, new { Id = 12L }));
        var row = new LocMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));
        var handler = new MockDeleteHandler<LocMainRow>
        {
            Row = row,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        };

        behavior.OnBeforeDelete(handler);

        Assert.Equal([11L, 12L], deleted);
    }

    [Fact]
    public async Task OnBeforeDeleteAsync_Async_DeletesLocalizations()
    {
        var deleted = new List<object>();
        var behavior = new LocalizationBehavior(new MockHandlerFactory((rowType, intfType) =>
        {
            Assert.Equal(typeof(LocMainLangRow), rowType);
            Assert.Equal(typeof(IDeleteRequestProcessorAsync), intfType);
            return new MockDeleteHandlerAsync<LocMainLangRow>(x =>
            {
                deleted.Add(x.Request?.EntityId);
            });
        }));

        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => new MockDbDataReader(new { Id = 11L }, new { Id = 12L }));
        var row = new LocMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));
        var handler = new MockDeleteHandlerAsync<LocMainRow>
        {
            Row = row,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        };

        await behavior.OnBeforeDeleteAsync(handler, CancellationToken.None);

        Assert.Equal([11L, 12L], deleted);
    }
}
