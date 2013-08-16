using System.Data;

namespace Serenity.Data
{
    /// <summary>
    ///   Helper class returned from DataHelper.BeginTransactionIf method.</summary>
    public class DummyTransaction : IDbTransaction
    {
        /// <summary>
        ///   Creates a new DummyTransaction instance.</summary>
        public DummyTransaction()
        {
        }

        /// <summary>
        ///   Returns null.</summary>
        public IDbConnection Connection
        {
            get { return null; }
        }

        /// <summary>
        ///   Returns IsolationLevel.Unspecified</summary>
        public IsolationLevel IsolationLevel
        {
            get { return IsolationLevel.Unspecified; }
        }

        /// <summary>
        ///   Does nothing.</summary>
        public void Commit()
        {
        }

        /// <summary>
        ///   Does nothing.</summary>
        public void Rollback()
        {
        }

        /// <summary>
        ///   Does nothing.</summary>
        public void Dispose()
        {
        }
    }
}
