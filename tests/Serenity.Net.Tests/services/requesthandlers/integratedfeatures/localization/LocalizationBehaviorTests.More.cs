#pragma warning disable CS0649
using System.Collections;
using System.Threading;

namespace Serenity.Services;

public partial class LocalizationBehaviorTests
{
    [TableName("PlainRows")]
    private class PlainRow : Row<PlainRow.RowFields>
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
        }
    }

    [TableName("BadLocMains")]
    [LocalizationRow(typeof(PlainRow))]
    private class BadLocMainRow : Row<BadLocMainRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
        }
    }

    [TableName("NotIdLocMains")]
    [LocalizationRow(typeof(LocMainLangRow), MappedIdField = "MasterId")]
    private class NotIdLocMainRow : Row<NotIdLocMainRow.RowFields>
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
        }
    }

    [TableName("MissingFkLocMains")]
    [LocalizationRow(typeof(LocMainLangRow), MappedIdField = "Nope")]
    private class MissingFkLocMainRow : Row<MissingFkLocMainRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
        }
    }

    [TableName("MatchMains")]
    private class MatchMainRow : Row<MatchMainRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }
        [NotMapped]
        public int? Temp { get => fields.Temp[this]; set => fields.Temp[this] = value; }
        [Localizable(false)]
        public string Secret { get => fields.Secret[this]; set => fields.Secret[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Name;
            public Int32Field Temp;
            public StringField Secret;
        }
    }

    [TableName("LocSecretMains")]
    [LocalizationRow(typeof(LocSecretLangRow), MappedIdField = "MasterId")]
    private class LocSecretMainRow : Row<LocSecretMainRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [Localizable(false)]
        public string Secret { get => fields.Secret[this]; set => fields.Secret[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Secret;
        }
    }

    [TableName("LocSecretLang")]
    private class LocSecretLangRow : Row<LocSecretLangRow.RowFields>, IIdRow, ILocalizationRow
    {
        [Identity]
        public long? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public int? MasterId { get => fields.MasterId[this]; set => fields.MasterId[this] = value; }
        public string LanguageId { get => fields.LanguageId[this]; set => fields.LanguageId[this] = value; }
        public string Secret { get => fields.Secret[this]; set => fields.Secret[this] = value; }
        public StringField CultureIdField => fields.LanguageId;

        public class RowFields : RowFieldsBase
        {
            public Int64Field Id;
            public Int32Field MasterId;
            public StringField LanguageId;
            public StringField Secret;
        }
    }

    [TableName("LocSoftMains")]
    [LocalizationRow(typeof(LocMainLangRow), MappedIdField = "MasterId")]
    private class LocSoftMainRow : Row<LocSoftMainRow.RowFields>, IIdRow, IIsDeletedRow
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }
        public bool? IsDeleted { get => fields.IsDeleted[this]; set => fields.IsDeleted[this] = value; }
        public BooleanField IsDeletedField => fields.IsDeleted;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Name;
            public BooleanField IsDeleted;
        }
    }

    [Fact]
    public void ActivateFor_Throws_WhenLocalRowNotILocalizationRow()
    {
        var behavior = new LocalizationBehavior(new MockHandlerFactory());
        Assert.Throws<ArgumentException>(() => behavior.ActivateFor(new BadLocMainRow { Id = 1 }));
    }

    [Fact]
    public void ActivateFor_Throws_WhenMasterNotIdRow()
    {
        var behavior = new LocalizationBehavior(new MockHandlerFactory());
        Assert.Throws<ArgumentException>(() => behavior.ActivateFor(new NotIdLocMainRow { Id = 1 }));
    }

    [Fact]
    public void ActivateFor_Throws_WhenMappedIdFieldMissing()
    {
        var behavior = new LocalizationBehavior(new MockHandlerFactory());
        Assert.Throws<ArgumentException>(() => behavior.ActivateFor(new MissingFkLocMainRow { Id = 1 }));
    }

    [Fact]
    public void GetLocalizationMatch_ReturnsNull_ForNonTableField()
    {
        Assert.Null(LocalizationBehavior.GetLocalizationMatch(
            MatchMainRow.Fields.Temp, new LocMainLangRow()));
    }

    [Fact]
    public void GetLocalizationMatch_ReturnsNull_ForNonLocalizableField()
    {
        Assert.Null(LocalizationBehavior.GetLocalizationMatch(
            MatchMainRow.Fields.Secret, new LocMainLangRow()));
    }

    [Fact]
    public void GetLocalizationMatch_ReturnsNull_ForIdField()
    {
        Assert.Null(LocalizationBehavior.GetLocalizationMatch(
            MatchMainRow.Fields.Id, new LocMainLangRow()));
    }

    [Fact]
    public void GetLocalizationMatch_ReturnsMatch_ForLocalizableField()
    {
        Assert.Same(LocMainLangRow.Fields.Name, LocalizationBehavior.GetLocalizationMatch(
            MatchMainRow.Fields.Name, new LocMainLangRow()));
    }

    [Fact]
    public void OnReturn_Retrieve_SetsLocalizations()
    {
        var factory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(LocMainLangRow), rowType);
            Assert.Equal(typeof(IListRequestProcessor), intf);
            return new MockListHandler<LocMainLangRow>(x =>
            {
                x.Response.Entities.Add(new LocMainLangRow
                {
                    LanguageId = "en", Name = "Hello", Description = "Desc"
                });
            });
        });

        var behavior = new LocalizationBehavior(factory);
        var row = new LocMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));
        var handler = new MockRetrieveHandler<LocMainRow>
        {
            Row = row,
            Connection = new MockDbConnection()
        };
        handler.Request.IncludeColumns = ["Localizations"];

        behavior.OnReturn(handler);

        Assert.NotNull(handler.Response.Localizations);
        var localized = Assert.IsType<LocMainRow>(handler.Response.Localizations!["en"]);
        Assert.Equal("Hello", localized.Name);
        Assert.Equal("Desc", localized.Description);
    }

    [Fact]
    public async Task OnReturnAsync_Retrieve_SetsLocalizations()
    {
        var factory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(LocMainLangRow), rowType);
            Assert.Equal(typeof(IListRequestProcessorAsync), intf);
            return new MockListHandlerAsync<LocMainLangRow>(x =>
            {
                x.Response.Entities.Add(new LocMainLangRow { LanguageId = "en", Name = "Hello" });
            });
        });

        var behavior = new LocalizationBehavior(factory);
        var row = new LocMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));
        var handler = new MockRetrieveHandler<LocMainRow>
        {
            Row = row,
            Connection = new MockDbConnection()
        };
        handler.Request.IncludeColumns = ["Localizations"];

        await behavior.OnReturnAsync(handler, TestContext.Current.CancellationToken);

        Assert.NotNull(handler.Response.Localizations);
        Assert.True(handler.Response.Localizations!.ContainsKey("en"));
    }

    [Fact]
    public void OnReturn_Retrieve_ReturnsEarly_WithoutIncludeColumns()
    {
        var behavior = new LocalizationBehavior(new MockHandlerFactory((_, _) =>
            throw new InvalidOperationException("should not create")));
        var row = new LocMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));
        var handler = new MockRetrieveHandler<LocMainRow> { Row = row };

        behavior.OnReturn(handler);

        Assert.Null(handler.Response.Localizations);
    }

    [Fact]
    public void OnAfterSave_ReturnsEarly_WhenNoLocalizations()
    {
        var behavior = new LocalizationBehavior(new MockHandlerFactory((_, _) =>
            throw new InvalidOperationException("should not create")));
        var row = new LocMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));

        var handler = new MockSaveHandler<LocMainRow>
        {
            Row = row,
            IsCreate = true,
            Connection = new MockDbConnection(),
            UnitOfWork = new MockUnitOfWork(new MockDbConnection()),
            Request = new SaveRequest<LocMainRow> { Entity = row, Localizations = null }
        };

        behavior.OnAfterSave(handler);
    }

    [Fact]
    public void OnAfterSave_Throws_WhenFieldNotLocalizable()
    {
        var behavior = new LocalizationBehavior(new MockHandlerFactory((_, _) =>
            throw new InvalidOperationException("should not create")));
        var row = new LocSecretMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));

        var handler = new MockSaveHandler<LocSecretMainRow>
        {
            Row = row,
            IsCreate = true,
            Connection = new MockDbConnection(),
            UnitOfWork = new MockUnitOfWork(new MockDbConnection()),
            Request = new SaveRequest<LocSecretMainRow>
            {
                Entity = row,
                Localizations = new Dictionary<string, LocSecretMainRow>
                {
                    ["en"] = new LocSecretMainRow { Secret = "x" }
                }
            }
        };

        Assert.Throws<ValidationError>(() => behavior.OnAfterSave(handler));
    }

    [Fact]
    public async Task OnAfterSaveAsync_Update_DeletesLocalization_WhenEmpty()
    {
        bool deleted = false;
        var behavior = new LocalizationBehavior(new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(LocMainLangRow), rowType);
            Assert.Equal(typeof(IDeleteRequestProcessorAsync), intf);
            return new MockDeleteHandlerAsync<LocMainLangRow>(x =>
            {
                Assert.Equal(123L, x.Request?.EntityId);
                deleted = true;
            });
        }));

        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => new MockDbDataReader(new { Id = 123L }));
        var row = new LocMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));
        var handler = CreateSaveHandlerAsync(connection, false, row,
            new Dictionary<string, LocMainRow> { ["en"] = new LocMainRow { Description = null } });

        await behavior.OnAfterSaveAsync(handler, TestContext.Current.CancellationToken);

        Assert.True(deleted);
    }

    [Fact]
    public void OnBeforeDelete_ReturnsEarly_ForSoftDelete()
    {
        var behavior = new LocalizationBehavior(new MockHandlerFactory((_, _) =>
            throw new InvalidOperationException("should not create")));
        var row = new LocSoftMainRow { Id = 7 };
        Assert.True(behavior.ActivateFor(row));

        behavior.OnBeforeDelete(new MockDeleteHandler<LocSoftMainRow>
        {
            Row = row,
            Connection = new MockDbConnection(),
            UnitOfWork = new MockUnitOfWork(new MockDbConnection())
        });
    }
}

