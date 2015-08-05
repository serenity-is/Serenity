using Serenity.Test.Data;
using Serenity.Testing;
using Xunit;

namespace Serenity.Data.Test
{
    [Collection("AvoidParallel")]
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
                Assert.Null(connection.GetCurrentActualTransaction());

                using (var transaction = connection.BeginTransaction())
                {
                    Assert.IsType<WrappedTransaction>(transaction);

                    var actualTransaction = connection.GetCurrentActualTransaction();
                    Assert.NotNull(actualTransaction);

                    Assert.Equal(((WrappedTransaction)transaction).ActualTransaction, actualTransaction);
                    Assert.NotEqual(actualTransaction, transaction);

                    transaction.Commit();

                    Assert.Null(connection.GetCurrentActualTransaction());
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
                Assert.Null(connection.GetCurrentActualTransaction());

                using (var transaction = connection.BeginTransaction())
                {
                    Assert.IsType<WrappedTransaction>(transaction);

                    var actualTransaction = connection.GetCurrentActualTransaction();
                    Assert.NotNull(actualTransaction);

                    Assert.Equal(((WrappedTransaction)transaction).ActualTransaction, actualTransaction);
                    Assert.NotEqual(actualTransaction, transaction);

                    transaction.Rollback();

                    Assert.Null(connection.GetCurrentActualTransaction());
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
                Assert.Null(connection.GetCurrentActualTransaction());

                using (var transaction = connection.BeginTransaction())
                {
                    Assert.IsType<WrappedTransaction>(transaction);

                    var actualTransaction = connection.GetCurrentActualTransaction();
                    Assert.NotNull(actualTransaction);

                    Assert.Equal(((WrappedTransaction)transaction).ActualTransaction, actualTransaction);
                    Assert.NotEqual(actualTransaction, transaction);
                }

                Assert.Null(connection.GetCurrentActualTransaction());
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
                Assert.Null(connection.GetCurrentActualTransaction());

                using (var transaction = connection.BeginTransaction())
                {
                    Assert.IsType<WrappedTransaction>(transaction);

                    var actualTransaction = connection.GetCurrentActualTransaction();
                    Assert.NotNull(actualTransaction);

                    Assert.Equal(((WrappedTransaction)transaction).ActualTransaction, actualTransaction);
                    Assert.NotEqual(actualTransaction, transaction);

                    using (var command = connection.CreateCommand())
                    {
                        Assert.NotNull(command.Transaction);
                        Assert.Equal(((WrappedConnection)connection).ActualConnection, command.Connection);
                        Assert.Equal(actualTransaction, command.Transaction);
                    }
                }

                Assert.Null(connection.GetCurrentActualTransaction());

                using (var command = connection.CreateCommand())
                {
                    Assert.Null(command.Transaction);
                    Assert.Equal(((WrappedConnection)connection).ActualConnection, command.Connection);
                }
            }
        }
    }
}