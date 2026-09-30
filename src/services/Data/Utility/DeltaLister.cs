namespace Serenity.Data;

/// <summary>
/// Helper class to find differences between two lists for updating.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
public class DeltaLister<TItem>
{
    private readonly DeltaOptions _options;
    private readonly Dictionary<long, TItem> _oldById;
    private readonly HashSet<long> _newById;
    private readonly List<(TItem Item, long? Id)> _oldEntries;
    private readonly List<(TItem Item, long? Id)> _newEntries;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeltaLister{TItem}"/> class.
    /// </summary>
    /// <param name="oldList">The old list.</param>
    /// <param name="newList">The new list.</param>
    /// <param name="getItemId">The function used to get the identifier of an item.
    /// Invoked exactly once per item, in the constructor.</param>
    /// <param name="options">The options.</param>
    /// <exception cref="ArgumentNullException">
    /// oldList, newList, getItemId, oldItem, oldItemId or newItem is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">newItemId is not present in the old list.</exception>
    /// <exception cref="ArgumentOutOfRangeException">newItemId is duplicated in the new list.</exception>
    /// <exception cref="ArgumentException">An id is duplicated in the old list.</exception>
    public DeltaLister(IEnumerable<TItem> oldList, IEnumerable<TItem> newList,
        Func<TItem, long?> getItemId, DeltaOptions options = DeltaOptions.Default)
    {
        _options = options;
        ArgumentNullException.ThrowIfNull(oldList);
        ArgumentNullException.ThrowIfNull(newList);
        ArgumentNullException.ThrowIfNull(getItemId);

        _oldById = [];
        _newById = [];
        _oldEntries = [];
        _newEntries = [];

        foreach (var item in oldList)
        {
            var id = ArgumentChecks.NotNull(getItemId(ArgumentChecks.NotNull(item, "oldItem")), "oldItemId");
            if (!_oldById.TryAdd(id, item))
                throw new ArgumentException($"Duplicate id {id} in old list.", nameof(oldList));
            _oldEntries.Add((item, id));
        }

        foreach (var item in newList)
        {
            var id = getItemId(ArgumentChecks.NotNull(item, "newItem"));
            if (id != null)
            {
                if (!_oldById.ContainsKey(id.Value))
                {
                    if ((_options & DeltaOptions.IgnoreInvalidNewId) != DeltaOptions.IgnoreInvalidNewId)
                        throw ArgumentExceptions.OutOfRange(id, "newItemId");
                }

                if (_newById.Contains(id.Value))
                    throw ArgumentExceptions.OutOfRange(id, "newItemId");

                _newById.Add(id.Value);
            }
            _newEntries.Add((item, id));
        }
    }

    /// <summary>
    /// Gets the items to delete.
    /// </summary>
    /// <value>
    /// The items to delete.
    /// </value>
    public IEnumerable<TItem> ItemsToDelete
    {
        get
        {
            foreach (var (item, id) in _oldEntries)
            {
                if (!_newById.Contains(id!.Value))
                    yield return item;
            }
        }
    }

    /// <summary>
    /// Gets the items to create.
    /// </summary>
    /// <value>
    /// The items to create.
    /// </value>
    public IEnumerable<TItem> ItemsToCreate
    {
        get
        {
            foreach (var (item, id) in _newEntries)
            {
                if (id == null || !_oldById.ContainsKey(id.Value))
                    yield return item;
            }
        }
    }

    /// <summary>
    /// Gets the items to update.
    /// </summary>
    /// <value>
    /// The items to update.
    /// </value>
    public IEnumerable<OldNewPair<TItem>> ItemsToUpdate
    {
        get
        {
            foreach (var (item, id) in _newEntries)
            {
                if (id != null && _oldById.TryGetValue(id.Value, out TItem? old))
                    yield return new OldNewPair<TItem>(old, item);
            }
        }
    }
}