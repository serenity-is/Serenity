using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace Serenity.Services;

public class SaveRequestHandlerTests_AsyncValidation
{
    /// <summary>
    /// A rule whose synchronous and asynchronous checks differ on purpose, so the tests can tell which one was called: only
    /// the asynchronous one rejects "taken".
    /// </summary>
    private class ValidateFreeAsyncAttribute : ValidationRuleAttribute
    {
        public static CancellationToken LastToken;

        public override string Rule => "freeAsync";

        protected override bool IsValidValue(object? value, ValidationContext context) => true;

        protected override async ValueTask<bool> IsValidValueAsync(object? value, ValidationContext context,
            CancellationToken cancellationToken)
        {
            LastToken = cancellationToken;
            await Task.Yield();
            return (string?)value != "taken";
        }
    }

    [ReadPermission(SpecialPermissionKeys.Public)]
    private class AsyncRow : Row<AsyncRow.RowFields>, IIdRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [ValidateFreeAsync]
        public string? Code { get => fields.Code[this]; set => fields.Code[this] = value; }

        [ValidateRange(1, 10)]
        public int? Quantity { get => fields.Quantity[this]; set => fields.Quantity[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public StringField Code = null!;
            public Int32Field Quantity = null!;
        }
    }

    private class AsyncHandler(IRequestContext context) :
        SaveRequestHandlerAsync<AsyncRow, SaveRequest<AsyncRow>, SaveResponse>(context)
    {
        public void SetRow(AsyncRow row) => Row = row;
        public void SetUow(IUnitOfWork uow) => UnitOfWork = uow;
        public Task DoValidateFieldValuesAsync(CancellationToken token = default) => ValidateFieldValuesAsync(token);
    }

    private static AsyncHandler NewHandler()
    {
        var handler = new AsyncHandler(new NullRequestContext().WithPermissions(_ => true));
        handler.SetUow(new MockUnitOfWork(new MockDbConnection()));
        return handler;
    }

    [Fact]
    public async Task Awaits_The_Asynchronous_Check_Of_A_Rule_With_The_Token()
    {
        var handler = NewHandler();
        handler.SetRow(new AsyncRow { Code = "taken" });
        using var cancellation = new CancellationTokenSource();

        var error = await Assert.ThrowsAsync<ValidationError>(() => handler.DoValidateFieldValuesAsync(cancellation.Token));
        Assert.Equal("Code", error.Arguments);
        Assert.Equal(cancellation.Token, ValidateFreeAsyncAttribute.LastToken);

        handler.SetRow(new AsyncRow { Code = "free" });
        await handler.DoValidateFieldValuesAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public void The_Synchronous_Check_Is_What_The_Synchronous_Handlers_Call()
    {
        // ValidateFieldValues of the synchronous handlers checks the rules by ValidationRules.Validate
        var context = new ValidationContext(new AsyncRow()) { MemberName = "Code" };
        Assert.Null(ValidationRules.Validate(new ValidateFreeAsyncAttribute(), "taken", context));
    }

    [Fact]
    public async Task Checks_The_Other_Rules_Synchronously()
    {
        var handler = NewHandler();
        handler.SetRow(new AsyncRow { Quantity = 11 });

        var error = await Assert.ThrowsAsync<ValidationError>(() => handler.DoValidateFieldValuesAsync(TestContext.Current.CancellationToken));
        Assert.Equal("Quantity", error.Arguments);
        Assert.Equal("Please enter a value between 1 and 10.", error.Message);
    }

    [Fact]
    public async Task ValidateAsync_Skips_A_Rule_Only_Checked_By_The_Forms_And_Skips_Empty_Values()
    {
        var context = new ValidationContext(new object()) { MemberName = "Code" };
        Assert.Null(await ValidationRules.ValidateAsync(new ValidateFreeAsyncAttribute { Side = ValidationSide.Client }, "taken", context, TestContext.Current.CancellationToken));
        Assert.Null(await ValidationRules.ValidateAsync(new ValidateFreeAsyncAttribute(), null, context, TestContext.Current.CancellationToken));
        Assert.Equal("Please enter a valid value.", await ValidationRules.ValidateAsync(new ValidateFreeAsyncAttribute(), "taken", context, TestContext.Current.CancellationToken));
    }
}
