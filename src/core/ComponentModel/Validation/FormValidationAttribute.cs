namespace Serenity.ComponentModel;

/// <summary>
/// Makes the save handlers also validate by a form what they save, i.e. what the form checks in the browser that the row
/// doesn't: its own <see cref="RequiredAttribute"/>s, <see cref="RequiredWhenAttribute"/>s and validation rules, like
/// <see cref="ValidateRangeAttribute"/>, skipping the fields the form hides or makes read only, as the browser does. It can be
/// on a save handler, or on a row for the handlers that have none, and a handler can choose a form by its own logic instead,
/// e.g. by overriding its <c>GetValidationForm</c>, when an entity has more than one.
/// </summary>
/// <param name="formType">The type of the form, e.g. <c>typeof(CustomerForm)</c>.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class FormValidationAttribute(Type formType) : Attribute
{
    /// <summary>
    /// Gets the type of the form.
    /// </summary>
    public Type FormType { get; } = formType ?? throw new ArgumentNullException(nameof(formType));
}
