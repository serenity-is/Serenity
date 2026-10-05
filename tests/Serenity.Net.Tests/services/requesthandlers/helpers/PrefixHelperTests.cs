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

    [Fact]
    public void DeterminePrefixLength_Enumerates_List_Only_Once()
    {
        int count = 0;

        IEnumerable<string> Lazy()
        {
            foreach (var name in new[] { "ABC_Name", "ABC_Other" })
            {
                count++;
                yield return name;
            }
        }

        Assert.Equal(4, PrefixHelper.DeterminePrefixLength(Lazy(), x => x));
        Assert.Equal(2, count);
    }

    [Fact]
    public void DeterminePrefixLength_Throws_For_Null_List()
    {
        Assert.Throws<ArgumentNullException>(() =>
            PrefixHelper.DeterminePrefixLength<string>(null, x => x));
    }

    [Fact]
    public void DeterminePrefixLength_Throws_For_Null_GetName()
    {
        Assert.Throws<ArgumentNullException>(() =>
            PrefixHelper.DeterminePrefixLength(["ABC_Name"], null));
    }
}
