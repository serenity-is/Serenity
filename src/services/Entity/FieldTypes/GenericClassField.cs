using System.Collections;

namespace Serenity.Data;

/// <summary>
/// Base class for fields with reference type values.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <seealso cref="Field" />
public abstract class GenericClassField<TValue> : Field where TValue : class
{
    /// <summary>
    /// The get value callback.
    /// </summary>
    protected internal Func<IRow, TValue?> _getValue;
    /// <summary>
    /// The set value callback.
    /// </summary>
    protected internal Action<IRow, TValue?> _setValue;

    internal GenericClassField(ICollection<Field> collection, FieldType type, string name, LocalText? caption, int size, FieldFlags flags,
        Func<IRow, TValue?>? getValue = null, Action<IRow, TValue?>? setValue = null)
        : base(collection, type, name, caption, size, flags)
    {
        _getValue = getValue ?? (r => (TValue?)r.GetIndexedData(index));
        _setValue = setValue ?? ((r, v) => r.SetIndexedData(index, v));
    }

    /// <summary>
    /// Copies the specified source.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="target">The target.</param>
    public override void Copy(IRow source, IRow target)
    {
        _setValue(target, _getValue(source));
        target.OnFieldSet(this);
    }

    /// <summary>
    /// Gets or sets the value of this field with the specified row.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <returns>The value of the field in the row.</returns>
    public TValue? this[IRow row]
    {
        get
        {
            row.OnFieldGet(this);
            return _getValue(row);
        }
        set
        {
            _setValue(row, value);
            row.OnFieldSet(this);
        }
    }

    /// <summary>
    /// Converts the value.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="provider">The provider.</param>
    /// <returns>The converted value.</returns>
    public override object? ConvertValue(object? source, IFormatProvider provider)
    {
        if (source is Newtonsoft.Json.Linq.JValue jValue)
            source = jValue.Value;

        if (source == null)
            return null;
        else
        {
            if (source is TValue value)
                return value;

            return Convert.ChangeType(source, typeof(TValue), provider);
        }
    }

    /// <summary>
    /// Compares two values of this field using the specified comparer.
    /// </summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <param name="comparer">The comparer, or null to use the default comparer for the value type.</param>
    /// <returns>A value indicating the relative order of the two values.</returns>
    protected virtual int CompareValues(TValue value1, TValue value2, IComparer? comparer)
    {
        if (comparer is IComparer<TValue> typed)
            return typed.Compare(value1, value2);

        return Comparer<TValue>.Default.Compare(value1, value2);
    }

    /// <summary>
    /// Compares the field values for two rows for an ascending index sort using the specified
    /// comparer, falling back to <see cref="Field.Comparer"/> when it is null.
    /// </summary>
    /// <param name="row1">The row1.</param>
    /// <param name="row2">The row2.</param>
    /// <param name="comparer">The comparer, or null to use the field comparer.</param>
    /// <returns>A value indicating the relative order of the two rows.</returns>
    public override int IndexCompare(IRow row1, IRow row2, IComparer? comparer = null)
    {
        var value1 = _getValue(row1);
        var value2 = _getValue(row2);

        if (value1 is null)
            return value2 is null ? 0 : -1;

        if (value2 is null)
            return 1;

        return CompareValues(value1, value2, comparer ?? Comparer);
    }

    /// <inheritdoc />
    public override object? AsObjectNoCheck(IRow row)
    {
        return _getValue(row);
    }

    /// <summary>
    /// Sets the value of this field in specified row as object.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="value">The value.</param>
    public override void AsObject(IRow row, object? value)
    {
        if (value == null)
            _setValue(row, null);
        else if (value is TValue typedValue)
            _setValue(row, typedValue);
        else
            throw new InvalidCastException(
                $"Invalid cast exception while trying to set the value of {Name} field on {row.GetType().Name} as object. " +
                $"Source type is {value.GetType().Name}. Use ConvertValue to convert the value to {typeof(TValue).Name} first.");

        row.OnFieldSet(this);
    }

    /// <inheritdoc />
    public override bool IsNullNoCheck(IRow row)
    {
        return _getValue(row) == null;
    }

    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    /// <value>
    /// The type of the value.
    /// </value>
    public override Type ValueType => typeof(TValue);
}