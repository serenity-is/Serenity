using System.Collections;

namespace Serenity.Data
{
    /// <summary>
    /// IRow
    /// </summary>
    /// <seealso cref="Serenity.Data.IEntityWithJoins" />
    public interface IRow : IEntityWithJoins
    {
        /// <summary>
        /// Creates the new.
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
        /// Fields the assigned value.
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
        /// Gets or sets a value indicating whether [track assignments].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [track assignments]; otherwise, <c>false</c>.
        /// </value>
        bool TrackAssignments { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [track with checks].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [track with checks]; otherwise, <c>false</c>.
        /// </value>
        bool TrackWithChecks { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance is any field assigned.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is any field assigned; otherwise, <c>false</c>.
        /// </value>
        bool IsAnyFieldAssigned { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is any field changed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is any field changed; otherwise, <c>false</c>.
        /// </value>
        bool IsAnyFieldChanged { get; }
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
        /// Gets or sets the <see cref="System.Object"/> with the specified field name.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        object this[string fieldName] { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TFields">The type of the fields.</typeparam>
    /// <seealso cref="Serenity.Data.IEntityWithJoins" />
    public interface IRow<TFields> : IRow
    {
        /// <summary>
        /// Creates the new.
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
}