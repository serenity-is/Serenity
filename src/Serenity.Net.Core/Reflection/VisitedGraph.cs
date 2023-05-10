namespace Serenity.Reflection;

class VisitedGraph : Dictionary<object, object>
{
    public new bool ContainsKey(object key)
    {
        if (key == null)
            return true;
        return base.ContainsKey(key);
    }

    public new object? this[object key]
    {
        get { if (key == null) return null; return base[key]; }
    }
}
