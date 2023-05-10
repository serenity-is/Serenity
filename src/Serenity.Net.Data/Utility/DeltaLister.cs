namespace Serenity.Data;

/// <summary>
/// Helper class to find differences between to lists for updating
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
public class DeltaLister<TItem>
{
    private readonly DeltaOptions _options;
    private readonly Dictionary<long, TItem> _oldById;
    private readonly HashSet<long> _newById;
    private readonly IEnumerable<TItem> _oldItems;
    private readonly IEnumerable<TItem> _newItems;
    private readonly Func<TItem, long?> _getItemId;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeltaLister{TItem}"/> class.
    /// </summary>
    /// <param name="oldList">The old list.</param>
    /// <param name="newList">The new list.</param>
    /// <param name="getItemId">The get item identifier.</param>
    /// <param name="options">The options.</param>
    /// <exception cref="ArgumentNullException">
    /// oldList or newList or getItemId or oldItem or oldItemId or newItem is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">newItemId</exception>
    /// <exception cref="DuplicateNameException">newItemId</exception>
    public DeltaLister(IEnumerable<TItem> oldList, IEnumerable<TItem> newList,
        Func<TItem, long?> getItemId, DeltaOptions options = DeltaOptions.Default)
    {
        _options = options;
        _oldItems = oldList ?? throw new ArgumentNullException("oldList");
        _newItems = newList ?? throw new ArgumentNullException("newList");
        _getItemId = getItemId ?? throw new ArgumentNullException("getItemId");

        _oldById = new Dictionary<long, TItem>();
        _newById = new HashSet<long>();

        foreach (var item in oldList)
        {
            if (item == null)
                throw new ArgumentNullException("oldItem");

            var id = getItemId(item);
            if (id == null)
                throw new ArgumentNullException("oldItemId");

            _oldById.Add(id.Value, item);
        }

        foreach (var item in newList)
        {
            if (item == null)
                throw new ArgumentNullException("newItem");

            var id = getItemId(item);
            if (id != null)
            {
                if (!_oldById.ContainsKey(id.Value))
                {
                    if ((_options & DeltaOptions.IgnoreInvalidNewId) != DeltaOptions.IgnoreInvalidNewId)
                        throw new ArgumentOutOfRangeException("newItemId");
                }

                if (_newById.Contains(id.Value))
                    throw new ArgumentException("newItemId");

                _newById.Add(id.Value);
            }
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
            foreach (var item in _oldItems)
            {
                var id = _getItemId(item);
                if (!_newById.Contains(id.Value))
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
            foreach (var item in _newItems)
            {
                var id = _getItemId(item);
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
            foreach (var item in _newItems)
            {
                var id = _getItemId(item);
                if (id != null && _oldById.TryGetValue(id.Value, out TItem old))
                    yield return new OldNewPair<TItem>(old, item);
            }
        }
    }
}