namespace Serenity.Data;

/// <summary>
/// Methods and properties of a row to support desktop app grid etc. editing
/// </summary>
public interface IEditableRow : IRow, IEditableObject, INotifyPropertyChanged
{
    /// <summary>
    /// Adds the validation error.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="error">The error.</param>
    void AddValidationError(string propertyName, string error);

    /// <summary>
    /// Clears the validation errors.
    /// </summary>
    void ClearValidationErrors();

    /// <summary>
    /// Gets a value indicating whether this instance has errors.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance has errors; otherwise, <c>false</c>.
    /// </value>
    bool HasErrors { get; }

    /// <summary>
    /// Gets a value indicating whether this instance has post handler.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance has post handler; otherwise, <c>false</c>.
    /// </value>
    bool HasPostHandler { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is any field changed.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is any field changed; otherwise, <c>false</c>.
    /// </value>
    bool IsAnyFieldChanged { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is editing.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is editing; otherwise, <c>false</c>.
    /// </value>
    bool IsEditing { get; }

    /// <summary>
    /// Determines whether [is field changed] [the specified field].
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>
    ///   <c>true</c> if [is field changed] [the specified field]; otherwise, <c>false</c>.
    /// </returns>
    bool IsFieldChanged(Field field);

    /// <summary>
    /// The post ended event
    /// </summary>
    event EventHandler PostEnded;

    /// <summary>
    /// Gets the original values.
    /// </summary>
    /// <value>
    /// The original values.
    /// </value>
    public IRow OriginalValues { get; }

    /// <summary>
    /// Gets the previous values.
    /// </summary>
    /// <value>
    /// The previous values.
    /// </value>
    public IRow PreviousValues { get; }

    /// <summary>
    /// Removes the validation error.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    void RemoveValidationError(string propertyName);

    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    /// <value>
    /// The validation errors.
    /// </value>        
    IDictionary<string, string> ValidationErrors { get; }

    /// <summary>
    /// Gets or sets the post handler.
    /// </summary>
    /// <value>
    /// The post handler.
    /// </value>
    Action<IRow> PostHandler { get; set; }
}