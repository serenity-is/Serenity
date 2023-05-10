namespace Serenity.Data;

/// <summary>
/// Field with an Enum value
/// </summary>
/// <typeparam name="TEnum">The type of the enum.</typeparam>
public class EnumField<TEnum> : Int32Field
    where TEnum : struct, IComparable, IFormattable, IConvertible
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumField{TEnum}"/> class.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <param name="name">The name.</param>
    /// <param name="caption">The caption.</param>
    /// <param name="size">The size.</param>
    /// <param name="flags">The flags.</param>
    /// <param name="getValue">The get value.</param>
    /// <param name="setValue">The set value.</param>
    /// <exception cref="InvalidProgramException">
    /// </exception>
    public EnumField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
        Func<IRow, int?> getValue = null, Action<IRow, int?> setValue = null)
        : base(collection, name, caption, size, flags, getValue, setValue)
    {
        if (!typeof(TEnum).IsEnum)
            throw new InvalidProgramException(typeof(TEnum).FullName + " is used as type parameter for an EnumField but it is not an enum type!");

        if (Enum.GetUnderlyingType(typeof(TEnum)) != typeof(int))
            throw new InvalidProgramException(typeof(TEnum).FullName + " is used as type parameter for an EnumField but it is not based on Int32!");

        EnumType = typeof(TEnum);
    }

    /// <summary>
    /// Gets or sets the value of this field with the specified row.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <returns></returns>
    public new TEnum? this[IRow row]
    {
        get
        {
            CheckUnassignedRead(row);
            var value = _getValue(row);
            if (value == null)
                return null;

            return (TEnum)(object)value;
        }
        set
        {
            int? v = value == null ? (int?)null : Convert.ToInt32(value);
            _setValue(row, v);
            row.FieldAssignedValue(this);
        }
    }
}
