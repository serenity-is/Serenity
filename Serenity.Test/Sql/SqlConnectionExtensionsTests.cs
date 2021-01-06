using Serenity.Services;
using Serenity.Test.Data;
using Serenity.Testing;
using System;
using Xunit;

namespace Serenity.Data.Test
{
    [Collection("AvoidParallel")]
    public partial class SqlConnectionExtensionsTests
    {
        private DisplayOrderRow.RowFields fld = DisplayOrderRow.Fields;

        private DbTestContext NewDbTestContext()
        {
            return new DbTestContext(DbOverride.New<SerenityDbScript>("Serenity", "DBSerenity"));
        }

        [Fact]
        public void ByIdThrowsExceptionIfNotFound()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 1)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                Assert.NotNull(connection.ById<DisplayOrderRow>(id));

                var error = Assert.Throws<ValidationError>(() =>
                {
                    connection.ById<DisplayOrderRow>(id + 1);
                });

                Assert.Equal("RecordNotFound", error.ErrorCode);
            }
        }

        [Fact]
        public void ByIdThrowsExceptionIfMoreThanOneResultReturns()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                var fld = FakeDisplayOrderRow.Fields;

                new SqlDelete(FakeDisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 6)
                    .ExecuteAndGetID(connection);

                var error = Assert.Throws<InvalidOperationException>(() =>
                {
                    connection.ById<FakeDisplayOrderRow>(777);
                });
            }
        }

        [Fact]
        public void ByIdWithEditQueryThrowsExceptionIfNotFound()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 1)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                Assert.NotNull(connection.ById<DisplayOrderRow>(id, q => q.Select(fld.ID)));

                var error = Assert.Throws<ValidationError>(() =>
                {
                    connection.ById<DisplayOrderRow>(id + 1, q => q.Select(fld.ID));
                });

                Assert.Equal("RecordNotFound", error.ErrorCode);
            }
        }

        [Fact]
        public void ByIdWithEditQueryThrowsExceptionIfMoreThanOneResultReturns()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                var fld = FakeDisplayOrderRow.Fields;

                new SqlDelete(FakeDisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 6)
                    .ExecuteAndGetID(connection);

                var error = Assert.Throws<InvalidOperationException>(() =>
                {
                    connection.ById<FakeDisplayOrderRow>(777, q => q.Select(fld.ID));
                });
            }
        }

        [Fact]
        public void ByIdLoadsAllTableFields()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.ById<DisplayOrderRow>(id);
                Assert.NotNull(row);
                Assert.Equal(7, row.GroupID);
                Assert.Equal((short)1, row.IsActive);
                Assert.Equal(5, row.DisplayOrder);
            }
        }

        [Fact]
        public void ByIdUsesTrackWithChecksMode()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.ById<DisplayOrderRow>(id);
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.True(row.IsAssigned(fld.DisplayOrder));
                Assert.True(row.IsAssigned(fld.IsActive));
            }
        }

        [Fact]
        public void ByIdWithEditQueryUsesTrackWithChecksMode()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.ById<DisplayOrderRow>(id, q => q.Select(fld.ID).Select(fld.GroupID));
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.False(row.IsAssigned(fld.DisplayOrder));
                Assert.False(row.IsAssigned(fld.IsActive));
            }
        }

        [Fact]
        public void ByIdWithEditQueryLoadsNoImplicitFields()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.ById<DisplayOrderRow>(id, q => q.Select(fld.ID).Select(fld.GroupID));
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.Equal(id, row.ID.Value);
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.Equal(7, row.GroupID);
                Assert.False(row.IsAssigned(fld.IsActive));
                Assert.False(row.IsAssigned(fld.DisplayOrder));
            }
        }

        [Fact]
        public void TryByIdReturnsNullIfNotFound()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 1)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                Assert.NotNull(connection.TryById<DisplayOrderRow>(id));
                Assert.Null(connection.TryById<DisplayOrderRow>(id + 1));
            }
        }

        [Fact]
        public void TryByIdThrowsExceptionIfMoreThanOneResultReturns()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(FakeDisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 6)
                    .ExecuteAndGetID(connection);

                var error = Assert.Throws<InvalidOperationException>(() =>
                {
                    connection.ById<FakeDisplayOrderRow>(777);
                });
            }
        }

        [Fact]
        public void TryByIdWithEditQueryReturnsNullIfNotFound()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 1)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                Assert.NotNull(connection.TryById<DisplayOrderRow>(id, q => q.Select(fld.ID)));
                Assert.Null(connection.TryById<DisplayOrderRow>(id + 1, q => q.Select(fld.ID)));
            }
        }

        [Fact]
        public void TryByIdWithEditQueryThrowsExceptionIfMoreThanOneResultReturns()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                var fld = FakeDisplayOrderRow.Fields;

                new SqlDelete(FakeDisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 6)
                    .ExecuteAndGetID(connection);

                var error = Assert.Throws<InvalidOperationException>(() =>
                {
                    connection.TryById<FakeDisplayOrderRow>(777, q => q.Select(fld.ID));
                });
            }
        }

        [Fact]
        public void TryByIdLoadsAllTableFields()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.TryById<DisplayOrderRow>(id);
                Assert.NotNull(row);
                Assert.Equal(7, row.GroupID);
                Assert.Equal((short)1, row.IsActive);
                Assert.Equal(5, row.DisplayOrder);
            }
        }

        [Fact]
        public void TryByIdUsesTrackWithChecksMode()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.TryById<DisplayOrderRow>(id);
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.True(row.IsAssigned(fld.DisplayOrder));
                Assert.True(row.IsAssigned(fld.IsActive));
            }
        }

        [Fact]
        public void TryByIdWithEditQueryUsesTrackWithChecksMode()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.TryById<DisplayOrderRow>(id, q => q.Select(fld.ID).Select(fld.GroupID));
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.False(row.IsAssigned(fld.DisplayOrder));
                Assert.False(row.IsAssigned(fld.IsActive));
            }
        }

        [Fact]
        public void TryByIdWithEditQueryLoadsNoImplicitFields()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.TryById<DisplayOrderRow>(id, q => q.Select(fld.ID).Select(fld.GroupID));
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.Equal(id, row.ID.Value);
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.Equal(7, row.GroupID);
                Assert.False(row.IsAssigned(fld.IsActive));
                Assert.False(row.IsAssigned(fld.DisplayOrder));
            }
        }

        [Fact]
        public void SingleThrowsExceptionIfNotFound()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 1)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                Assert.NotNull(connection.Single<DisplayOrderRow>(new Criteria(fld.ID) == id));

                var error = Assert.Throws<ValidationError>(() =>
                {
                    connection.Single<DisplayOrderRow>(new Criteria(fld.ID) == (id + 1));
                });

                Assert.Equal("RecordNotFound", error.ErrorCode);
            }
        }

        [Fact]
        public void SingleThrowsExceptionIfMoreThanOneResultReturns()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                var fld = FakeDisplayOrderRow.Fields;

                new SqlDelete(FakeDisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 6)
                    .ExecuteAndGetID(connection);

                var error = Assert.Throws<InvalidOperationException>(() =>
                {
                    connection.Single<FakeDisplayOrderRow>(new Criteria(fld.GroupID) == 777);
                });
            }
        }

        [Fact]
        public void SingleWithEditQueryThrowsExceptionIfNotFound()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 1)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                Assert.NotNull(connection.Single<DisplayOrderRow>(q =>
                    q.Where(new Criteria(fld.ID) == id).Select(fld.ID)));

                var error = Assert.Throws<ValidationError>(() =>
                {
                    connection.Single<DisplayOrderRow>(q =>
                         q.Where(new Criteria(fld.ID) == (id + 1)).Select(fld.ID));
                });

                Assert.Equal("RecordNotFound", error.ErrorCode);
            }
        }

        [Fact]
        public void SingleWithEditQueryThrowsExceptionIfMoreThanOneResultReturns()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                var fld = FakeDisplayOrderRow.Fields;

                new SqlDelete(FakeDisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 6)
                    .ExecuteAndGetID(connection);

                var error = Assert.Throws<InvalidOperationException>(() =>
                {
                    connection.Single<FakeDisplayOrderRow>(q => 
                         q.Where(new Criteria(fld.GroupID) == 777).Select(fld.ID));
                });
            }
        }

        [Fact]
        public void SingleLoadsAllTableFields()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.Single<DisplayOrderRow>(new Criteria(fld.ID) == id);
                Assert.NotNull(row);
                Assert.Equal(7, row.GroupID);
                Assert.Equal((short)1, row.IsActive);
                Assert.Equal(5, row.DisplayOrder);
            }
        }

        [Fact]
        public void SingleUsesTrackWithChecksMode()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.Single<DisplayOrderRow>(new Criteria(fld.ID) == id);
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.True(row.IsAssigned(fld.DisplayOrder));
                Assert.True(row.IsAssigned(fld.IsActive));
            }
        }

        [Fact]
        public void SingleWithEditQueryUsesTrackWithChecksMode()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.Single<DisplayOrderRow>(q => 
                    q.Where(new Criteria(fld.ID) == id).Select(fld.ID).Select(fld.GroupID));
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.False(row.IsAssigned(fld.DisplayOrder));
                Assert.False(row.IsAssigned(fld.IsActive));
            }
        }

        [Fact]
        public void SingleWithEditQueryLoadsNoImplicitFields()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.Single<DisplayOrderRow>(q => 
                    q.Where(new Criteria(fld.ID) == id).Select(fld.ID).Select(fld.GroupID));
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.Equal(id, row.ID.Value);
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.Equal(7, row.GroupID);
                Assert.False(row.IsAssigned(fld.IsActive));
                Assert.False(row.IsAssigned(fld.DisplayOrder));
            }
        }

        [Fact]
        public void TrySingleReturnsNullIfNotFound()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 1)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                Assert.NotNull(connection.TrySingle<DisplayOrderRow>(new Criteria(fld.ID) == id));
                Assert.Null(connection.TrySingle<DisplayOrderRow>(new Criteria(fld.ID) == (id + 1)));
            }
        }

        [Fact]
        public void TrySingleThrowsExceptionIfMoreThanOneResultReturns()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(FakeDisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 6)
                    .ExecuteAndGetID(connection);

                var error = Assert.Throws<InvalidOperationException>(() =>
                {
                    connection.Single<FakeDisplayOrderRow>(new Criteria(fld.GroupID) == 777);
                });
            }
        }

        [Fact]
        public void TrySingleWithEditQueryReturnsNullIfNotFound()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 1)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                Assert.NotNull(connection.TrySingle<DisplayOrderRow>(q => 
                    q.Where(new Criteria(fld.ID) == id).Select(fld.ID)));
                Assert.Null(connection.TrySingle<DisplayOrderRow>(q => 
                    q.Where(new Criteria(fld.ID) == (id + 1)).Select(fld.ID)));
            }
        }

        [Fact]
        public void TrySingleWithEditQueryThrowsExceptionIfMoreThanOneResultReturns()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                var fld = FakeDisplayOrderRow.Fields;

                new SqlDelete(FakeDisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 6)
                    .ExecuteAndGetID(connection);

                var error = Assert.Throws<InvalidOperationException>(() =>
                {
                    connection.TrySingle<FakeDisplayOrderRow>(q =>
                         q.Where(new Criteria(fld.GroupID) == 777).Select(fld.ID));
                });
            }
        }

        [Fact]
        public void TrySingleLoadsAllTableFields()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.TrySingle<DisplayOrderRow>(new Criteria(fld.ID) == id);
                Assert.NotNull(row);
                Assert.Equal(7, row.GroupID);
                Assert.Equal((short)1, row.IsActive);
                Assert.Equal(5, row.DisplayOrder);
            }
        }

        [Fact]
        public void TrySingleUsesTrackWithChecksMode()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.TrySingle<DisplayOrderRow>(new Criteria(fld.ID) == id);
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.True(row.IsAssigned(fld.DisplayOrder));
                Assert.True(row.IsAssigned(fld.IsActive));
            }
        }

        [Fact]
        public void TrySingleWithEditQueryUsesTrackWithChecksMode()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.TrySingle<DisplayOrderRow>(q => 
                    q.Where(new Criteria(fld.ID) == id).Select(fld.ID).Select(fld.GroupID));
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.False(row.IsAssigned(fld.DisplayOrder));
                Assert.False(row.IsAssigned(fld.IsActive));
            }
        }

        [Fact]
        public void TrySingleWithEditQueryLoadsNoImplicitFields()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.TrySingle<DisplayOrderRow>(q => 
                    q.Where(new Criteria(fld.ID) == id).Select(fld.ID).Select(fld.GroupID));
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.Equal(id, row.ID.Value);
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.Equal(7, row.GroupID);
                Assert.False(row.IsAssigned(fld.IsActive));
                Assert.False(row.IsAssigned(fld.DisplayOrder));
            }
        }

        [Fact]
        public void FirstThrowsExceptionIfNotFound()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 1)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                Assert.NotNull(connection.First<DisplayOrderRow>(new Criteria(fld.ID) == id));

                var error = Assert.Throws<ValidationError>(() =>
                {
                    connection.First<DisplayOrderRow>(new Criteria(fld.ID) == (id + 1));
                });

                Assert.Equal("RecordNotFound", error.ErrorCode);
            }
        }

        [Fact]
        public void FirstDoesntThrowExceptionIfMoreThanOneResultReturns()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                var fld = FakeDisplayOrderRow.Fields;

                new SqlDelete(FakeDisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var firstID = new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var secondID = new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 6)
                    .ExecuteAndGetID(connection);

                var first = connection.First<FakeDisplayOrderRow>(new Criteria(fld.GroupID) == 777);

                Assert.True(first.ID == firstID || first.ID == secondID);
            }
        }

        [Fact]
        public void FirstWithEditQueryThrowsExceptionIfNotFound()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 1)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                Assert.NotNull(connection.First<DisplayOrderRow>(q =>
                    q.Where(new Criteria(fld.ID) == id).Select(fld.ID)));

                var error = Assert.Throws<ValidationError>(() =>
                {
                    connection.First<DisplayOrderRow>(q =>
                         q.Where(new Criteria(fld.ID) == (id + 1)).Select(fld.ID));
                });

                Assert.Equal("RecordNotFound", error.ErrorCode);
            }
        }

        [Fact]
        public void FirstWithEditQueryDoesntThrowExceptionIfMoreThanOneResultReturns()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                var fld = FakeDisplayOrderRow.Fields;

                new SqlDelete(FakeDisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var firstID = (int)new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var secondID = (int)new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 6)
                    .ExecuteAndGetID(connection);

                var first = connection.First<FakeDisplayOrderRow>(q =>
                        q.Where(new Criteria(fld.GroupID) == 777).Select(fld.ID));

                Assert.True(first.ID == firstID || first.ID == secondID);
            }
        }

        [Fact]
        public void FirstLoadsAllTableFields()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.First<DisplayOrderRow>(new Criteria(fld.ID) == id);
                Assert.NotNull(row);
                Assert.Equal(7, row.GroupID);
                Assert.Equal((short)1, row.IsActive);
                Assert.Equal(5, row.DisplayOrder);
            }
        }

        [Fact]
        public void FirstUsesTrackWithChecksMode()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.First<DisplayOrderRow>(new Criteria(fld.ID) == id);
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.True(row.IsAssigned(fld.DisplayOrder));
                Assert.True(row.IsAssigned(fld.IsActive));
            }
        }

        [Fact]
        public void FirstWithEditQueryUsesTrackWithChecksMode()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.First<DisplayOrderRow>(q =>
                    q.Where(new Criteria(fld.ID) == id).Select(fld.ID).Select(fld.GroupID));
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.False(row.IsAssigned(fld.DisplayOrder));
                Assert.False(row.IsAssigned(fld.IsActive));
            }
        }

        [Fact]
        public void FirstWithEditQueryLoadsNoImplicitFields()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.First<DisplayOrderRow>(q =>
                    q.Where(new Criteria(fld.ID) == id).Select(fld.ID).Select(fld.GroupID));
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.Equal(id, row.ID.Value);
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.Equal(7, row.GroupID);
                Assert.False(row.IsAssigned(fld.IsActive));
                Assert.False(row.IsAssigned(fld.DisplayOrder));
            }
        }

        [Fact]
        public void TryFirstReturnsNullIfNotFound()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 1)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                Assert.NotNull(connection.TryFirst<DisplayOrderRow>(new Criteria(fld.ID) == id));
                Assert.Null(connection.TryFirst<DisplayOrderRow>(new Criteria(fld.ID) == (id + 1)));
            }
        }

        [Fact]
        public void TryFirstDoesntThrowExceptionIfMoreThanOneResultReturns()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                var fld = FakeDisplayOrderRow.Fields;

                new SqlDelete(FakeDisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var firstID = new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var secondID = new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 6)
                    .ExecuteAndGetID(connection);

                var first = connection.TryFirst<FakeDisplayOrderRow>(new Criteria(fld.GroupID) == 777);

                Assert.True(first.ID == firstID || first.ID == secondID);
            }
        }

        [Fact]
        public void TryFirstWithEditQueryReturnsNullIfNotFound()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 1)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                Assert.NotNull(connection.TryFirst<DisplayOrderRow>(q =>
                    q.Where(new Criteria(fld.ID) == id).Select(fld.ID)));
                Assert.Null(connection.TryFirst<DisplayOrderRow>(q =>
                    q.Where(new Criteria(fld.ID) == (id + 1)).Select(fld.ID)));
            }
        }

        [Fact]
        public void TryFirstWithEditQueryDoesntThrowExceptionIfMoreThanOneResultReturns()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                var fld = FakeDisplayOrderRow.Fields;

                new SqlDelete(FakeDisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var firstID = (int)new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var secondID = (int)new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 6)
                    .ExecuteAndGetID(connection);

                var first = connection.TryFirst<FakeDisplayOrderRow>(q =>
                        q.Where(new Criteria(fld.GroupID) == 777).Select(fld.ID));

                Assert.True(first.ID == firstID || first.ID == secondID);
            }
        }

        [Fact]
        public void TryFirstLoadsAllTableFields()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.TryFirst<DisplayOrderRow>(new Criteria(fld.ID) == id);
                Assert.NotNull(row);
                Assert.Equal(7, row.GroupID);
                Assert.Equal((short)1, row.IsActive);
                Assert.Equal(5, row.DisplayOrder);
            }
        }

        [Fact]
        public void TryFirstUsesTrackWithChecksMode()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.TryFirst<DisplayOrderRow>(new Criteria(fld.ID) == id);
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.True(row.IsAssigned(fld.DisplayOrder));
                Assert.True(row.IsAssigned(fld.IsActive));
            }
        }

        [Fact]
        public void TryFirstWithEditQueryUsesTrackWithChecksMode()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.TryFirst<DisplayOrderRow>(q =>
                    q.Where(new Criteria(fld.ID) == id).Select(fld.ID).Select(fld.GroupID));
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.False(row.IsAssigned(fld.DisplayOrder));
                Assert.False(row.IsAssigned(fld.IsActive));
            }
        }

        [Fact]
        public void TryFirstWithEditQueryLoadsNoImplicitFields()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var row = connection.TryFirst<DisplayOrderRow>(q =>
                    q.Where(new Criteria(fld.ID) == id).Select(fld.ID).Select(fld.GroupID));
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.Equal(id, row.ID.Value);
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.Equal(7, row.GroupID);
                Assert.False(row.IsAssigned(fld.IsActive));
                Assert.False(row.IsAssigned(fld.DisplayOrder));
            }
        }

        [Fact]
        public void ListReturnsEmptyListIfNotFound()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 1)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var list = connection.List<DisplayOrderRow>(new Criteria(fld.ID) == (id + 1));

                Assert.NotNull(list);
                Assert.StrictEqual(0, list.Count);
            }
        }

        [Fact]
        public void ListCanLoadMoreThanOneResult()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                var fld = FakeDisplayOrderRow.Fields;

                new SqlDelete(FakeDisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var firstID = new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var secondID = new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 6)
                    .ExecuteAndGetID(connection);

                var list = connection.List<FakeDisplayOrderRow>(new Criteria(fld.GroupID) == 777);
                Assert.NotNull(list);
                Assert.StrictEqual(2, list.Count);
                Assert.True((list[0].ID == firstID && list[1].ID == secondID) ||
                    (list[0].ID == secondID && list[1].ID == firstID));
            }
        }

        [Fact]
        public void ListWithEditQueryReturnsEmptyListIfNotFound()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 1)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var list = connection.List<DisplayOrderRow>(new Criteria(fld.ID) == (id + 1));

                Assert.NotNull(list);
                Assert.StrictEqual(0, list.Count);
            }
        }

        [Fact]
        public void ListWithEditQueryCanLoadMoreThanOneResult()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                var fld = FakeDisplayOrderRow.Fields;

                new SqlDelete(FakeDisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var firstID = (int)new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var secondID = (int)new SqlInsert(FakeDisplayOrderRow.TableName)
                    .Set(fld.GroupID, 777)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 6)
                    .ExecuteAndGetID(connection);

                var list = connection.List<FakeDisplayOrderRow>(q =>
                        q.Where(new Criteria(fld.GroupID) == 777).Select(fld.ID));

                Assert.NotNull(list);
                Assert.StrictEqual(2, list.Count);
                Assert.True((list[0].ID == firstID && list[1].ID == secondID) ||
                    (list[0].ID == secondID && list[1].ID == firstID));
            }
        }

        [Fact]
        public void ListLoadsAllTableFields()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var list = connection.List<DisplayOrderRow>(new Criteria(fld.ID) == id);
                Assert.NotNull(list);
                Assert.StrictEqual(1, list.Count);
                var row = list[0];
                Assert.NotNull(row);
                Assert.Equal(7, row.GroupID);
                Assert.Equal((short)1, row.IsActive);
                Assert.Equal(5, row.DisplayOrder);
            }
        }

        [Fact]
        public void ListUsesTrackWithChecksMode()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var list = connection.List<DisplayOrderRow>(new Criteria(fld.ID) == id);
                Assert.NotNull(list);
                Assert.StrictEqual(1, list.Count);
                var row = list[0];
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.True(row.IsAssigned(fld.DisplayOrder));
                Assert.True(row.IsAssigned(fld.IsActive));
            }
        }

        [Fact]
        public void ListWithEditQueryUsesTrackWithChecksMode()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var list = connection.List<DisplayOrderRow>(q =>
                    q.Where(new Criteria(fld.ID) == id).Select(fld.ID).Select(fld.GroupID));
                Assert.NotNull(list);
                Assert.StrictEqual(1, list.Count);
                var row = list[0];
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.False(row.IsAssigned(fld.DisplayOrder));
                Assert.False(row.IsAssigned(fld.IsActive));
            }
        }

        [Fact]
        public void ListWithEditQueryLoadsNoImplicitFields()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection, ExpectedRows.Ignore);

                var id = (int)new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 7)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .ExecuteAndGetID(connection);

                var list = connection.List<DisplayOrderRow>(q =>
                    q.Where(new Criteria(fld.ID) == id).Select(fld.ID).Select(fld.GroupID));
                Assert.NotNull(list);
                Assert.StrictEqual(1, list.Count);
                var row = list[0];
                Assert.NotNull(row);
                Assert.True(row.TrackWithChecks);
                Assert.True(row.IsAssigned(fld.ID));
                Assert.Equal(id, row.ID.Value);
                Assert.True(row.IsAssigned(fld.GroupID));
                Assert.Equal(7, row.GroupID);
                Assert.False(row.IsAssigned(fld.IsActive));
                Assert.False(row.IsAssigned(fld.DisplayOrder));
            }
        }

    }
}