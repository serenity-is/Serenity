namespace Serenity.Data
{
    /// <summary>
    /// Connection string setting
    /// </summary>
    public class ConnectionStringEntry
    {
        /// <summary>
        /// Gets / sets connection string
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Gets / sets provider name
        /// </summary>
        public string ProviderName { get; set; }
        /// <summary>
        /// Gets / sets dialect
        /// </summary>
        public string Dialect { get; set; }
    }
}