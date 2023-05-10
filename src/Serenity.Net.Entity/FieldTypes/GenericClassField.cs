using Newtonsoft.Json.Linq;

namespace Serenity.Data;

/// <summary>
/// Base class for fields with reference type values
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <seealso cref="Field" />
public abstract class GenericClassField<TValue> : Field where TValue : class
{
    /// <summary>
    /// The get value callback
    /// </summary>
    protected internal Func<IRow, TValue> _getValue;
    /// <summary>
    /// The set value callback
    /// </summary>
    protected internal Action<IRow, TValue> _setValue;

    internal GenericClassField(ICollection<Field> collection, FieldType type, string name, LocalText caption, int size, FieldFlags flags,
        Func<IRow, TValue> getValue = null, Action<IRow, TValue> setValue = null)
        : base(collection, type, name, caption, size, flags)
    {
        _getValue = getValue ?? (r => (TValue)(r.GetIndexedData(index)));
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
        target.FieldAssignedValue(this);
    }

    /// <summary>
    /// Gets or sets the value of this field with the specified row.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <returns></returns>
    public TValue this[IRow row]
    {
        get
        {
            CheckUnassignedRead(row);
            return _getValue(row);
        }
        set
        {
            _setValue(row, value);
            row.FieldAssignedValue(this);
        }
    }

    /// <summary>
    /// Converts the value.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="provider">The provider.</param>
    /// <returns></returns>
    public override object ConvertValue(object source, IFormatProvider provider)
    {
        if (source is JValue jValue)
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
    /// Gets the value of this field in specified row as object.
    /// </summary>
    /// <param name="row">The row.</param>
    public override object AsObject(IRow row)
    {
        CheckUnassignedRead(row);
        return _getValue(row);
    }

    /// <summary>
    /// Sets the value of this field in specified row as object.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="value">The value.</param>
    public override void AsObject(IRow row, object value)
    {
        _setValue(row, (TValue)value);
        row.FieldAssignedValue(this);
    }

    /// <summary>
    /// Gets if the field value is null.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <returns></returns>
    protected override bool GetIsNull(IRow row)
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