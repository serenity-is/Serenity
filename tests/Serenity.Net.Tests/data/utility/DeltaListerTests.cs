namespace Serenity.Data;

public class DeltaListerTests
{
    private static DeltaLister<int?> GetLister(int?[] oldIds, int?[] newIds,
        DeltaOptions options = DeltaOptions.Default)
    {
        return new DeltaLister<int?>([.. oldIds], [.. newIds], i => i, options);
    }

    [Fact]
    public void Constructor_NullArguments_ThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DeltaLister<int?>(null, [], i => i));
        Assert.Throws<ArgumentNullException>(() => new DeltaLister<int?>([], null, i => i));
        Assert.Throws<ArgumentNullException>(() => new DeltaLister<int?>([], [], null));
    }

    [Fact]
    public void ItemsToDelete_AreOldItemsNotInNewList()
    {
        var lister = GetLister([1, 2, 3], [2, 3, 4], DeltaOptions.IgnoreInvalidNewId);

        Assert.Equal([1], lister.ItemsToDelete.Cast<int?>().ToArray());
    }

    [Fact]
    public void ItemsToCreate_AreNewItemsWithoutOldId()
    {
        var lister = new DeltaLister<int?>([2], [3, 7], i => i == 7 ? null : i,
            DeltaOptions.IgnoreInvalidNewId);

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
    public void NewItemWithUnknownId_ThrowsByDefaultOption()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            GetLister([1], [99]));
    }

    [Fact]
    public void NewItemWithUnknownId_Allowed_WhenIgnoreInvalidNewIdSet()
    {
        var lister = GetLister([1], [99], DeltaOptions.IgnoreInvalidNewId);

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
    public void DuplicatedOldItemId_ThrowsArgumentException_WithId()
    {
        var ex = Assert.Throws<ArgumentException>(() => GetLister([2, 2], []));

        Assert.Contains("2", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GetItemId_InvokedOncePerItem()
    {
        var calls = 0;
        var lister = new DeltaLister<int?>([1, 2], [2], i =>
        {
            calls++;
            return i;
        });

        Assert.Equal(3, calls);
        _ = lister.ItemsToDelete.ToList();
        _ = lister.ItemsToCreate.ToList();
        _ = lister.ItemsToUpdate.ToList();
        _ = lister.ItemsToDelete.ToList();

        Assert.Equal(3, calls);
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
        var lister = new DeltaLister<int?>([1], [2], i => i == 2 ? null : i);

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
