using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic;

public class HashTable<TKey, TValue> : IDictionary<TKey, TValue>
    where TValue : class
{
    protected IDictionary<TKey, TValue> _items = new Dictionary<TKey, TValue>();

    public TValue this[TKey key] { get => _items.TryGetValue(key, out var o) ? o : null; }

    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
        get => _items.TryGetValue(key, out var o) ? o : null;
        set => _items[key] = value;
    }

    public ICollection<TKey> Keys => _items.Keys;

    public ICollection<TValue> Values => _items.Values;

    public int Count => _items.Count;

    public bool IsReadOnly => _items.IsReadOnly;

    public void Add(TKey key, TValue value)
    {
        _items[key] = value;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        _items[item.Key] = item.Value;
    }

    public void Clear()
    {
        _items.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return _items.Contains(item);
    }

    public bool ContainsKey(TKey key)
    {
        return _items.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        _items.CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    public bool Remove(TKey key)
    {
        return _items.Remove(key);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return _items.Remove(item);
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return _items.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }
}