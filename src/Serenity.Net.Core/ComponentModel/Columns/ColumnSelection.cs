namespace Serenity.Services
{
    /// <summary>
    /// Column selection types for List services
    /// </summary>
    public enum ColumnSelection
    {
        /// <summary>
        /// List, e.g. only the table columns
        /// </summary>
        List = 0,
        /// <summary>
        /// Key Only, e.g. the primary key of the table
        /// </summary>
        KeyOnly = 1,
        /// <summary>
        /// Details, e.g. all the fields
        /// </summary>
        Details = 2,
    }
}