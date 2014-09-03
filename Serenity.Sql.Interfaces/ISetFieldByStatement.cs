namespace Serenity.Data
{
    /// <summary>
    ///   Interface for objects setting values by SetTo method (like SqlInsert, SqlUpdate...)</summary>
    /// <typeparam name="T">
    ///   Type of the parameterized object itself.</typeparam>
    public interface ISetFieldByStatement : IQueryWithParams
    {
        /// <summary>
        ///   Sets a field to given value identifier.</summary>
        /// <param name="name">
        ///   Field name (required).</param>
        /// <param name="statement">
        ///   Value identifier (e.g. param name).</param>
        /// <returns>
        ///   Query itself.</returns>
        void SetTo(string name, string statement);
    }
}