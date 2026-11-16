namespace Serenity.ComponentModel;

/// <summary>
/// How an object is validated by <see cref="ValidationRules.ValidateObject"/>.
/// </summary>
public class ObjectValidationOptions
{
    /// <summary>
    /// Gets or sets the type whose properties declare what is checked, e.g. a form, which is the type of the object when it
    /// is not set. The properties of the object are found by the names of the ones of this type.
    /// </summary>
    public Type? RulesType { get; set; }

    /// <summary>
    /// Gets or sets the services of the context the rules are checked in, e.g. a localizer, see
    /// <see cref="ValidationContextExtensions.GetLocalizer"/>.
    /// </summary>
    public IServiceProvider? Services { get; set; }

    /// <summary>
    /// Gets or sets whether the object is being inserted, for the fields hidden on insert or update, which are not checked,
    /// or null when neither applies.
    /// </summary>
    public bool? IsInsert { get; set; }

    /// <summary>
    /// Gets or sets which properties are checked, e.g. only the ones an update has, by their names. All are checked when it
    /// is not set. The ones that are not checked are still read for the conditions of the others.
    /// </summary>
    public Func<string, bool>? IncludeProperty { get; set; }

    /// <summary>
    /// Gets or sets how the value of a property is read, e.g. from the fields of a row, by its name. The values are read
    /// from the properties of the object when it is not set, and a property the object doesn't have is not checked.
    /// </summary>
    public Func<string, object?>? GetValue { get; set; }

    /// <summary>
    /// Gets or sets whether the validation rules of the properties are checked, which is the default. They are not when
    /// they are checked otherwise, e.g. the ones of a row by its save handler.
    /// </summary>
    public bool CheckRules { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the <see cref="RequiredAttribute"/>s of the properties are checked, which is the default, while
    /// their <see cref="RequiredWhenAttribute"/>s always are. They are not for a row, as a required field of a row is one that
    /// is not null in the database, which is checked otherwise, and its attribute only makes a field required in the forms.
    /// </summary>
    public bool CheckRequiredAttribute { get; set; } = true;

    /// <summary>
    /// Gets or sets the local text key prefix of the properties, which the implied text keys of the messages of their rules
    /// start with, e.g. <c>Forms.Northwind.Customer.</c>, by default the one of the form or columns of the
    /// <see cref="RulesType"/>, if it is one.
    /// </summary>
    public string? TextKeyPrefix { get; set; }
}
