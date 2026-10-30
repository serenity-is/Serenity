namespace Serenity.Testing.Playwright;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class FindByClassAttribute(string value) : Attribute
{
    public string Value { get; } = value ?? throw new ArgumentNullException(nameof(value));
}