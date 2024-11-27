namespace Serenity.Tests.Services;

public partial class MasterDetailRelationBehaviorTests
{
    [Fact]
    public void LoadsDetailsOnRetrieve_Int32Keys()
    {
        using var connection = new MockDbConnection();

        var detailListHandler = new MockListHandler<Int32DetailRow>(handler =>
        {
            var request = handler.Request;
            Assert.NotNull(request);
            var criteria = Assert.IsType<BinaryCriteria>(request.Criteria);
            var operand = Assert.IsType<Criteria>(criteria.LeftOperand);
            Assert.Equal("MasterID", operand.Expression);
            Assert.Equal(CriteriaOperator.EQ, criteria.Operator);
            var value = Assert.IsType<ValueCriteria>(criteria.RightOperand);
            Assert.Equal(123, value.Value);

            handler.Response.Entities.Add(new Int32DetailRow
            {
                DetailID = 456
            });

            handler.Response.Entities.Add(new Int32DetailRow
            {
                DetailID = 789
            });
        });

        var handlerFactory = new MockHandlerFactory((rowType, intf) =>
        {
            Assert.Equal(typeof(Int32DetailRow), rowType);
            Assert.Equal(typeof(IListRequestProcessor), intf);
            return detailListHandler;
        });

        var masterRetrieveHandler = new MockRetrieveHandler<Int32MasterRow>();
        masterRetrieveHandler.Row.ID = 123;
        var behavior = new MasterDetailRelationBehavior(handlerFactory)
        {
            Target = masterRetrieveHandler.Row.GetFields().DetailList
        };
        behavior.ActivateFor(masterRetrieveHandler.Row);
        behavior.OnReturn(masterRetrieveHandler);

        Assert.Collection(masterRetrieveHandler.Row.DetailList,
            x1 => Assert.Equal(456, x1.DetailID),
            x2 => Assert.Equal(789, x2.DetailID));
    }
}