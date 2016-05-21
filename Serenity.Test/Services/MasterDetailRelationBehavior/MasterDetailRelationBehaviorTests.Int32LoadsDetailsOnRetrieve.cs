using Serenity.Data;
using Serenity.Services;
using Serenity.Test.Data;
using Serenity.Testing;
using Xunit;

namespace Serenity.Test.Services
{
    [Collection("AvoidParallel")]
    public partial class MasterDetailRelationBehaviorTests : SerenityTestBase
    {
        [Fact]
        public void LoadsDetailsOnRetrieve()
        {
            using (new DbTestContext<SerenityDbScript>())
            {
                using (var connection = SqlConnections.NewFor<Int32MasterRow>())
                {
                    var master1 = (int)connection.InsertAndGetID(new Int32MasterRow
                    {
                        Name = "Master1"
                    });

                    var master2 = (int)connection.InsertAndGetID(new Int32MasterRow
                    {
                        Name = "Master2"
                    });

                    var master3 = (int)connection.InsertAndGetID(new Int32MasterRow
                    {
                        Name = "Master3"
                    });

                    var detail11 = (int)connection.InsertAndGetID(new Int32DetailRow
                    {
                        MasterID = master1,
                        ProductID = 1,
                        Quantity = 5
                    });

                    var detail31 = (int)connection.InsertAndGetID(new Int32DetailRow
                    {
                        MasterID = master3,
                        ProductID = 1,
                        Quantity = 7
                    });

                    var detail32 = (int)connection.InsertAndGetID(new Int32DetailRow
                    {
                        MasterID = master3,
                        ProductID = 3,
                        Quantity = 10
                    });

                    var detail33 = (int)connection.InsertAndGetID(new Int32DetailRow
                    {
                        MasterID = master3,
                        ProductID = 3,
                        Quantity = 12
                    });

                    var result1 = new RetrieveRequestHandler<Int32MasterRow>().Process(connection, 
                        new RetrieveRequest
                        {
                            EntityId = master1
                        });

                    Assert.NotNull(result1.Entity.DetailList);
                    Assert.Equal(1, result1.Entity.DetailList.Count);
                    var result11 = result1.Entity.DetailList[0];
                    Assert.Equal(master1, result11.MasterID);
                    Assert.Equal(detail11, result11.DetailID);
                    Assert.Equal(1, result11.ProductID);
                    Assert.Equal(5, result11.Quantity);

                    var result2 = new RetrieveRequestHandler<Int32MasterRow>().Process(connection, 
                        new RetrieveRequest
                        {
                            EntityId = master2
                        });

                    Assert.NotNull(result2.Entity.DetailList);
                    Assert.Equal(0, result2.Entity.DetailList.Count);

                    var result3 = new RetrieveRequestHandler<Int32MasterRow>().Process(connection, 
                        new RetrieveRequest
                        {
                            EntityId = master3
                        });

                    Assert.NotNull(result3.Entity.DetailList);
                    Assert.Equal(3, result3.Entity.DetailList.Count);

                    result3.Entity.DetailList.Sort((x, y) => 
                        (x.DetailID ?? 0).CompareTo(y.DetailID ?? 0));

                    var result31 = result3.Entity.DetailList[0];
                    Assert.Equal(master3, result31.MasterID);
                    Assert.Equal(detail31, result31.DetailID);
                    Assert.Equal(1, result31.ProductID);
                    Assert.Equal(7, result31.Quantity);

                    var result32 = result3.Entity.DetailList[1];
                    Assert.Equal(master3, result32.MasterID);
                    Assert.Equal(detail32, result32.DetailID);
                    Assert.Equal(3, result32.ProductID);
                    Assert.Equal(10, result32.Quantity);

                    var result33 = result3.Entity.DetailList[2];
                    Assert.Equal(master3, result33.MasterID);
                    Assert.Equal(detail33, result33.DetailID);
                    Assert.Equal(3, result33.ProductID);
                    Assert.Equal(12, result33.Quantity);
                }
            }
        }
    }
}