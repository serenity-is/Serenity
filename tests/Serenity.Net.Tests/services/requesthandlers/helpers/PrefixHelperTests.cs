namespace Serenity;

public class PrefixHelperTests
{
    [Fact]
    public void DeterminePrefixLength_Returns_Zero_For_Empty_List()
    {
        Assert.Equal(0, PrefixHelper.DeterminePrefixLength(Array.Empty<string>(), x => x));
    }

    [Fact]
    public void DeterminePrefixLength_Returns_Zero_When_No_Underscore()
    {
        Assert.Equal(0, PrefixHelper.DeterminePrefixLength(["Name", "Other"], x => x));
    }

    [Fact]
    public void DeterminePrefixLength_Returns_Zero_When_Underscore_First()
    {
        Assert.Equal(0, PrefixHelper.DeterminePrefixLength(["_Name", "_Other"], x => x));
    }

    [Fact]
    public void DeterminePrefixLength_Returns_Common_Prefix_Length_Plus_One()
    {
        Assert.Equal(4, PrefixHelper.DeterminePrefixLength(["ABC_Name", "ABC_Other"], x => x));
    }

    [Fact]
    public void DeterminePrefixLength_Returns_Zero_When_Some_Item_Differs()
    {
        Assert.Equal(0, PrefixHelper.DeterminePrefixLength(["ABC_Name", "XYZ_Other"], x => x));
    }
}
