namespace Serenity.Services;

public class SaveRequestHandlerTests_RequiredWhen
{
    [ReadPermission(SpecialPermissionKeys.Public)]
    private class ConditionalRow : Row<ConditionalRow.RowFields>, IIdRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public string? Kind { get => fields.Kind[this]; set => fields.Kind[this] = value; }

        [DisplayName("Company Name"), RequiredWhen(nameof(Kind), "company")]
        public string? Company { get => fields.Company[this]; set => fields.Company[this] = value; }

        // only required in the forms, as it is not "not null" in the database
        [Required]
        public string? Note { get => fields.Note[this]; set => fields.Note[this] = value; }

        [VisibleWhen(nameof(Kind), "person"), RequiredWhen(nameof(Kind), "company")]
        public string? HiddenForCompanies { get => fields.HiddenForCompanies[this]; set => fields.HiddenForCompanies[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Kind;
            public StringField Company;
            public StringField Note;
            public StringField HiddenForCompanies;
        }
    }

    private class Handler(IRequestContext context) :
        SaveRequestHandler<ConditionalRow, SaveRequest<ConditionalRow>, SaveResponse>(context)
    {
        public void SetRow(ConditionalRow row) => Row = row;
        public void SetOld(ConditionalRow old) => Old = old;
        public void SetUow(IUnitOfWork uow) => UnitOfWork = uow;
        public void DoValidateRequiredWhen() => ValidateRequiredWhen();
    }

    private static Handler NewHandler()
    {
        var handler = new Handler(new NullRequestContext().WithPermissions(_ => true));
        handler.SetUow(new MockUnitOfWork(new MockDbConnection()));
        return handler;
    }

    [Fact]
    public void A_Field_Required_By_Its_Condition_Is_Required_On_Insert_When_It_Is_Met()
    {
        var handler = NewHandler();
        handler.SetRow(new ConditionalRow { Kind = "company" });

        var error = Assert.Throws<ValidationError>(handler.DoValidateRequiredWhen);
        Assert.Equal("Required", error.ErrorCode);
        Assert.Equal("Company", error.Arguments);

        handler.SetRow(new ConditionalRow { Kind = "person" });
        handler.DoValidateRequiredWhen();
        handler.SetRow(new ConditionalRow { Kind = "company", Company = "Acme" });
        handler.DoValidateRequiredWhen();
    }

    [Fact]
    public void A_Required_Attribute_Of_The_Row_Is_Still_Only_For_The_Forms()
    {
        var handler = NewHandler();
        handler.SetRow(new ConditionalRow { Kind = "person", Note = null });
        handler.DoValidateRequiredWhen();
    }

    [Fact]
    public void An_Update_Only_Checks_The_Fields_It_Has_With_The_Old_Values_For_The_Conditions()
    {
        var handler = NewHandler();
        handler.SetOld(new ConditionalRow { Id = 1, Kind = "company", Company = "Acme" });
        handler.SetRow(new ConditionalRow { Id = 1, Note = "x" });
        handler.DoValidateRequiredWhen();

        handler.SetRow(new ConditionalRow { Id = 1, Company = "" });
        Assert.Equal("Company", Assert.Throws<ValidationError>(handler.DoValidateRequiredWhen).Arguments);
    }

    [Fact]
    public void A_Field_Hidden_By_Its_Condition_Is_Not_Checked()
    {
        var handler = NewHandler();
        handler.SetRow(new ConditionalRow { Kind = "company", Company = "Acme", HiddenForCompanies = null });
        handler.DoValidateRequiredWhen();
    }
}
