using System.Data;

namespace Serenity.Data
{
    /// <summary>
    ///   Interface for database-bound caching objects</summary>
    public interface IDbCache
    {
        /// <summary>
        ///   Refreshes cache using specified connection.</summary>
        /// <param name="connection">
        ///   Connection to be used (required).</param>
        void Refresh();
    }
}