using System.Data.Common;

namespace Serenity.Services.SqlErrors;

public class HumanizeSqlExceptionBehaviorTests
{
    private sealed class MockDbException(string message, string? sqlState = null, int? number = null) : DbException(message)
    {
        public override string? SqlState { get; } = sqlState;
        public int? Number { get; } = number;
    }

    [TableName("IT_Humanize_Row")]
    private class HumanizeRow : Row<HumanizeRow.RowFields>, IIdRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
        }
    }

    private static HumanizeSqlExceptionBehavior CreateBehavior() =>
        new(new DefaultSqlErrorExtractor());

    private static MockSaveHandler<HumanizeRow> CreateSaveHandler(bool isCreate)
    {
        return new MockSaveHandler<HumanizeRow>
        {
            Connection = new MockDbConnection { Dialect = SqlServer2012Dialect.Instance },
            Context = new NullRequestContext(),
            Row = new HumanizeRow { Id = 1 },
            IsCreate = isCreate,
            IsUpdate = !isCreate
        };
    }

    private static MockDeleteHandler<HumanizeRow> CreateDeleteHandler()
    {
        return new MockDeleteHandler<HumanizeRow>
        {
            Connection = new MockDbConnection { Dialect = SqlServer2012Dialect.Instance },
            Context = new NullRequestContext(),
            Row = new HumanizeRow { Id = 1 }
        };
    }

    [Fact]
    public void Save_Humanizes_PrimaryKey_Violation()
    {
        var handler = CreateSaveHandler(isCreate: true);

        var error = Assert.Throws<ValidationError>(() => CreateBehavior().OnException(handler,
            new MockDbException(
                "Violation of PRIMARY KEY constraint 'PK_X'. Cannot insert duplicate key in object " +
                "'dbo.IT_Humanize_Row'. The duplicate key value is (1).", number: 2627)));

        Assert.Equal("UniqueViolation", error.ErrorCode);
        Assert.Contains("same", error.Message);
    }

    [Fact]
    public void Save_Humanizes_NotNull_Violation()
    {
        var handler = CreateSaveHandler(isCreate: false);

        var error = Assert.Throws<ValidationError>(() => CreateBehavior().OnException(handler,
            new MockDbException(
                "Cannot insert the value NULL into column 'Id', table 'dbo.IT_Humanize_Row'; " +
                "column does not allow nulls. UPDATE fails.", number: 515)));

        Assert.Equal("Required", error.ErrorCode);
    }

    [Fact]
    public void Delete_Humanizes_ForeignKey_Violation()
    {
        var handler = CreateDeleteHandler();

        var error = Assert.Throws<ValidationError>(() => CreateBehavior().OnException(handler,
            new MockDbException(
                "The DELETE statement conflicted with the REFERENCE constraint \"FK_X\". " +
                "The conflict occurred in database \"db\", table \"dbo.Child\", column 'ParentId'.", number: 547)));

        Assert.Equal("RelatedRecordExist", error.ErrorCode);
        Assert.Contains("Child", error.Message);
    }

    [Fact]
    public void Save_Ignores_NonConstraint_Exception()
    {
        var handler = CreateSaveHandler(isCreate: true);

        CreateBehavior().OnException(handler, new InvalidOperationException("boom"));
    }

    [Fact]
    public void Save_NotNull_Without_Parsed_Column_Does_Not_Use_Id_Title()
    {
        var handler = CreateSaveHandler(isCreate: true);

        var error = Assert.Throws<ValidationError>(() => CreateBehavior().OnException(handler,
            new MockDbException(
                "null value in column \"Name\" of relation \"Child\" violates not-null constraint",
                sqlState: "23502")));

        Assert.Equal("Required", error.ErrorCode);
        Assert.Equal("??? field is required!", error.Message);
    }

    [Fact]
    public void Uses_Connection_Dialect_ServerType()
    {
        var handler = CreateSaveHandler(isCreate: true);

        // the mock exception has no recognizable provider type, but the connection
        // dialect is SqlServer so the error number should still be classified
        var error = Assert.Throws<ValidationError>(() => CreateBehavior().OnException(handler,
            new MockDbException("x", number: 2627)));

        Assert.Equal("UniqueViolation", error.ErrorCode);
    }
}
