namespace Serenity.Extensions;

/// <summary>
/// Base user retrieve service that provides common functionality for user retrieve services.
/// </summary>
/// <typeparam name="TRow">User row type</typeparam>
/// <param name="cache">Cache</param>
/// <param name="sqlConnections">SQL connections</param>
public abstract class BaseUserRetrieveService<TRow>(ITwoLevelCache cache, ISqlConnections sqlConnections) 
    : BaseUserRetrieveService(cache)
    where TRow : class, IRow, IIdRow, INameRow, new()
{
    private readonly ISqlConnections sqlConnections = sqlConnections ?? throw new ArgumentNullException(nameof(sqlConnections));

    /// <summary>
    /// Converts the specified row to a user definition.
    /// </summary>
    /// <param name="row">User row</param>
    protected abstract IUserDefinition ToUserDefinition(TRow row);

    /// <summary>
    /// Loads a user from the database by the specified criteria.
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="criteria">Criteria</param>
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

    /// <summary>
    /// Gets the cache group key for user retrieval.
    /// </summary>
    protected override string GetCacheGroupKey()
    {
        return (cacheGroupKey ??= new TRow().Fields.GenerationKey);
    }

    /// <summary>
    /// Checks if the specified user ID is valid.
    /// </summary>
    /// <param name="userId">User ID</param>
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

    /// <summary>
    /// Loads the user by the specified ID from database.
    /// </summary>
    /// <param name="id">User ID</param>
    protected override IUserDefinition LoadById(string id)
    {
        idField ??= new TRow().IdField;
        using var connection = sqlConnections.NewFor<TRow>();

        return LoadByCriteria(connection, new Criteria(idField) == 
            new ValueCriteria(idField.ConvertValue(id, CultureInfo.InvariantCulture)));
    }

    /// <summary>
    /// Loads the user by the specified username from database.
    /// </summary>
    /// <param name="username">Username</param>
    protected override IUserDefinition LoadByUsername(string username)
    {
        nameField ??= new TRow().NameField;
        using var connection = sqlConnections.NewFor<TRow>();

        return LoadByCriteria(connection, new Criteria(nameField) ==
            new ValueCriteria(nameField.ConvertValue(username, CultureInfo.InvariantCulture)));
    }
}