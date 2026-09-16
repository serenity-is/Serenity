namespace Serenity.Services;

public class DataValidationTests
{
    private enum TestEnum
    {
        A = 1,
        B = 2
    }

    [TableName("DataValidationTest")]
    private class TestRow : Row<TestRow.RowFields>, IRow
    {
        [IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }
        public int? Number { get => fields.Number[this]; set => fields.Number[this] = value; }
        public DateTime? Date { get => fields.Date[this]; set => fields.Date[this] = value; }
        public int? WithDefault { get => fields.WithDefault[this]; set => fields.WithDefault[this] = value; }
        public DateTime? Start { get => fields.Start[this]; set => fields.Start[this] = value; }
        public DateTime? Finish { get => fields.Finish[this]; set => fields.Finish[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID = null!;
            public StringField Name = null!;
            public Int32Field Number = null!;
            public DateTimeField Date = null!;
            public Int32Field WithDefault = null!;
            public DateTimeField Start = null!;
            public DateTimeField Finish = null!;
        }

        public TestRow()
        {
        }

        public TestRow(RowFields fields)
            : base(fields)
        {
        }
    }

    private static TestRow NewRow()
    {
        var fields = new TestRow.RowFields();
        fields.Initialize(annotations: null, dialect: SqlSettings.DefaultDialect);
        return new TestRow(fields);
    }

    [Fact]
    public void AutoTrim_TrimToEmpty_Trims_And_Empties()
    {
        var row = NewRow();
        row.Name = "  abc  ";
        var field = row.GetFields().Name;
        field.Flags = FieldFlags.Trim | FieldFlags.TrimToEmpty;

        DataValidation.AutoTrim(row, field);

        Assert.Equal("abc", row.Name);
    }

    [Fact]
    public void AutoTrim_TrimToNull_Returns_Null_For_Whitespace()
    {
        var row = NewRow();
        row.Name = "   ";
        var field = row.GetFields().Name;
        field.Flags = FieldFlags.Trim;

        DataValidation.AutoTrim(row, field);

        Assert.Null(row.Name);
    }

    [Fact]
    public void AutoTrim_Does_Nothing_Without_Trim_Flag()
    {
        var row = NewRow();
        row.Name = "  abc  ";
        var field = row.GetFields().Name;
        field.Flags = 0;

        DataValidation.AutoTrim(row, field);

        Assert.Equal("  abc  ", row.Name);
    }

    [Fact]
    public void ValidateRequired_Throws_For_Empty_String()
    {
        var row = NewRow();
        row.Name = "";
        Assert.Throws<ValidationError>(() => row.ValidateRequired(row.GetFields().Name, null));
    }

    [Fact]
    public void ValidateRequired_Throws_For_Null_Value()
    {
        var row = NewRow();
        Assert.Throws<ValidationError>(() => row.ValidateRequired(row.GetFields().Number, null));
    }

    [Fact]
    public void ValidateRequired_Passes_For_Value()
    {
        var row = NewRow();
        row.Name = "x";
        row.Number = 1;
        row.ValidateRequired(row.GetFields().Name, null);
        row.ValidateRequired(row.GetFields().Number, null);
    }

    [Fact]
    public void ValidateRequired_Enumerable_Skips_Unassigned_Default()
    {
        var row = NewRow();
        var fields = row.GetFields();
        row.Name = "x";
        fields.WithDefault.DefaultValue = 5;
        row.ValidateRequired([fields.Name, fields.WithDefault], null);
    }

    [Fact]
    public void ValidateRequired_Enumerable_Validates_Assigned_With_Default()
    {
        var row = NewRow();
        var fields = row.GetFields();
        row.Name = "x";
        row.WithDefault = null;
        fields.WithDefault.DefaultValue = 5;
        Assert.Throws<ValidationError>(() => row.ValidateRequired([fields.Name, fields.WithDefault], null));
    }

    [Fact]
    public void ValidateRequiredIfModified_Only_Validates_Assigned()
    {
        var row = NewRow();
        var fields = row.GetFields();
        row.Name = "x";
        row.ValidateRequiredIfModified([fields.Number], null);

        row.Number = null;
        Assert.Throws<ValidationError>(() => row.ValidateRequiredIfModified([fields.Number], null));
    }

    [Fact]
    public void EnsureUniversalTime_Converts_Value()
    {
        var row = NewRow();
        row.Date = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Local);
        DataValidation.EnsureUniversalTime(row, row.GetFields().Date);
        Assert.Equal(DateTimeKind.Utc, row.Date.Value.Kind);
    }

    [Fact]
    public void EnsureUniversalTime_Does_Nothing_For_Null()
    {
        var row = NewRow();
        DataValidation.EnsureUniversalTime(row, row.GetFields().Date);
        Assert.Null(row.Date);
    }

    [Fact]
    public void ValidateEnum_Field_Throws_For_Undefined_Value()
    {
        var row = NewRow();
        row.Number = 5;
        Assert.Throws<ValidationError>(() =>
            DataValidation.ValidateEnum(row, row.GetFields().Number, typeof(TestEnum), null));
    }

    [Fact]
    public void ValidateEnum_Field_Passes_For_Defined_Value()
    {
        var row = NewRow();
        row.Number = 1;
        DataValidation.ValidateEnum(row, row.GetFields().Number, typeof(TestEnum), null);
        var empty = NewRow();
        DataValidation.ValidateEnum(empty, empty.GetFields().Number, typeof(TestEnum), null);
    }

    [Fact]
    public void ValidateEnum_Generic_Field_Throws_For_Undefined_Value()
    {
        var row = NewRow();
        row.Number = 5;
        var field = row.GetFields().Number;
        field.EnumType = typeof(TestEnum);
        Assert.Throws<ValidationError>(() => DataValidation.ValidateEnum(row, field, null));
    }

    [Fact]
    public void ValidateEnum_Generic_Field_Passes_Without_EnumType()
    {
        var row = NewRow();
        row.Number = 5;
        row.GetFields().Number.EnumType = null;
        DataValidation.ValidateEnum(row, row.GetFields().Number, null);
    }

    [Fact]
    public void ValidateEnum_Generic_Value_Throws_For_Undefined()
    {
        Assert.Throws<ValidationError>(() => DataValidation.ValidateEnum((TestEnum)999, null));
        DataValidation.ValidateEnum(TestEnum.A, null);
    }

    [Fact]
    public void ValidateDateRange_Throws_When_Start_After_Finish()
    {
        var row = NewRow();
        row.Start = new DateTime(2020, 2, 1);
        row.Finish = new DateTime(2020, 1, 1);
        Assert.Throws<ValidationError>(() => DataValidation.ValidateDateRange(row, row.GetFields().Start, row.GetFields().Finish, null));

        var ok = NewRow();
        ok.Start = new DateTime(2020, 1, 1);
        ok.Finish = new DateTime(2020, 2, 1);
        DataValidation.ValidateDateRange(ok, ok.GetFields().Start, ok.GetFields().Finish, null);
        var empty = NewRow();
        DataValidation.ValidateDateRange(empty, empty.GetFields().Start, empty.GetFields().Finish, null);
    }

    [Fact]
    public void Error_Factories_Return_ValidationErrors()
    {
        var row = NewRow();
        var fields = row.GetFields();
        var field = fields.Name;
        Assert.NotNull(DataValidation.RequiredError(field, null));
        Assert.NotNull(DataValidation.RequiredError("x", null));
        Assert.NotNull(DataValidation.RequiredError("x", null, "T"));

        row.Name = "n";
        row.Number = 1;
        Assert.NotNull(DataValidation.InvalidIdError(row, field, null));
        Assert.NotNull(DataValidation.InvalidIdError(field, 1, null));
        Assert.NotNull(DataValidation.InvalidDateRangeError(fields.Date, fields.Date, null));
        Assert.NotNull(DataValidation.ReadOnlyError(field, null));
        Assert.NotNull(DataValidation.InvalidValueError(field, 1, null));
        Assert.NotNull(DataValidation.InvalidValueError(row, field, null));
        Assert.NotNull(DataValidation.EntityNotFoundError(row, 1, null));
        Assert.NotNull(DataValidation.EntityReadAccessError(row, 1, null));
        Assert.NotNull(DataValidation.EntityWriteAccessError(row, 1, null));
        Assert.NotNull(DataValidation.RelatedRecordExist("T", null));
        Assert.NotNull(DataValidation.ParentRecordDeleted("T", null));
        Assert.NotNull(DataValidation.RecordNotActive(row, null));
        Assert.NotNull(DataValidation.UnexpectedError(null));
        Assert.Equal("T", DataValidation.GetEntitySingular("T", null));
        Assert.NotNull(DataValidation.ArgumentNull("x", null));
        Assert.NotNull(DataValidation.ArgumentOutOfRange("x", null));
    }

    [Fact]
    public void GetEntitySingular_Uses_Localizer()
    {
        var localizer = new MockTextLocalizer(_ => "Entity");
        Assert.Equal("Entity", DataValidation.GetEntitySingular("T", localizer));
    }
}
