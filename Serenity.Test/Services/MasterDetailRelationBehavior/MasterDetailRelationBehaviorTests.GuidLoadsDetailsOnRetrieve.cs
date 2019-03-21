using Serenity.Data;
using Serenity.Services;
using Serenity.Test.Data;
using Serenity.Testing;
using System;
using Xunit;

namespace Serenity.Test.Services
{
    public partial class MasterDetailRelationBehaviorTests
    {
        [Fact]
        public void GuidLoadsDetailsOnRetrieve()
        {
            using (new MunqContext())
            using (new DbTestContext<SerenityDbScript>())
            {
                using (var connection = SqlConnections.NewFor<GuidMasterRow>())
                {
                    var master1 = Guid.NewGuid();
                    connection.Insert(new GuidMasterRow
                    {
                        ID = master1,
                        Name = "Master1"
                    });

                    var master2 = Guid.NewGuid();
                    connection.Insert(new GuidMasterRow
                    {
                        ID = master2,
                        Name = "Master2"
                    });

                    var master3 = Guid.NewGuid();
                    connection.Insert(new GuidMasterRow
                    {
                        ID = master3,
                        Name = "Master3"
                    });

                    var detail11 = Guid.NewGuid();
                    connection.Insert(new GuidDetailRow
                    {
                        DetailID = detail11,
                        MasterID = master1,
                        ProductID = 1,
                        Quantity = 5
                    });

                    var guids = new Guid[]
                    {
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        Guid.NewGuid()
                    };

                    Array.Sort(guids);

                    var detail31 = guids[0];
                    connection.Insert(new GuidDetailRow
                    { 
                        DetailID = detail31,
                        MasterID = master3,
                        ProductID = 1,
                        Quantity = 7
                    });

                    var detail32 = guids[1];
                    connection.Insert(new GuidDetailRow
                    {
                        DetailID = detail32,
                        MasterID = master3,
                        ProductID = 3,
                        Quantity = 10
                    });

                    var detail33 = guids[2];
                    connection.Insert(new GuidDetailRow
                    {
                        DetailID = detail33,
                        MasterID = master3,
                        ProductID = 3,
                        Quantity = 12
                    });

                    var result1 = new RetrieveRequestHandler<GuidMasterRow>().Process(connection, 
                        new RetrieveRequest
                        {
                            EntityId = master1
                        });

                    Assert.NotNull(result1.Entity.DetailList);
                    Assert.StrictEqual(1, result1.Entity.DetailList.Count);
                    var result11 = result1.Entity.DetailList[0];
                    Assert.Equal(master1, result11.MasterID);
                    Assert.Equal(detail11, result11.DetailID);
                    Assert.Equal(1, result11.ProductID);
                    Assert.Equal(5, result11.Quantity);

                    var result2 = new RetrieveRequestHandler<GuidMasterRow>().Process(connection, 
                        new RetrieveRequest
                        {
                            EntityId = master2
                        });

                    Assert.NotNull(result2.Entity.DetailList);
                    Assert.StrictEqual(0, result2.Entity.DetailList.Count);

                    var result3 = new RetrieveRequestHandler<GuidMasterRow>().Process(connection, 
                        new RetrieveRequest
                        {
                            EntityId = master3
                        });

                    Assert.NotNull(result3.Entity.DetailList);
                    Assert.Equal(3, result3.Entity.DetailList.Count);

                    result3.Entity.DetailList.Sort((x, y) => x.DetailID.Value.CompareTo(y.DetailID.Value));

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