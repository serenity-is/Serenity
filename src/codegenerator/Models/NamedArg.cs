namespace Serenity.CodeGenerator;

public class NamedArg(string propertyName, object value)
{
    public string PropertyName { get; } = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
    public object Value { get; } = value;
}