using Serenity.Test.Data;
using Serenity.Testing;
using Xunit;

namespace Serenity.Data.Test
{
    public partial class WrappedConnectionTests
    {
        private DisplayOrderRow.RowFields fld = DisplayOrderRow.Fields;

        private DbTestContext NewDbTestContext()
        {
            return new DbTestContext(DbOverride.New<SerenityDbScript>("Serenity", "DBSerenity"));
        }

        [Fact]
        public void SqlConnectionsNewByKeyReturnsWrappedConnection()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                Assert.IsType<WrappedConnection>(connection);
            }
        }

        [Fact]
        public void WrappedTransactionWorksProperlyWithCommit()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                connection.EnsureOpen();

                Assert.IsType<WrappedConnection>(connection);
                Assert.Null(connection.GetCurrentTransaction());

                using (var transaction = connection.BeginTransaction())
                {
                    Assert.IsType<WrappedTransaction>(transaction);

                    var currentTransaction = connection.GetCurrentTransaction();
                    Assert.NotNull(currentTransaction);

                    Assert.Equal(((WrappedTransaction)transaction).Transaction, currentTransaction);
                    Assert.NotEqual(currentTransaction, transaction);

                    transaction.Commit();

                    Assert.Null(connection.GetCurrentTransaction());
                }
            }
        }

        [Fact]
        public void WrappedTransactionWorksProperlyWithRollback()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                connection.EnsureOpen();

                Assert.IsType<WrappedConnection>(connection);
                Assert.Null(connection.GetCurrentTransaction());

                using (var transaction = connection.BeginTransaction())
                {
                    Assert.IsType<WrappedTransaction>(transaction);

                    var currentTransaction = connection.GetCurrentTransaction();
                    Assert.NotNull(currentTransaction);

                    Assert.Equal(((WrappedTransaction)transaction).Transaction, currentTransaction);
                    Assert.NotEqual(currentTransaction, transaction);

                    transaction.Rollback();

                    Assert.Null(connection.GetCurrentTransaction());
                }
            }
        }

        [Fact]
        public void WrappedTransactionWorksProperlyWithDispose()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                connection.EnsureOpen();

                Assert.IsType<WrappedConnection>(connection);
                Assert.Null(connection.GetCurrentTransaction());

                using (var transaction = connection.BeginTransaction())
                {
                    Assert.IsType<WrappedTransaction>(transaction);

                    var currentTransaction = connection.GetCurrentTransaction();
                    Assert.NotNull(currentTransaction);

                    Assert.Equal(((WrappedTransaction)transaction).Transaction, currentTransaction);
                    Assert.NotEqual(currentTransaction, transaction);
                }

                Assert.Null(connection.GetCurrentTransaction());
            }
        }

        [Fact]
        public void NewCommandUsesCorrectTransaction()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                connection.EnsureOpen();

                Assert.IsType<WrappedConnection>(connection);
                Assert.Null(connection.GetCurrentTransaction());

                using (var transaction = connection.BeginTransaction())
                {
                    Assert.IsType<WrappedTransaction>(transaction);

                    var currentTransaction = connection.GetCurrentTransaction();
                    Assert.NotNull(currentTransaction);

                    Assert.Equal(((WrappedTransaction)transaction).Transaction, currentTransaction);
                    Assert.NotEqual(currentTransaction, transaction);

                    using (var command = connection.CreateCommand())
                    {
                        Assert.NotNull(command.Transaction);
                        Assert.Equal(((WrappedConnection)connection).Connection, command.Connection);
                        Assert.Equal(currentTransaction, command.Transaction);
                    }
                }

                Assert.Null(connection.GetCurrentTransaction());

                using (var command = connection.CreateCommand())
                {
                    Assert.Null(command.Transaction);
                    Assert.Equal(((WrappedConnection)connection).Connection, command.Connection);
                }
            }
        }
    }
}