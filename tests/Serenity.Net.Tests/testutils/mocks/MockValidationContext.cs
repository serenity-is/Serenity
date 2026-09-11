namespace Serenity.TestUtils;

public class MockValidationContext(object? value = null, ITextLocalizer? localizer = null) : IValidationContext
{
    public object? Value { get; set; } = value;
    public ITextLocalizer Localizer { get; } = localizer ?? NullTextLocalizer.Instance;
    public IDbConnection? Connection => null;

    public object? GetFieldValue(string fieldName) => null;
}
