using System.Collections;

namespace Serenity.Data;

/// <summary>
/// Base class for Serenity entities
/// </summary>
/// <typeparam name="TFields">The type of the fields.</typeparam>
/// <seealso cref="IRow" />
/// <seealso cref="IRow{TFields}" />
/// <seealso cref="INotifyPropertyChanged" />
/// <seealso cref="IEditableObject" />
[JsonConverter(typeof(JsonRowConverter))]
public abstract partial class Row<TFields> : IRow, IRow<TFields>
    where TFields : RowFieldsBase
{
    /// <summary>
    /// The fields
    /// </summary>
    protected readonly TFields fields;
    internal bool[] assignedFields;
    internal Hashtable dictionaryData;
    internal object[] indexedData;
    internal bool tracking;
    internal bool trackWithChecks;

    /// <summary>
    /// Initializes a new instance of the <see cref="Row{TFields}"/> class.
    /// </summary>
    protected Row()
        : this((TFields)RowFieldsProvider.Current.Resolve(typeof(TFields)))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Row{TFields}"/> class.
    /// </summary>
    /// <param name="fields">The fields.</param>
    /// <exception cref="ArgumentNullException">fields</exception>
    /// <exception cref="ArgumentOutOfRangeException">fields</exception>
    protected Row(TFields fields)
    {
        if (fields == null)
            throw new ArgumentNullException(nameof(fields));

        if (!fields.isInitialized)
            throw new ArgumentOutOfRangeException("fields", $"{GetType().FullName} constructor is called " +
                $"with a fields object that is not initialized. Please call .Init() method on it before using!");

        this.fields = fields;

        fields.RowCreated(this);

        ((IRow)this).TrackAssignments = true;
    }

    TFields IRow<TFields>.Fields => fields;
    RowFieldsBase IRow.Fields => fields;
    /// <summary>
    /// Gets the fields.
    /// </summary>
    /// <returns></returns>
    public TFields GetFields() => fields;
    /// <summary>
    /// Gets the fields.
    /// </summary>
    /// <value>
    /// The fields.
    /// </value>
    public static TFields Fields => (TFields)RowFieldsProvider.Current.Resolve(typeof(TFields));

    /// <summary>
    /// Clones the row into target.
    /// </summary>
    /// <param name="clone">The clone.</param>
    /// <param name="cloneHandlers">if set to <c>true</c> [clone handlers].</param>
    protected virtual void CloneInto(Row<TFields> clone,
        bool cloneHandlers)
    {
        ((IRow)clone).IgnoreConstraints = ((IRow)this).IgnoreConstraints;

        foreach (var field in fields)
            field.Copy(this, clone);

        clone.tracking = tracking;
        if (tracking && assignedFields != null)
        {
            clone.assignedFields = new bool[assignedFields.Length];
            Array.Copy(assignedFields, clone.assignedFields, assignedFields.Length);
        }
        else
            clone.assignedFields = null;

        clone.trackWithChecks = trackWithChecks;

        clone.originalValues = originalValues;

        if (dictionaryData != null)
            clone.dictionaryData = (Hashtable)dictionaryData.Clone();
        else
            clone.dictionaryData = null;

        if (indexedData != null)
        {
            clone.indexedData = new object[indexedData.Length];
            for (var i = 0; i < indexedData.Length; i++)
                clone.indexedData[i] = indexedData[i];
        }
        else
            clone.indexedData = null;

        if (previousValues != null)
            clone.previousValues = previousValues.CloneRow();
        else
            clone.previousValues = null;

        if (cloneHandlers)
        {
            clone.postHandler = postHandler;
            clone.propertyChanged = propertyChanged;

            if (validationErrors != null)
                clone.validationErrors = new Dictionary<string, string>(validationErrors);
            else
                clone.validationErrors = null;
        }
    }

    /// <summary>
    /// Clones the row.
    /// </summary>
    /// <returns></returns>
    protected virtual Row<TFields> CloneRow()
    {
        var clone = CreateNew();
        CloneInto(clone, true);
        return clone;
    }

    /// <summary>
    /// Creates the new.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected virtual Row<TFields> CreateNew()
    {
        if (fields.rowFactory == null)
            throw new NotImplementedException();

        return (Row<TFields>)fields.rowFactory();
    }

    IRow<TFields> IRow<TFields>.CreateNew()
    {
        return CreateNew();
    }

    IRow<TFields> IRow<TFields>.CloneRow()
    {
        return CloneRow();
    }

    IRow IRow.CloneRow()
    {
        return CloneRow();
    }

    IRow IRow.CreateNew()
    {
        return CreateNew();
    }

    void IRow.FieldAssignedValue(Field field)
    {
        if (!tracking)
            return;

        assignedFields ??= new bool[fields.Count];

        assignedFields[field.index] = true;

        if (validationErrors != null)
            ((IEditableRow)this).RemoveValidationError(field.PropertyName ?? field.Name);

        if (propertyChanged != null &&
            previousValues != null &&
            field.IndexCompare(previousValues, this) != 0) { 
            RaisePropertyChanged(field);
            field.Copy(this, previousValues);
        }
    }

    /// <summary>
    /// Finds the field.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns></returns>
    public Field FindField(string fieldName)
    {
        return fields.FindField(fieldName);
    }

    /// <summary>
    /// Finds the name of the field by property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    public Field FindFieldByPropertyName(string propertyName)
    {
        return fields.FindFieldByPropertyName(propertyName);
    }

    /// <summary>
    /// Gets a value indicating whether this instance is any field assigned.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is any field assigned; otherwise, <c>false</c>.
    /// </value>
    bool IRow.IsAnyFieldAssigned => tracking && assignedFields != null;

    bool IRow.IgnoreConstraints { get; set; }

    /// <summary>
    /// Table name
    /// </summary>
    string IEntity.Table => fields.TableName;

    /// <summary>
    /// Gets or sets a value indicating whether [track assignments].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [track assignments]; otherwise, <c>false</c>.
    /// </value>
    bool IRow.TrackAssignments
    {
        get
        {
            return tracking;
        }
        set
        {
            if (tracking != value)
            {
                if (value)
                {
                    if (propertyChanged != null)
                        previousValues = CloneRow();

                    tracking = value;
                }
                else
                {
                    tracking = false;
                    trackWithChecks = false;
                    assignedFields = null;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [track with checks].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [track with checks]; otherwise, <c>false</c>.
    /// </value>
    bool IRow.TrackWithChecks
    {
        get
        {
            return tracking && trackWithChecks;
        }
        set
        {
            if (value != (tracking && trackWithChecks))
            {
                if (value && !tracking)
                    ((IRow)this).TrackAssignments = true;

                trackWithChecks = value;
            }
        }
    }

    private Field FindFieldEnsure(string fieldName)
    {
        var field = FindField(fieldName);
        if (field is null)
            throw new ArgumentOutOfRangeException("fieldName", string.Format(
                "{0} has no field with name '{1}'.", GetType().Name, fieldName));
        return field;
    }

    /// <summary>
    /// Gets or sets the <see cref="object"/> with the specified field name.
    /// </summary>
    /// <value>
    /// The <see cref="object"/>.
    /// </value>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns></returns>
    object IRow.this[string fieldName]
    {
        get
        {
            var field = FindFieldByPropertyName(fieldName) ??
                FindField(fieldName);

            if (field is null)
            {
                if (dictionaryData != null)
                    return dictionaryData[fieldName];

                return null;
            }

            return field.AsObject(this);
        }
        set
        {
            (FindFieldByPropertyName(fieldName) ??
                FindFieldEnsure(fieldName)).AsObject(this, value);
        }
    }

    /// <summary>
    /// Sets the dictionary data.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    void IRow.SetDictionaryData(object key, object value)
    {
        if (value == null)
        {
            if (dictionaryData == null)
                return;
            dictionaryData[key] = null;
        }
        else
        {
            dictionaryData ??= new Hashtable();
            dictionaryData[key] = value;
        }
    }

    /// <summary>
    /// Gets the dictionary data keys.
    /// </summary>
    /// <returns></returns>
    IEnumerable IRow.GetDictionaryDataKeys()
    {
        if (dictionaryData == null)
            return Array.Empty<object>();

        return dictionaryData.Keys;
    }

    /// <summary>
    /// Gets the dictionary data.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    object IRow.GetDictionaryData(object key)
    {
        if (dictionaryData != null)
            return dictionaryData[key];

        return null;
    }

    object IRow.GetIndexedData(int index)
    {
        if (indexedData != null)
            return indexedData[index];

        return null;
    }

    void IRow.SetIndexedData(int index, object value)
    {
        if (value == null)
        {
            if (indexedData == null)
                return;

            indexedData[index] = null;
        }
        else
        {
            indexedData ??= new object[fields.Count];

            indexedData[index] = value;
        }
    }

    /// <summary>
    /// Determines whether the specified field is assigned.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>
    ///   <c>true</c> if the specified field is assigned; otherwise, <c>false</c>.
    /// </returns>
    public bool IsAssigned(Field field)
    {
        if (assignedFields == null)
            return false;

        return assignedFields[field.index];
    }

    /// <summary>
    /// Clears the assignment flag of the field.
    /// </summary>
    /// <param name="field">The field.</param>
    public void ClearAssignment(Field field)
    {
        if (assignedFields == null)
            return;

        assignedFields[field.index] = false;

        for (var i = 0; i < assignedFields.Length; i++)
            if (assignedFields[i])
                return;

        assignedFields = null;
    }

    IDictionary<string, Join> IHaveJoins.Joins => fields.Joins;

    /// <summary>
    /// Gets the identifier field.
    /// </summary>
    /// <value>
    /// The identifier field.
    /// </value>
    Field IRow.IdField => fields.IdField;

    /// <summary>
    /// Gets the name field.
    /// </summary>
    /// <value>
    /// The name field.
    /// </value>
    Field IRow.NameField => fields.NameField;
}