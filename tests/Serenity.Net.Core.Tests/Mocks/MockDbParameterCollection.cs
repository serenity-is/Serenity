using System.Collections;
using System.Data.Common;

namespace Serenity.Tests;

public class MockDbParameterCollection : DbParameterCollection
{
    private readonly List<IDbDataParameter> parameters = new();

    public override int Count => parameters.Count;

    public override object SyncRoot => throw new NotImplementedException();

    public override int Add(object value)
    {
        parameters.Add((IDbDataParameter)value);
        return parameters.Count - 1;
    }

    public override void AddRange(Array values)
    {
        foreach (var value in values)
            parameters.Add((IDbDataParameter)value);
    }

    public override void Clear()
    {
        parameters.Clear();
    }

    public override bool Contains(object value)
    {
        return parameters.Any(x => x.Value == value);
    }

    public override bool Contains(string value)
    {
        return parameters.Any(x => x.ParameterName == value);
    }

    public override void CopyTo(Array array, int index)
    {
        parameters.ToArray().CopyTo(array, index);
    }

    public override IEnumerator GetEnumerator()
    {
        return parameters.GetEnumerator();
    }

    public override int IndexOf(object value)
    {
        return parameters.IndexOf(value as IDbDataParameter);
    }

    public override int IndexOf(string parameterName)
    {
        return parameters.FindIndex(x => x.ParameterName == parameterName);
    }

    public override void Insert(int index, object value)
    {
        parameters.Insert(index, (IDbDataParameter)value);
    }

    public override void Remove(object value)
    {
        parameters.Remove((IDbDataParameter)value);
    }

    public override void RemoveAt(int index)
    {
        parameters.RemoveAt(index);
    }

    public override void RemoveAt(string parameterName)
    {
        parameters.RemoveAt(IndexOf(parameterName));
    }

    protected override DbParameter GetParameter(int index)
    {
        return (DbParameter)parameters[index];
    }

    protected override DbParameter GetParameter(string parameterName)
    {
        return (DbParameter)parameters.Find(x => x.ParameterName == parameterName);
    }

    protected override void SetParameter(int index, DbParameter value)
    {
        parameters[index] = value;
    }

    protected override void SetParameter(string parameterName, DbParameter value)
    {
        parameters[IndexOf(parameterName)] = value;
    }
}