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
#pragma warning disable CS0649
            public Int32Field ID;
            public StringField Name;
            public Int32Field Number;
            public DateTimeField Date;
            public Int32Field WithDefault;
            public DateTimeField Start;
            public DateTimeField Finish;
#pragma warning restore CS0649
        }
    }

    private static TestRow.RowFields Fields => new TestRow().GetFields();

    [Fact]
    public void AutoTrim_TrimToEmpty_Trims_And_Empties()
    {
        var row = new TestRow { Name = "  abc  " };
        var field = Fields.Name;
        field.Flags = FieldFlags.Trim | FieldFlags.TrimToEmpty;

        DataValidation.AutoTrim(row, field);

        Assert.Equal("abc", row.Name);
    }

    [Fact]
    public void AutoTrim_TrimToNull_Returns_Null_For_Whitespace()
    {
        var row = new TestRow { Name = "   " };
        var field = Fields.Name;
        field.Flags = FieldFlags.Trim;

        DataValidation.AutoTrim(row, field);

        Assert.Null(row.Name);
    }

    [Fact]
    public void AutoTrim_Does_Nothing_Without_Trim_Flag()
    {
        var row = new TestRow { Name = "  abc  " };
        var field = Fields.Name;
        field.Flags = 0;

        DataValidation.AutoTrim(row, field);

        Assert.Equal("  abc  ", row.Name);
    }

    [Fact]
    public void ValidateRequired_Throws_For_Empty_String()
    {
        var row = new TestRow { Name = "" };
        Assert.Throws<ValidationError>(() => row.ValidateRequired(Fields.Name, null));
    }

    [Fact]
    public void ValidateRequired_Throws_For_Null_Value()
    {
        var row = new TestRow();
        Assert.Throws<ValidationError>(() => row.ValidateRequired(Fields.Number, null));
    }

    [Fact]
    public void ValidateRequired_Passes_For_Value()
    {
        var row = new TestRow { Name = "x", Number = 1 };
        row.ValidateRequired(Fields.Name, null);
        row.ValidateRequired(Fields.Number, null);
    }

    [Fact]
    public void ValidateRequired_Enumerable_Skips_Unassigned_Default()
    {
        var row = new TestRow { Name = "x" };
        Fields.WithDefault.DefaultValue = 5;
        row.ValidateRequired([Fields.Name, Fields.WithDefault], null);
    }

    [Fact]
    public void ValidateRequired_Enumerable_Validates_Assigned_With_Default()
    {
        var row = new TestRow { Name = "x", WithDefault = null };
        Fields.WithDefault.DefaultValue = 5;
        Assert.Throws<ValidationError>(() => row.ValidateRequired([Fields.Name, Fields.WithDefault], null));
    }

    [Fact]
    public void ValidateRequiredIfModified_Only_Validates_Assigned()
    {
        var row = new TestRow { Name = "x" };
        row.ValidateRequiredIfModified([Fields.Number], null);

        row.Number = null;
        Assert.Throws<ValidationError>(() => row.ValidateRequiredIfModified([Fields.Number], null));
    }

    [Fact]
    public void EnsureUniversalTime_Converts_Value()
    {
        var row = new TestRow { Date = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Local) };
        DataValidation.EnsureUniversalTime(row, Fields.Date);
        Assert.Equal(DateTimeKind.Utc, row.Date.Value.Kind);
    }

    [Fact]
    public void EnsureUniversalTime_Does_Nothing_For_Null()
    {
        var row = new TestRow();
        DataValidation.EnsureUniversalTime(row, Fields.Date);
        Assert.Null(row.Date);
    }

    [Fact]
    public void ValidateEnum_Field_Throws_For_Undefined_Value()
    {
        var row = new TestRow { Number = 5 };
        Assert.Throws<ValidationError>(() =>
            DataValidation.ValidateEnum(row, Fields.Number, typeof(TestEnum), null));
    }

    [Fact]
    public void ValidateEnum_Field_Passes_For_Defined_Value()
    {
        var row = new TestRow { Number = 1 };
        DataValidation.ValidateEnum(row, Fields.Number, typeof(TestEnum), null);
        DataValidation.ValidateEnum(new TestRow(), Fields.Number, typeof(TestEnum), null);
    }

    [Fact]
    public void ValidateEnum_Generic_Field_Throws_For_Undefined_Value()
    {
        var row = new TestRow { Number = 5 };
        var field = Fields.Number;
        field.EnumType = typeof(TestEnum);
        Assert.Throws<ValidationError>(() => DataValidation.ValidateEnum(row, field, null));
    }

    [Fact]
    public void ValidateEnum_Generic_Field_Passes_Without_EnumType()
    {
        var row = new TestRow { Number = 5 };
        Fields.Number.EnumType = null;
        DataValidation.ValidateEnum(row, Fields.Number, null);
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
        var row = new TestRow
        {
            Start = new DateTime(2020, 2, 1),
            Finish = new DateTime(2020, 1, 1)
        };
        Assert.Throws<ValidationError>(() => DataValidation.ValidateDateRange(row, Fields.Start, Fields.Finish, null));

        var ok = new TestRow
        {
            Start = new DateTime(2020, 1, 1),
            Finish = new DateTime(2020, 2, 1)
        };
        DataValidation.ValidateDateRange(ok, Fields.Start, Fields.Finish, null);
        DataValidation.ValidateDateRange(new TestRow(), Fields.Start, Fields.Finish, null);
    }

    [Fact]
    public void Error_Factories_Return_ValidationErrors()
    {
        var field = Fields.Name;
        Assert.NotNull(DataValidation.RequiredError(field, null));
        Assert.NotNull(DataValidation.RequiredError("x", null));
        Assert.NotNull(DataValidation.RequiredError("x", null, "T"));

        var row = new TestRow { Name = "n", Number = 1 };
        Assert.NotNull(DataValidation.InvalidIdError(row, field, null));
        Assert.NotNull(DataValidation.InvalidIdError(field, 1, null));
        Assert.NotNull(DataValidation.InvalidDateRangeError(Fields.Date, Fields.Date, null));
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

