namespace Serenity.Data
{
    /// <summary>
    /// IUpdateLogRow
    /// </summary>
    public interface IUpdateLogRow
    {
        /// <summary>
        /// Gets the update user identifier field.
        /// </summary>
        /// <value>
        /// The update user identifier field.
        /// </value>
        Field UpdateUserIdField { get; }
        /// <summary>
        /// Gets the update date field.
        /// </summary>
        /// <value>
        /// The update date field.
        /// </value>
        DateTimeField UpdateDateField { get; }
    }

    /// <summary>
    /// IInsertLogRow
    /// </summary>
    public interface IInsertLogRow
    {
        /// <summary>
        /// Gets the insert user identifier field.
        /// </summary>
        /// <value>
        /// The insert user identifier field.
        /// </value>
        Field InsertUserIdField { get; }
        /// <summary>
        /// Gets the insert date field.
        /// </summary>
        /// <value>
        /// The insert date field.
        /// </value>
        DateTimeField InsertDateField { get; }
    }

    /// <summary>
    /// IDeleteLogRow
    /// </summary>
    public interface IDeleteLogRow
    {
        /// <summary>
        /// Gets the delete user identifier field.
        /// </summary>
        /// <value>
        /// The delete user identifier field.
        /// </value>
        Field DeleteUserIdField { get; }
        /// <summary>
        /// Gets the delete date field.
        /// </summary>
        /// <value>
        /// The delete date field.
        /// </value>
        DateTimeField DeleteDateField { get; }
    }

    /// <summary>
    /// ILoggingRow
    /// </summary>
    /// <seealso cref="Serenity.Data.IUpdateLogRow" />
    /// <seealso cref="Serenity.Data.IInsertLogRow" />
    public interface ILoggingRow : IUpdateLogRow, IInsertLogRow
    {
    }
}