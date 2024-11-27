using System.Collections;

namespace Serenity.Data;

/// <summary>
/// Row interface
/// </summary>
/// <seealso cref="IEntityWithJoins" />
public interface IRow : IEntityWithJoins
{
    /// <summary>
    /// Creates a new instance of the row type.
    /// </summary>
    /// <returns></returns>
    IRow CreateNew();
    /// <summary>
    /// Clones the row.
    /// </summary>
    /// <returns></returns>
    IRow CloneRow();
    /// <summary>
    /// Gets the fields.
    /// </summary>
    /// <value>
    /// The fields.
    /// </value>
    RowFieldsBase Fields { get; }
    /// <summary>
    /// Called when field is assigned a value.
    /// </summary>
    /// <param name="field">The field.</param>
    void FieldAssignedValue(Field field);
    /// <summary>
    /// Gets the dictionary data.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    object GetDictionaryData(object key);
    /// <summary>
    /// Gets the dictionary data keys.
    /// </summary>
    /// <returns></returns>
    IEnumerable GetDictionaryDataKeys();
    /// <summary>
    /// Sets the dictionary data.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    void SetDictionaryData(object key, object value);
    /// <summary>
    /// Gets the indexed data.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns></returns>
    object GetIndexedData(int index);
    /// <summary>
    /// Sets the indexed data.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="value">The value.</param>
    void SetIndexedData(int index, object value);
    /// <summary>
    /// Gets or sets a value indicating whether to track assignments to field values.
    /// </summary>
    /// <value>
    ///   <c>true</c> if assignments are tracked; otherwise, <c>false</c>.
    /// </value>
    bool TrackAssignments { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether track assignments to field values,
    /// and raise an exception if an unassigned field is tried to read.
    /// </summary>
    /// <value>
    ///   <c>true</c> if track with checks; otherwise, <c>false</c>.
    /// </value>
    bool TrackWithChecks { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [ignore constraints].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [ignore constraints]; otherwise, <c>false</c>.
    /// </value>
    bool IgnoreConstraints { get; set; }
    /// <summary>
    /// Gets a value indicating whether this row instance has any field assigned.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance has any field assigned; otherwise, <c>false</c>.
    /// </value>
    bool IsAnyFieldAssigned { get; }
    /// <summary>
    /// Determines whether the specified field is assigned.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>
    ///   <c>true</c> if the specified field is assigned; otherwise, <c>false</c>.
    /// </returns>
    bool IsAssigned(Field field);
    /// <summary>
    /// Clears the assignment.
    /// </summary>
    /// <param name="field">The field.</param>
    void ClearAssignment(Field field);
    /// <summary>
    /// Gets the identifier field.
    /// </summary>
    /// <value>
    /// The identifier field.
    /// </value>
    Field IdField { get; }
    /// <summary>
    /// Gets the name field.
    /// </summary>
    /// <value>
    /// The name field.
    /// </value>
    Field NameField { get; }
    /// <summary>
    /// Gets or sets a field value with specified name
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    object this[string fieldName] { get; set; }
}

/// <summary>
/// Base interface for Rows with a known Fields type
/// </summary>
/// <typeparam name="TFields">The type of the fields.</typeparam>
/// <seealso cref="IEntityWithJoins" />
public interface IRow<TFields> : IRow
{
    /// <summary>
    /// Creates a new instance of the row.
    /// </summary>
    /// <returns></returns>
    public new IRow<TFields> CreateNew();
    /// <summary>
    /// Clones the row.
    /// </summary>
    /// <returns></returns>
    public new IRow<TFields> CloneRow();
    /// <summary>
    /// Gets the fields.
    /// </summary>
    /// <value>
    /// The fields.
    /// </value>
    public new TFields Fields { get; }
}