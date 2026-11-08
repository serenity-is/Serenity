using System.Globalization;

namespace Serenity.Data;

/// <summary>
/// Field with a string value that is one of the members of a <see cref="StringEnum"/> type. Like
/// <see cref="EnumField{TEnum}"/>, a value that is not one of the members is rejected while the value is
/// deserialized from JSON, so it never reaches the row.
/// </summary>
/// <remarks>
/// A value stored earlier that is no longer a member, e.g. a retired one, is rejected too when a row that has it
/// is deserialized, as it is for numeric enums. Set <see cref="EnumType"/> to null to skip the validation.
/// </remarks>
/// <typeparam name="TStringEnum">The string enumeration type.</typeparam>
/// <seealso cref="StringField" />
/// <seealso cref="IEnumTypeField" />
public class StringEnumField<TStringEnum> : StringField, IEnumTypeField
    where TStringEnum : StringEnum
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringEnumField{TStringEnum}"/> class.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <param name="name">The name.</param>
    /// <param name="caption">The caption.</param>
    /// <param name="size">The size.</param>
    /// <param name="flags">The flags.</param>
    /// <param name="getValue">The get value.</param>
    /// <param name="setValue">The set value.</param>
    /// <exception cref="InvalidProgramException">
    /// TStringEnum is not a <see cref="StringEnum"/> type.
    /// </exception>
    public StringEnumField(ICollection<Field> collection, string name, LocalText? caption = null, int size = 0,
        FieldFlags flags = FieldFlags.Default, Func<IRow, string?>? getValue = null,
        Action<IRow, string?>? setValue = null)
        : base(collection, name, caption, size, flags, getValue, setValue)
    {
        if (!StringEnum.IsStringEnum(typeof(TStringEnum)))
            throw new InvalidProgramException(typeof(TStringEnum).FullName +
                " is used as type parameter for a StringEnumField but it is not a StringEnum type!");

        EnumType = typeof(TStringEnum);
    }

    /// <summary>
    /// Gets or sets the type of the string enumeration. It is <typeparamref name="TStringEnum"/> by default, and
    /// no validation is performed when it is set to null, like Int32Field.EnumType.
    /// </summary>
    public Type? EnumType { get; set; }

    /// <summary>
    /// Checks that the value read from JSON is one of the members of the string enumeration. An empty value is
    /// converted to null, like an empty string is for an enum field, so it is left to the required field validation.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The value itself, or null if it is empty.</returns>
    /// <exception cref="InvalidCastException">The value is not one of the members.</exception>
    protected override string? ConvertJsonValue(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        var enumType = EnumType;
        if (enumType == null || StringEnum.IsDefined(enumType, value))
            return value;

        throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture,
            "'{0}' is not a valid {1} string enum value!", value, enumType.Name));
    }
}
