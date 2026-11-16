using System.ComponentModel.DataAnnotations;

namespace Serenity.ComponentModel;

/// <summary>
/// What is checked for a property of an object, see <see cref="ObjectValidator"/>.
/// </summary>
internal sealed class ObjectValidationCheck(string name, object? value, bool required, List<ValidationAttribute> rules,
    ValidationContext context)
{
    /// <summary>Gets the name of the property.</summary>
    public string Name { get; } = name;
    /// <summary>Gets the value of the property.</summary>
    public object? Value { get; } = value;
    /// <summary>Gets whether a value is required.</summary>
    public bool Required { get; } = required;
    /// <summary>Gets the rules of the property.</summary>
    public List<ValidationAttribute> Rules { get; } = rules;
    /// <summary>Gets the context its rules are checked in.</summary>
    public ValidationContext Context { get; } = context;
}
