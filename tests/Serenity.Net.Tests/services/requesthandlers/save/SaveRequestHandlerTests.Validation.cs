using System.ComponentModel.DataAnnotations;

namespace Serenity.Services;

public class SaveRequestHandlerTests_Validation
{
    private interface ICodeChecker
    {
        bool IsTaken(string code);
    }

    private class TakenCodes(params string[] codes) : ICodeChecker
    {
        public bool IsTaken(string code) => codes.Contains(code);
    }

    /// <summary>A rule that needs services of the request, the handler and its connection.</summary>
    private class ValidateFreeCodeAttribute : ValidationRuleAttribute
    {
        public static ISaveRequestHandler? Handler = null!;
        public static IDbConnection? Connection = null!;

        public override string Rule => "freeCode";

        protected override bool IsValidValue(object? value, ValidationContext context)
        {
            Handler = context.GetService(typeof(ISaveRequestHandler)) as ISaveRequestHandler;
            Connection = context.GetConnection();
            var checker = (ICodeChecker)context.GetService(typeof(ICodeChecker))!;
            return !checker.IsTaken((string)value!);
        }
    }

    [ReadPermission(SpecialPermissionKeys.Public)]
    private class ValidatedRow : Row<ValidatedRow.RowFields>, IIdRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [ValidateRange(1, 10)]
        public int? Quantity { get => fields.Quantity[this]; set => fields.Quantity[this] = value; }

        [EmailEditor]
        public string? Email { get => fields.Email[this]; set => fields.Email[this] = value; }

        [ValidateRange(1, 10, Side = ValidationSide.Client)]
        public int? Hint { get => fields.Hint[this]; set => fields.Hint[this] = value; }

        [ValidateFreeCode]
        public string? Code { get => fields.Code[this]; set => fields.Code[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public Int32Field Quantity = null!;
            public StringField Email = null!;
            public Int32Field Hint = null!;
            public StringField Code = null!;
        }
    }

    private class Handler(IRequestContext context) :
        SaveRequestHandler<ValidatedRow, SaveRequest<ValidatedRow>, SaveResponse>(context)
    {
        public void SetRow(ValidatedRow row) => Row = row;
        public void SetUow(IUnitOfWork uow) => UnitOfWork = uow;
        public void DoValidateFieldValues() => ValidateFieldValues();
    }

    private static Handler NewHandler(params string[] takenCodes)
    {
        var context = new NullRequestContext().WithPermissions(_ => true);
        context.ServiceProvider = new ServiceCollection().AddSingleton<ICodeChecker>(new TakenCodes(takenCodes)).BuildServiceProvider();
        var handler = new Handler(context);
        handler.SetUow(new MockUnitOfWork(new MockDbConnection()));
        return handler;
    }

    [Fact]
    public void Throws_For_A_Value_Out_Of_A_Range_With_The_Field_And_The_Message()
    {
        var handler = NewHandler();
        handler.SetRow(new ValidatedRow { Quantity = 11 });

        var error = Assert.Throws<ValidationError>(handler.DoValidateFieldValues);
        Assert.Equal("Quantity", error.Arguments);
        Assert.Equal("Please enter a value between 1 and 10.", error.Message);
    }

    [Fact]
    public void Accepts_Valid_Values_And_Skips_The_Unassigned_Fields()
    {
        var handler = NewHandler();
        handler.SetRow(new ValidatedRow { Quantity = 5, Email = "a@b.com" });
        handler.DoValidateFieldValues();
    }

    [Fact]
    public void Checks_The_Rule_An_Editor_Implies()
    {
        var handler = NewHandler();
        handler.SetRow(new ValidatedRow { Email = "invalid" });

        var error = Assert.Throws<ValidationError>(handler.DoValidateFieldValues);
        Assert.Equal("Email", error.Arguments);
    }

    [Fact]
    public void Skips_A_Rule_Only_Checked_By_The_Forms()
    {
        var handler = NewHandler();
        handler.SetRow(new ValidatedRow { Hint = 99 });
        handler.DoValidateFieldValues();
    }

    [Fact]
    public void Gives_A_Rule_The_Services_Of_The_Request_The_Handler_And_Its_Connection()
    {
        var handler = NewHandler("taken");
        handler.SetRow(new ValidatedRow { Code = "free" });
        handler.DoValidateFieldValues();
        Assert.Same(handler, ValidateFreeCodeAttribute.Handler);
        Assert.Same(handler.Connection, ValidateFreeCodeAttribute.Connection);

        handler.SetRow(new ValidatedRow { Code = "taken" });
        var error = Assert.Throws<ValidationError>(handler.DoValidateFieldValues);
        Assert.Equal("Code", error.Arguments);
    }
}
