namespace Serenity.CodeGenerator;

public class TestRowPropertyMetadata : IRowPropertyMetadata
{
    public string PropertyName { get; set; } = null!;
    public string? ColumnName { get; set; }
}

public class TestRowMetadata : IRowMetadata
{
    public string? Namespace { get; set; }
    public string ClassName { get; set; } = null!;

    public List<TestRowPropertyMetadata> Props { get; } = [];

    public bool HasLookupScriptAttribute { get; set; }
    public string? ListServiceRoute { get; set; }
    public string? IdProperty { get; set; }
    public string? NameProperty { get; set; }
    public string? Module { get; set; }

    public virtual IRowPropertyMetadata? GetTableField(string columnName)
    {
        return Props.FirstOrDefault(x =>
            string.Equals(x.ColumnName, columnName, StringComparison.OrdinalIgnoreCase));
    }

    public virtual IRowPropertyMetadata? GetProperty(string name)
    {
        return Props.FirstOrDefault(x => x.PropertyName == name);
    }
}

public class TestApplicationMetadata : IApplicationMetadata
{
    public Dictionary<string, IRowMetadata> Rows { get; } = [];

    public IRowMetadata? GetRowByTablename(string tablename)
    {
        return Rows.TryGetValue(tablename, out var row) ? row : null;
    }
}
