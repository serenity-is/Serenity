namespace Serenity.Navigation;

public class NavigationHelperTests
{
    [Fact]
    public void HealOrders_ReturnsOrdersUnchanged_WhenAllAreDistinct()
    {
        var result = NavigationHelper.HealOrders([100, 200, 300]);

        Assert.Equal([100m, 200m, 300m], result);
    }

    [Fact]
    public void HealOrders_PreservesFractionalCmsOrders()
    {
        var result = NavigationHelper.HealOrders([100m, 100.5m, 101m]);

        Assert.Equal([100m, 100.5m, 101m], result);
    }

    [Fact]
    public void HealOrders_ThrowsArgumentNull_ForNullOrders()
    {
        Assert.Throws<ArgumentNullException>(() => NavigationHelper.HealOrders(null));
    }

    [Fact]
    public void HealOrders_KeepsFirstOfEachDuplicateGroupUnchanged()
    {
        var result = NavigationHelper.HealOrders([100, 100, 100, 200]);

        Assert.Equal(100m, result[0]);
        Assert.Equal(200m, result[3]);
    }

    [Fact]
    public void HealOrders_SpreadsDuplicatesStrictlyBetweenBaseAndNextDistinctOrder()
    {
        var result = NavigationHelper.HealOrders([100, 100, 100, 200]);

        Assert.True(result[0] < result[1]);
        Assert.True(result[1] < result[2]);
        Assert.True(result[2] < result[3]);
    }

    [Fact]
    public void HealOrders_HandlesDuplicateGroupWithNoFollowingOrder()
    {
        var result = NavigationHelper.HealOrders([100, 200, 200]);

        Assert.Equal(100m, result[0]);
        Assert.Equal(200m, result[1]);
        Assert.True(result[2] > 200m);
    }

    [Fact]
    public void HealOrders_HandlesMultipleIndependentDuplicateGroups()
    {
        var result = NavigationHelper.HealOrders([100, 100, 200, 300, 300, 300, 400]);

        Assert.Equal(7, result.Count);
        for (var i = 1; i < result.Count; i++)
            Assert.True(result[i - 1] < result[i]);
    }

    [Fact]
    public void HealOrders_ReturnsEmpty_ForEmptyInput()
    {
        var result = NavigationHelper.HealOrders([]);

        Assert.Empty(result);
    }

    [Fact]
    public void HealOrders_ResolvesSingleSentinelOrder_AfterLastRealSiblingOrder()
    {
        var result = NavigationHelper.HealOrders([9800, int.MaxValue]);

        Assert.Equal(9800m, result[0]);
        Assert.Equal(9900m, result[1]);
    }

    [Fact]
    public void HealOrders_ResolvesMultipleSentinelOrders_SpacedAfterLastRealSiblingOrder()
    {
        var result = NavigationHelper.HealOrders([9800, int.MaxValue, int.MaxValue, int.MaxValue]);

        Assert.Equal([9800m, 9900m, 10000m, 10100m], result);

        result = NavigationHelper.HealOrders([9850, int.MaxValue, int.MaxValue, int.MaxValue]);

        Assert.Equal([9850m, 9950m, 10050m, 10150m], result);

        result = NavigationHelper.HealOrders([9850, int.MaxValue, 9900, int.MaxValue, int.MaxValue]);
        Assert.Equal([9850m, 9950m, 9900m, 10000m, 10100m], result);

    }

    [Fact]
    public void HealOrders_ResolvesSentinelOrder_UsingParentOrder_WhenNoRealSiblingOrder()
    {
        var result = NavigationHelper.HealOrders([int.MaxValue], parentOrder: 8000m);

        Assert.Equal(8100m, result[0]);
    }

    [Fact]
    public void HealOrders_ResolvesSentinelOrder_UsingZeroBaseline_WhenNoRealSiblingOrNoParentOrder()
    {
        var result = NavigationHelper.HealOrders([int.MaxValue, int.MaxValue]);

        Assert.Equal([100m, 200m], result);
    }
}
