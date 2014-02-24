using Serenity.Data;
using Serenity.Testing;
using Serenity.Test.Testing;
using System;
using Xunit;
using Xunit.Extensions;
using System.Linq;

namespace Serenity.Test.Data
{
    public partial class DisplayOrderTests
    {
        private DisplayOrderRow.RowFields fld = DisplayOrderRow.Fields;

        private DbTestContext NewDbTestContext()
        {
            return new DbTestContext(DbOverride.New<SerenityDbScript>("Serenity", "DBSerenity"));
        }

        [Fact]
        public void NextDisplayOrderValueReturnOneWhenTableIsEmpty()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection);

                var next = DisplayOrderHelper.GetNextValue(connection, DisplayOrderRow.Instance);
                Assert.Equal(1, next);
            }
        }

        [Fact]
        public void NextDisplayOrderValueReturnOneMoreThanMax()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                new SqlDelete(DisplayOrderRow.TableName)
                    .Execute(connection);

                new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 1)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 5)
                    .Execute(connection);

                new SqlInsert(DisplayOrderRow.TableName)
                    .Set(fld.GroupID, 2)
                    .Set(fld.IsActive, 1)
                    .Set(fld.DisplayOrder, 15)
                    .Execute(connection);

                var next = DisplayOrderHelper.GetNextValue(connection, DisplayOrderRow.Instance);
                Assert.Equal(16, next);

                next = DisplayOrderHelper.GetNextValue(connection, DisplayOrderRow.Instance, new Criteria(fld.GroupID) == 1);
                Assert.Equal(6, next);

                next = DisplayOrderHelper.GetNextValue(connection, DisplayOrderRow.Instance, new Criteria(fld.GroupID) == 2);
                Assert.Equal(16, next);

                next = DisplayOrderHelper.GetNextValue(connection, DisplayOrderRow.Instance, new Criteria(fld.GroupID) == -99);
                Assert.Equal(1, next);

                next = DisplayOrderHelper.GetNextValue(connection, DisplayOrderRow.TableName, DisplayOrderRow.Fields.DisplayOrder, 
                    new Criteria(fld.GroupID) == 1);
                Assert.Equal(6, next);
            }
        }

        [Fact]
        public void ReorderWithNoSpecificItemSetsValuesProperly()
        {
            using (var dbContext = NewDbTestContext())
            using (var connection = SqlConnections.NewByKey("Serenity"))
            {
                foreach (var data in new[] { /* input in id order, expected output in same order */ 
                    new Tuple<int[], int[]>(new[] { 0, 0, 0, 0, 0 }, new[] { 1, 2, 3, 4, 5 }),
                    new Tuple<int[], int[]>(new[] { 1, 5, 3, 0, 4 }, new[] { 2, 5, 3, 1, 4 }),
                    new Tuple<int[], int[]>(new[] { 1, 2, 2, 9, 0 }, new[] { 2, 3, 4, 5, 1 }),
                    new Tuple<int[], int[]>(new[] { 0, 0, 3, 1, 4 }, new[] { 1, 2, 4, 3, 5 })
                })
                {
                    new SqlDelete(DisplayOrderRow.TableName)
                        .Execute(connection);

                    for (var i = 0; i < data.Item1.Length; i++)
                    {
                        new SqlInsert(DisplayOrderRow.TableName)
                            .Set(fld.GroupID, 1)
                            .Set(fld.IsActive, 1)
                            .Set(fld.DisplayOrder, data.Item1[i])
                            .Execute(connection);
                    }

                    DisplayOrderHelper.ReorderValues(connection, DisplayOrderRow.Instance);

                    var row = new DisplayOrderRow();
                    var actual = new SqlQuery().From(row).Select(fld.DisplayOrder).OrderBy(fld.ID)
                        .List(connection, row).Select(x => x.DisplayOrder.Value).ToArray();

                    Assert.Equal(data.Item2, actual);
                }
            }
        }
    }
}