using Serenity.Data;
using Xunit;
using System.Linq;
using System;
using System.Data.SqlClient;
using Serenity.Testing;

namespace Serenity.Test.Data
{
    [Collection("AvoidParallel")]
    public class SqlHelperParamTests
    {
        private enum MyEnum32 : int
        {
            Value1 = 1,
            Value2 = 2
        }

        private enum MyEnum64 : long
        {
            Value1 = 1,
            Value2 = 2
        }

        private DisplayOrderRow.RowFields fld = DisplayOrderRow.Fields;

        private DbTestContext NewDbTestContext()
        {
            return new DbTestContext(DbOverride.New<SerenityDbScript>("Serenity", "DBSerenity"));
        }

        [Fact]
        public void SqlHelper_AddParamWithValue_HandlesEnumsCorrectly()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                var param32 = new SqlCommand()
                .AddParamWithValue(connection, "@name", MyEnum32.Value2);

                Assert.Equal(System.Data.DbType.Int32, param32.DbType);
                Assert.IsType(typeof(Int32), param32.Value);
                Assert.Equal((int)MyEnum32.Value2, param32.Value);

                var param64 = new SqlCommand()
                    .AddParamWithValue(connection, "@name", MyEnum64.Value2);

                Assert.Equal(System.Data.DbType.Int64, param64.DbType);
                Assert.IsType(typeof(Int64), param64.Value);
                Assert.Equal((long)MyEnum64.Value2, param64.Value);
            }
        }

        [Fact]
        public void SqlInsert_WorksWithEnums()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.DisplayOrder, MyEnum32.Value2)
                    .Set(fld.IsActive, 1)
                    .Execute(connection);

                var row = connection.Single<DisplayOrderRow>(Criteria.Empty);
                Assert.NotNull(row);
                Assert.Equal(row.DisplayOrder, (int)MyEnum32.Value2);
            }
        }
    }
}