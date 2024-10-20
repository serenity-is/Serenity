namespace Serenity.Extensions;

public abstract class BaseUserRetrieveService<TRow>(ITwoLevelCache cache, ISqlConnections sqlConnections) 
    : BaseUserRetrieveService(cache)
    where TRow : class, IRow, IIdRow, INameRow, new()
{
    private readonly ISqlConnections sqlConnections = sqlConnections ?? throw new ArgumentNullException(nameof(sqlConnections));

    protected abstract IUserDefinition ToUserDefinition(TRow row);

    protected virtual IUserDefinition LoadByCriteria(System.Data.IDbConnection connection, BaseCriteria criteria)
    {
        var user = connection.TrySingle<TRow>(criteria);
        if (user != null)
            return ToUserDefinition(user);

        return null;
    }

    private string cacheGroupKey;
    private Field idField;
    private Field nameField;

    protected override string GetCacheGroupKey()
    {
        return (cacheGroupKey ??= new TRow().Fields.GenerationKey);
    }

    protected override bool IsValidUserId(string userId)
    {
        if (!base.IsValidUserId(userId))
            return false;

        idField ??= new TRow().IdField;
        if (idField is Int32Field)
            return int.TryParse(userId, CultureInfo.InvariantCulture, out _);

        if (idField is Int64Field)
            return long.TryParse(userId, CultureInfo.InvariantCulture, out _);

        if (idField is GuidField)
            return Guid.TryParse(userId, out _);

        return true;
    }

    protected override IUserDefinition LoadById(string id)
    {
        idField ??= new TRow().IdField;
        using var connection = sqlConnections.NewFor<TRow>();

        return LoadByCriteria(connection, new Criteria(idField) == 
            new ValueCriteria(idField.ConvertValue(id, CultureInfo.InvariantCulture)));
    }

    protected override IUserDefinition LoadByUsername(string username)
    {
        nameField ??= new TRow().NameField;
        using var connection = sqlConnections.NewFor<TRow>();

        return LoadByCriteria(connection, new Criteria(nameField) ==
            new ValueCriteria(nameField.ConvertValue(username, CultureInfo.InvariantCulture)));
    }
}