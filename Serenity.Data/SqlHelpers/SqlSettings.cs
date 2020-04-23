
namespace Serenity.Data
{
    /// <summary>
    /// Global SQL settings
    /// </summary>
    public static class SqlSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to automatically quote identifiers.
        /// </summary>
        /// <value>
        ///   <c>true</c> if should automatically quote identifiers; otherwise, <c>false</c>.
        /// </value>
        public static bool AutoQuotedIdentifiers { get; set; }

        /// <summary>
        /// Gets or sets the default command timeout.
        /// </summary>
        /// <value>
        /// The default command timeout.
        /// </value>
        public static int? DefaultCommandTimeout { get; set; }

        /// <summary>
        /// The default dialect
        /// </summary>
        public static ISqlDialect DefaultDialect = new SqlServer2012Dialect();
    }
}