namespace Serenity.Data
{
    /// <summary>
    ///   Interface for objects setting parameters by PARAM method (like SqlInsert, SqlUpdate, SqlDelete...)</summary>
    /// <typeparam name="T">
    ///   Type of the parameterized object itself.</typeparam>
    public interface IDbParameterized
    {
        /// <summary>
        ///   Sets a param with specified value.</summary>
        /// <param name="name">
        ///   Parameter name (required).</param>
        /// <param name="value">
        ///   Value.</param>
        /// <returns>
        ///   Query itself.</returns>
        void SetParam(string name, object value);

        /// <summary>
        ///   Gets next available parameter.</summary>
        Parameter AutoParam();
    }
}