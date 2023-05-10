namespace Serenity.Tests.Services;

public partial class MasterDetailRelationBehaviorTests
{
    [Fact]
    public void LoadsDetailsOnRetrieve_GuidKeys()
    {
        using var connection = new MockDbConnection();

        var masterGuid = Guid.NewGuid();
        var detailGuid1 = Guid.NewGuid();
        var detailGuid2 = Guid.NewGuid();

        var detailListHandler = new MockListHandler<GuidDetailRow>(handler =>
        {
            var request = handler.Request;
            Assert.NotNull(request);
            var criteria = Assert.IsType<BinaryCriteria>(request.Criteria);
            var operand = Assert.IsType<Criteria>(criteria.LeftOperand);
            Assert.Equal("MasterID", operand.Expression);
            Assert.Equal(CriteriaOperator.EQ, criteria.Operator);
            var value = Assert.IsType<ValueCriteria>(criteria.RightOperand);
            Assert.Equal(masterGuid, value.Value);

            handler.Response.Entities.Add(new GuidDetailRow
            {
                DetailID = detailGuid1
            });

            handler.Response.Entities.Add(new GuidDetailRow
            {
                DetailID = detailGuid2
            });
        });

        var handlerFactory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(GuidDetailRow), rowType);
            Assert.Equal(typeof(IListRequestProcessor), intf);
            return detailListHandler;
        });

        var masterRetrieveHandler = new MockRetrieveHandler<GuidMasterRow>();
        masterRetrieveHandler.Row.ID = masterGuid;
        var behavior = new MasterDetailRelationBehavior(handlerFactory)
        {
            Target = masterRetrieveHandler.Row.GetFields().DetailList
        };
        behavior.ActivateFor(masterRetrieveHandler.Row);
        behavior.OnReturn(masterRetrieveHandler);

        Assert.Collection(masterRetrieveHandler.Row.DetailList,
            x1 => Assert.Equal(detailGuid1, x1.DetailID),
            x2 => Assert.Equal(detailGuid2, x2.DetailID));
    }
}