namespace Serenity.Extensions.Entities;

public class LoggingRowTests
{
    private class TestLoggingRow : LoggingRow<TestLoggingRow.RowFields>
    {
        public TestLoggingRow()
        {
        }

        public TestLoggingRow(RowFields fields) : base(fields)
        {
        }

        public class RowFields : LoggingRowFields
        {
        }
    }

    [Fact]
    public void Constructor_With_Fields_Sets_Fields()
    {
        var fields = new TestLoggingRow.RowFields();
        fields.Initialize(null, SqlSettings.DefaultDialect);
        var row = new TestLoggingRow(fields);
        Assert.Same(fields, row.GetFields());
    }

    [Fact]
    public void Properties_Roundtrip()
    {
        var date = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        var row = new TestLoggingRow
        {
            InsertUserId = 1,
            InsertDate = date,
            UpdateUserId = 2,
            UpdateDate = date
        };

        Assert.Equal(1, row.InsertUserId);
        Assert.Equal(date, row.InsertDate);
        Assert.Equal(2, row.UpdateUserId);
        Assert.Equal(date, row.UpdateDate);
    }

    [Fact]
    public void Interface_Field_Properties_Return_Fields()
    {
        var row = new TestLoggingRow();
        var fields = row.GetFields();
        Assert.Same(fields.InsertDate, ((IInsertDateRow)row).InsertDateField);
        Assert.Same(fields.InsertUserId, ((IInsertUserIdRow)row).InsertUserIdField);
        Assert.Same(fields.UpdateDate, ((IUpdateDateRow)row).UpdateDateField);
        Assert.Same(fields.UpdateUserId, ((IUpdateUserIdRow)row).UpdateUserIdField);
    }

    [Fact]
    public void Fields_Have_Expected_Names()
    {
        Assert.Equal("InsertUserId", TestLoggingRow.Fields.InsertUserId.Name);
        Assert.Equal("InsertDate", TestLoggingRow.Fields.InsertDate.Name);
        Assert.Equal("UpdateUserId", TestLoggingRow.Fields.UpdateUserId.Name);
        Assert.Equal("UpdateDate", TestLoggingRow.Fields.UpdateDate.Name);
    }
}
