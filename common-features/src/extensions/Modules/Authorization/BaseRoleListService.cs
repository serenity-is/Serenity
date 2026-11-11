namespace Serenity.Extensions;

/// <summary>
/// Base role list service that lists the roles from their rows. The name of a role is the name field of the row, and
/// its key is read from it when it has <see cref="IRoleKeyRow"/>.
/// </summary>
/// <typeparam name="TRow">Role row type</typeparam>
/// <param name="sqlConnections">SQL connections</param>
public abstract class BaseRoleListService<TRow>(ISqlConnections sqlConnections) : IRoleListService
    where TRow : class, IRow, IIdRow, INameRow, new()
{
    private readonly ISqlConnections sqlConnections = sqlConnections ?? throw new ArgumentNullException(nameof(sqlConnections));

    /// <inheritdoc/>
    public virtual IReadOnlyList<IRoleDefinition> ListRoles(RoleListOptions? options = null)
    {
        options ??= new();

        var row = new TRow();
        var criteria = GetCriteria(row, options);

        using var connection = sqlConnections.NewFor<TRow>();
        var rows = connection.List<TRow>(query =>
        {
            query.Select(row.GetIdField()).Select(GetNameField(row));

            if (row is IRoleKeyRow roleKey)
                query.Select(roleKey.RoleKeyField);

            if (!criteria.IsEmpty)
                query.Where(criteria);

            query.OrderBy(GetNameField(row));

            if (options.Take > 0)
                query.Take(options.Take);
        });

        return [.. rows.Select(ToListItem)];
    }

    /// <summary>
    /// Gets the criteria of the roles to list, from the options.
    /// </summary>
    /// <param name="row">A role row, which has the fields.</param>
    /// <param name="options">The options.</param>
    protected virtual BaseCriteria GetCriteria(TRow row, RoleListOptions options)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.ContainsText))
            return Criteria.Empty;

        var text = options.ContainsText.Trim();
        var matches = new Criteria(GetNameField(row)).Contains(text);
        if (row is IRoleKeyRow roleKey)
            matches |= new Criteria(roleKey.RoleKeyField).Contains(text);

        return matches;
    }

    /// <summary>
    /// Converts a row, which has the fields that are selected to list the roles, to a role of the list.
    /// </summary>
    /// <param name="row">The row.</param>
    protected virtual IRoleDefinition ToListItem(TRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        return new RoleListItem
        {
            Id = Convert.ToString(row.GetIdField().AsObject(row), CultureInfo.InvariantCulture) ?? "",
            Name = Convert.ToString(GetNameField(row).AsObject(row), CultureInfo.InvariantCulture) ?? "",
            RoleKey = row is IRoleKeyRow roleKey && roleKey.RoleKeyField[row] is { } key && !string.IsNullOrWhiteSpace(key) ? key : null
        };
    }

    private static Field GetNameField(TRow row) => row.NameField ??
        throw new InvalidOperationException($"The row type {typeof(TRow).FullName} does not have a name property for its role name.");
}
