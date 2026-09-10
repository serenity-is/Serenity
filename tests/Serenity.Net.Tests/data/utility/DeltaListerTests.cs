namespace Serenity.Data;

public class DeltaListerTests
{
    private static DeltaLister<int?> GetLister(int?[] oldIds, int?[] newIds,
        DeltaOptions options = DeltaOptions.Default)
    {
        return new DeltaLister<int?>(oldIds.ToList(), newIds.ToList(), i => i, options);
    }

    [Fact]
    public void Constructor_NullArguments_ThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DeltaLister<int?>(null!, [], i => i));
        Assert.Throws<ArgumentNullException>(() => new DeltaLister<int?>([], null!, i => i));
        Assert.Throws<ArgumentNullException>(() => new DeltaLister<int?>([], [], null!));
    }

    [Fact]
    public void ItemsToDelete_AreOldItemsNotInNewList()
    {
        var lister = GetLister([1, 2, 3], [2, 3, 4]);

        Assert.Equal([1], lister.ItemsToDelete.Cast<int?>().ToArray());
    }

    [Fact]
    public void ItemsToCreate_AreNewItemsWithoutOldId()
    {
        var lister = new DeltaLister<int?>(new int?[] { 2 }.ToList(), new int?[] { 3, 7 }.ToList(), i => i == 7 ? null : i);

        Assert.Equal([3, 7], lister.ItemsToCreate.Cast<int?>().ToArray());
    }

    [Fact]
    public void ItemsToUpdate_MatchesOldNewPairs()
    {
        var lister = GetLister([1, 2], [2, 1]);

        var pairs = lister.ItemsToUpdate.ToList();

        Assert.Equal(2, pairs.Count);
        Assert.Equal(2, pairs[0].Old);
        Assert.Equal(2, pairs[0].New);
        Assert.Equal(1, pairs[1].Old);
        Assert.Equal(1, pairs[1].New);
    }

    [Fact]
    public void NewItemWithUnknownId_AllowedByDefaultOption()
    {
        var lister = GetLister([1], [99]);

        Assert.Single(lister.ItemsToCreate);
        Assert.Single(lister.ItemsToDelete);
    }

    [Fact]
    public void NewItemWithUnknownId_Throws_WhenIgnoreInvalidNewIdNotSet()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            GetLister([1, 2], [99], 0));
    }

    [Fact]
    public void DuplicatedNewItemId_ThrowsArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => GetLister([1, 2], [1, 1]));
    }

    [Fact]
    public void OldItemWithNullId_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => GetLister([null], []));
    }

    [Fact]
    public void NewItemWithNullItemId_CreatesNotUpdates()
    {
        var lister = new DeltaLister<int?>(new int?[] { 1 }.ToList(), new int?[] { 2 }.ToList(), i => i == 2 ? null : i);

        Assert.Single(lister.ItemsToCreate);
        Assert.Empty(lister.ItemsToUpdate);
    }
}

public class OldNewPairTests
{
    [Fact]
    public void Properties_AreSet()
    {
        var lister = new DeltaLister<int?>([1], [1], i => i);
        var pair = lister.ItemsToUpdate.Single();

        Assert.Equal(1, pair.Old);
        Assert.Equal(1, pair.New);
    }

    [Fact]
    public void ObjectInitializationList_Constructor()
    {
        var pair = new OldNewPair<int>(1, 2);
        Assert.Equal(1, pair.Old);
        Assert.Equal(2, pair.New);
    }
}
