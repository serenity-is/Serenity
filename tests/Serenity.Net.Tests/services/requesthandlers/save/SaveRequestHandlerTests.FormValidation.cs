using System.Threading;

namespace Serenity.Services;

public class SaveRequestHandlerTests_FormValidation
{
    [ReadPermission(SpecialPermissionKeys.Public)]
    [FormValidation(typeof(RowForm))]
    private class FormRow : Row<FormRow.RowFields>, IIdRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [DisplayName("Customer Name")]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public string? Kind { get => fields.Kind[this]; set => fields.Kind[this] = value; }

        public string? Company { get => fields.Company[this]; set => fields.Company[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public StringField Name = null!;
            public StringField Kind = null!;
            public StringField Company = null!;
        }
    }

    [FormScript("RowForm")]
    private class RowForm
    {
        [Required]
        public string? Name { get; set; }
    }

    [FormScript("CompanyForm")]
    private class CompanyForm
    {
        public string? Kind { get; set; }

        [RequiredWhen(nameof(Kind), "company")]
        public string? Company { get; set; }
    }

    private class Handler(IRequestContext context) :
        SaveRequestHandler<FormRow, SaveRequest<FormRow>, SaveResponse>(context)
    {
        public void SetRow(FormRow row) => Row = row;
        public void SetOld(FormRow old) => Old = old;
        public void SetUow(IUnitOfWork uow) => UnitOfWork = uow;
        public Type? DoGetValidationForm() => GetValidationForm();
        public void DoValidateForm(Type formType) => ValidateForm(formType);
    }

    [FormValidation(typeof(CompanyForm))]
    private class HandlerWithForm(IRequestContext context) : Handler(context)
    {
    }

    private class AsyncHandler(IRequestContext context) :
        SaveRequestHandlerAsync<FormRow, SaveRequest<FormRow>, SaveResponse>(context)
    {
        public void SetRow(FormRow row) => Row = row;
        public void SetUow(IUnitOfWork uow) => UnitOfWork = uow;
        public Task DoValidateFormAsync(Type formType, CancellationToken token = default) => ValidateFormAsync(formType, token);
    }

    private static T New<T>(Func<IRequestContext, T> create) where T : class
    {
        var handler = create(new NullRequestContext().WithPermissions(_ => true));
        switch (handler)
        {
            case Handler h: h.SetUow(new MockUnitOfWork(new MockDbConnection())); break;
            case AsyncHandler a: a.SetUow(new MockUnitOfWork(new MockDbConnection())); break;
        }
        return handler;
    }

    [Fact]
    public void The_Form_Of_The_Handler_Comes_Before_The_One_Of_The_Row()
    {
        Assert.Equal(typeof(RowForm), New(c => new Handler(c)).DoGetValidationForm());
        Assert.Equal(typeof(CompanyForm), New(c => new HandlerWithForm(c)).DoGetValidationForm());
    }

    [Fact]
    public void A_Missing_Required_Field_Of_The_Form_Is_A_Required_Error_With_The_Title_Of_The_Field()
    {
        var handler = New(c => new Handler(c));
        handler.SetRow(new FormRow());

        var error = Assert.Throws<ValidationError>(() => handler.DoValidateForm(typeof(RowForm)));
        Assert.Equal("Required", error.ErrorCode);
        Assert.Equal("Name", error.Arguments);

        handler.SetRow(new FormRow { Name = "a" });
        handler.DoValidateForm(typeof(RowForm));
    }

    [Fact]
    public void An_Update_Only_Checks_The_Fields_It_Has()
    {
        var handler = New(c => new Handler(c));
        handler.SetOld(new FormRow { Id = 1, Name = "a" });
        handler.SetRow(new FormRow { Id = 1, Kind = "x" });
        handler.DoValidateForm(typeof(RowForm));

        handler.SetRow(new FormRow { Id = 1, Name = "" });
        Assert.Throws<ValidationError>(() => handler.DoValidateForm(typeof(RowForm)));
    }

    [Fact]
    public void Conditions_Of_The_Form_Read_The_Old_Values_Of_The_Fields_An_Update_Doesnt_Have()
    {
        var handler = New(c => new Handler(c));
        handler.SetOld(new FormRow { Id = 1, Kind = "company", Company = "Acme" });
        handler.SetRow(new FormRow { Id = 1, Company = "" });

        var error = Assert.Throws<ValidationError>(() => handler.DoValidateForm(typeof(CompanyForm)));
        Assert.Equal("Company", error.Arguments);
    }

    [Fact]
    public async Task The_Async_Handler_Validates_By_The_Form_Too()
    {
        var handler = New(c => new AsyncHandler(c));
        handler.SetRow(new FormRow { Kind = "company" });

        var error = await Assert.ThrowsAsync<ValidationError>(() => handler.DoValidateFormAsync(typeof(CompanyForm), TestContext.Current.CancellationToken));
        Assert.Equal("Company", error.Arguments);

        handler.SetRow(new FormRow { Kind = "person" });
        await handler.DoValidateFormAsync(typeof(CompanyForm), TestContext.Current.CancellationToken);
    }
}
