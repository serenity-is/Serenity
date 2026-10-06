using System.Data.Common;

namespace Serenity.Services.SqlErrors;

public class DefaultSqlErrorExtractorTests
{
    private sealed class MockDbException(string message, string? sqlState = null, int? number = null)
        : DbException(message)
    {
        public override string? SqlState { get; } = sqlState;

        public int? Number { get; } = number;
    }

    private sealed class ServerTypedExtractor(ServerType serverType) : DefaultSqlErrorExtractor
    {
        protected override ServerType? GetServerType(DbException exception) => serverType;
    }

    [Fact]
    public void Extract_Returns_Null_For_Unknown_Exception()
    {
        Assert.Null(new DefaultSqlErrorExtractor().Extract(new InvalidOperationException("boom")));
    }

    [Fact]
    public void Extract_Unwraps_Inner_Exception()
    {
        var info = new DefaultSqlErrorExtractor().Extract(
            new InvalidOperationException("outer", new MockDbException("x", sqlState: "23505")));

        Assert.NotNull(info);
        Assert.Equal(SqlErrorConstraintType.Unique, info!.Type);
    }

    [Fact]
    public void Extract_Uses_ServerType_From_Options_Over_Inference()
    {
        var extractor = new DefaultSqlErrorExtractor();

        Assert.Null(extractor.Extract(new MockDbException("x", number: 2627)));

        var info = extractor.Extract(new MockDbException("x", number: 2627),
            new SqlErrorExtractOptions { ServerType = "SqlServer" });

        Assert.NotNull(info);
        Assert.Equal(SqlErrorConstraintType.PrimaryKey, info!.Type);
        Assert.Equal(ServerType.SqlServer, info.ServerType);
    }

    [Theory]
    [InlineData("23505", SqlErrorConstraintType.Unique)]
    [InlineData("23503", SqlErrorConstraintType.ForeignKey)]
    [InlineData("23502", SqlErrorConstraintType.NotNull)]
    public void Extract_Uses_SqlState(string sqlState, SqlErrorConstraintType expected)
    {
        var info = new DefaultSqlErrorExtractor().Extract(new MockDbException("x", sqlState: sqlState));

        Assert.NotNull(info);
        Assert.Equal(expected, info!.Type);
    }

    [Theory]
    [InlineData(ServerType.SqlServer, 2627, SqlErrorConstraintType.PrimaryKey)]
    [InlineData(ServerType.SqlServer, 2601, SqlErrorConstraintType.Unique)]
    [InlineData(ServerType.SqlServer, 547, SqlErrorConstraintType.ForeignKey)]
    [InlineData(ServerType.MySql, 1062, SqlErrorConstraintType.Unique)]
    [InlineData(ServerType.MySql, 1451, SqlErrorConstraintType.ForeignKey)]
    [InlineData(ServerType.Oracle, 1, SqlErrorConstraintType.Unique)]
    [InlineData(ServerType.Oracle, 2291, SqlErrorConstraintType.ForeignKey)]
    [InlineData(ServerType.Firebird, 335544349, SqlErrorConstraintType.Unique)]
    [InlineData(ServerType.Firebird, 335544466, SqlErrorConstraintType.ForeignKey)]
    [InlineData(ServerType.Sqlite, 787, SqlErrorConstraintType.ForeignKey)]
    public void Extract_Uses_Provider_Error_Number(ServerType serverType, int number, SqlErrorConstraintType expected)
    {
        var info = new ServerTypedExtractor(serverType).Extract(new MockDbException("x", number: number));

        Assert.NotNull(info);
        Assert.Equal(expected, info!.Type);
        Assert.Equal(serverType, info.ServerType);
    }

    [Fact]
    public void Extract_Parses_SqlServer_PrimaryKey()
    {
        const string message = "Violation of PRIMARY KEY constraint 'PK_SomeTable'. " +
            "Cannot insert duplicate key in object 'dbo.SomeTable'. The duplicate key value is (7005950).";

        var info = new ServerTypedExtractor(ServerType.SqlServer)
            .Extract(new MockDbException(message, number: 2627));

        Assert.NotNull(info);
        Assert.Equal(SqlErrorConstraintType.PrimaryKey, info!.Type);
        Assert.Equal("SomeTable", info.TableName);
        Assert.Equal("PK_SomeTable", info.ConstraintName);
        Assert.Equal("7005950", info.KeyValue);
    }

    [Fact]
    public void Extract_Parses_SqlServer_ForeignKey()
    {
        const string message = "The DELETE statement conflicted with the REFERENCE constraint " +
            "\"FK_SomeTable_SomeFieldID\". The conflict occurred in database \"DBSome\", " +
            "table \"dbo.SomeTable\", column 'SomeFieldID'.";

        var info = new ServerTypedExtractor(ServerType.SqlServer)
            .Extract(new MockDbException(message, number: 547));

        Assert.NotNull(info);
        Assert.Equal(SqlErrorConstraintType.ForeignKey, info!.Type);
        Assert.Equal("SomeTable", info.TableName);
        Assert.Equal("SomeFieldID", info.ColumnName);
    }

    [Fact]
    public void Extract_Parses_Sqlite_Unique()
    {
        var info = new DefaultSqlErrorExtractor().Extract(
            new MockDbException("SQLite Error 19: 'UNIQUE constraint failed: SomeTable.Code'."));

        Assert.NotNull(info);
        Assert.Equal(SqlErrorConstraintType.Unique, info!.Type);
        Assert.Equal("SomeTable", info.TableName);
        Assert.Equal("Code", info.ColumnName);
    }

    [Fact]
    public void Extract_Parses_MySql_Unique()
    {
        var info = new DefaultSqlErrorExtractor().Extract(
            new MockDbException("Duplicate entry 'X' for key 'SomeTable.IX_Code'"));

        Assert.NotNull(info);
        Assert.Equal(SqlErrorConstraintType.Unique, info!.Type);
        Assert.Equal("SomeTable.IX_Code", info.ConstraintName);
    }

    [Fact]
    public void Extract_Parses_Localized_SqlServer_NotNull_German()
    {
        var info = new ServerTypedExtractor(ServerType.SqlServer).Extract(new MockDbException(
            "Der Wert NULL kann in die ParentId-Spalte, dbo.Child-Tabelle nicht eingefügt werden. " +
            "Die Spalte lässt NULL-Werte nicht zu. Fehler bei INSERT.", number: 515));

        Assert.NotNull(info);
        Assert.Equal(SqlErrorConstraintType.NotNull, info!.Type);
        Assert.Equal("Child", info.TableName);
        Assert.Equal("ParentId", info.ColumnName);
    }

    [Fact]
    public void Extract_Parses_Localized_SqlServer_ForeignKey_German()
    {
        var info = new ServerTypedExtractor(ServerType.SqlServer).Extract(new MockDbException(
            "Die DELETE-Anweisung steht in Konflikt mit der REFERENCE-Einschränkung \"FK_Orders\". " +
            "Der Konflikt trat in der TestDB-Datenbank, Tabelle \"dbo.Child\", Spalte 'ParentId' auf.", number: 547));

        Assert.NotNull(info);
        Assert.Equal(SqlErrorConstraintType.ForeignKey, info!.Type);
        Assert.Equal("Child", info.TableName);
        Assert.Equal("FK_Orders", info.ConstraintName);
    }

    [Fact]
    public void Extract_Parses_Localized_SqlServer_NotNull_Turkish()
    {
        var info = new ServerTypedExtractor(ServerType.SqlServer).Extract(new MockDbException(
            "NULL değeri 'dbo.Child' tablosunun 'ParentId' sütununa eklemez; " +
            "sütun null değerlere izin vermiyor. INSERT başarısız.", number: 515));

        Assert.NotNull(info);
        Assert.Equal(SqlErrorConstraintType.NotNull, info!.Type);
        Assert.Equal("Child", info.TableName);
        Assert.Equal("ParentId", info.ColumnName);
    }

    [Fact]
    public void Extract_Parses_Localized_SqlServer_Unique_Japanese()
    {
        var info = new ServerTypedExtractor(ServerType.SqlServer).Extract(new MockDbException(
            "一意インデックス 'IX_Code' を含むオブジェクト 'dbo.Child' には重複するキー行を挿入できません。" +
            "重複するキーの値は X です。", number: 2601));

        Assert.NotNull(info);
        Assert.Equal(SqlErrorConstraintType.Unique, info!.Type);
        Assert.Equal("Child", info.TableName);
        Assert.Equal("IX_Code", info.ConstraintName);
        Assert.Equal("X", info.KeyValue);
    }

    [Fact]
    public void Extract_Parses_Localized_SqlServer_PrimaryKey_Russian()
    {
        var info = new ServerTypedExtractor(ServerType.SqlServer).Extract(new MockDbException(
            "Нарушено \"PK_Child\" ограничения PRIMARY KEY. " +
            "Не удается вставить повторяющийся ключ в объект \"dbo.Child\". " +
            "Повторяющееся значение ключа: 1.", number: 2627));

        Assert.NotNull(info);
        Assert.Equal(SqlErrorConstraintType.PrimaryKey, info!.Type);
        Assert.Equal("Child", info.TableName);
        Assert.Equal("PK_Child", info.ConstraintName);
        Assert.Equal("1", info.KeyValue);
    }
}

public class SqlErrorInfoTests
{
    private sealed class TestLocalizer : ITextLocalizer
    {
        private readonly Dictionary<string, string> map = new(StringComparer.Ordinal);

        public TestLocalizer Add(LocalText text, string value)
        {
            map[text.Key] = value;
            return this;
        }

        public string? TryGet(string key) => map.TryGetValue(key, out var value) ? value : null;
    }

    private static TestLocalizer CreateLocalizer() => new TestLocalizer()
        .Add(SqlErrorValidationTexts.SavePrimaryKeyError,
            "Can't save record. There is another record with the same {1} value!")
        .Add(SqlErrorValidationTexts.SavePrimaryKeyErrorGeneric,
            "Can't save record. There is another record with the same value(s)!")
        .Add(SqlErrorValidationTexts.DeleteForeignKeyError,
            "Can't delete record. '{0}' table has records that depends on this one!")
        .Add(SqlErrorValidationTexts.DeleteForeignKeyErrorGeneric,
            "Can't delete record. There are related records that depend on this one!")
        .Add(SqlErrorValidationTexts.SaveForeignKeyError,
            "Can't save record. Referenced '{0}' record does not exist!")
        .Add(SqlErrorValidationTexts.SaveForeignKeyErrorGeneric,
            "Can't save record. A referenced record does not exist!")
        .Add(DataValidationTexts.FieldIsRequired, "{0} field is required!");

    [Fact]
    public void ToString_Formats_PrimaryKey_With_Field_Name()
    {
        var info = new SqlErrorInfo { Type = SqlErrorConstraintType.Unique, TableName = "Users" };

        var text = info.ToString(CreateLocalizer(), new SqlErrorFormatOptions { FieldName = "Email" });

        Assert.Equal("Can't save record. There is another record with the same Email value!", text);
    }

    [Fact]
    public void ToString_Formats_Delete_ForeignKey()
    {
        var info = new SqlErrorInfo { Type = SqlErrorConstraintType.ForeignKey, TableName = "Orders" };

        var text = info.ToString(CreateLocalizer(), new SqlErrorFormatOptions { Operation = SqlErrorOperation.Delete });

        Assert.Equal("Can't delete record. 'Orders' table has records that depends on this one!", text);
    }

    [Fact]
    public void ToString_Formats_Save_ForeignKey()
    {
        var info = new SqlErrorInfo { Type = SqlErrorConstraintType.ForeignKey, ReferencedTableName = "Customers" };

        var text = info.ToString(CreateLocalizer(), new SqlErrorFormatOptions { Operation = SqlErrorOperation.Insert });

        Assert.Equal("Can't save record. Referenced 'Customers' record does not exist!", text);
    }

    [Fact]
    public void ToString_Uses_Custom_ErrorMessage()
    {
        var info = new SqlErrorInfo { Type = SqlErrorConstraintType.Unique, TableName = "Users" };

        var text = info.ToString(CreateLocalizer(), new SqlErrorFormatOptions
        {
            ErrorMessage = "Custom {0}.{1}",
            FieldName = "Email"
        });

        Assert.Equal("Custom Users.Email", text);
    }

    [Fact]
    public void ToString_Falls_Back_To_Generic_PrimaryKey_Message()
    {
        var info = new SqlErrorInfo { Type = SqlErrorConstraintType.Unique };
        var localizer = CreateLocalizer();

        var text = info.ToString(localizer, new SqlErrorFormatOptions());

        Assert.Equal(SqlErrorValidationTexts.SavePrimaryKeyErrorGeneric.ToString(localizer), text);
    }

    [Fact]
    public void ToString_Falls_Back_To_Generic_Delete_ForeignKey_Message()
    {
        var info = new SqlErrorInfo { Type = SqlErrorConstraintType.ForeignKey };
        var localizer = CreateLocalizer();

        var text = info.ToString(localizer, new SqlErrorFormatOptions { Operation = SqlErrorOperation.Delete });

        Assert.Equal(SqlErrorValidationTexts.DeleteForeignKeyErrorGeneric.ToString(localizer), text);
    }

    [Fact]
    public void ToString_Falls_Back_To_Generic_Save_ForeignKey_Message()
    {
        var info = new SqlErrorInfo { Type = SqlErrorConstraintType.ForeignKey };
        var localizer = CreateLocalizer();

        var text = info.ToString(localizer, new SqlErrorFormatOptions { Operation = SqlErrorOperation.Update });

        Assert.Equal(SqlErrorValidationTexts.SaveForeignKeyErrorGeneric.ToString(localizer), text);
    }

    [Fact]
    public void ToString_Throws_For_Null_Localizer()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlErrorInfo().ToString(null!));
    }
}
