using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    ///   Interface for query object that can automatically include a join by its alias (by looking up in Row definition)
    public interface IDbEnsureJoin
    {
        void EnsureJoin(string alias);
    }
}