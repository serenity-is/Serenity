namespace Serenity.Extensions;

/// <summary>
/// Base user list service that lists the users from their rows. The username is the name field of the row, and the
/// display name, email and active state are read from it when it has <see cref="IDisplayNameRow"/>,
/// <see cref="IEmailRow"/> and <see cref="IIsActiveRow"/>. Only these fields are selected, so nothing like the
/// password hash is loaded.
/// </summary>
/// <typeparam name="TRow">User row type</typeparam>
/// <param name="sqlConnections">SQL connections</param>
public abstract class BaseUserListService<TRow>(ISqlConnections sqlConnections) : IUserListService
    where TRow : class, IRow, IIdRow, INameRow, new()
{
    /// <summary>
    /// The number of identifiers or usernames that are asked for with one query, so the number of its parameters
    /// stays below what the databases allow.
    /// </summary>
    protected const int MaxValuesPerQuery = 500;

    private readonly ISqlConnections sqlConnections = sqlConnections ?? throw new ArgumentNullException(nameof(sqlConnections));

    /// <inheritdoc/>
    public virtual IReadOnlyList<IUserDefinition> ListUsers(UserListOptions? options = null)
    {
        options ??= new();

        var row = new TRow();
        var queries = new List<BaseCriteria>();

        if (options.UserIds is not null || options.Usernames is not null)
        {
            var ids = ToValues(options.UserIds).Select(x => ToIdValue(row, x)).Where(x => x is not null).Cast<object>().ToList();
            var usernames = ToValues(options.Usernames).Select(x => GetUsernameValue(row, x)).ToList();

            // what is asked for by who they are is listed whatever their state
            foreach (var chunk in ids.Chunk(MaxValuesPerQuery))
                queries.Add(And(new Criteria(row.GetIdField()).In(chunk), GetTextCriteria(row, options)));

            foreach (var chunk in usernames.Chunk(MaxValuesPerQuery))
                queries.Add(And(new Criteria(GetUsernameCriteriaField(row)).In(chunk), GetTextCriteria(row, options)));

            // nobody was asked for
            if (queries.Count == 0)
                return [];
        }
        else
            queries.Add(GetCriteria(row, options));

        using var connection = sqlConnections.NewFor<TRow>();
        var rows = new List<TRow>();
        foreach (var criteria in queries)
            rows.AddRange(connection.List<TRow>(query =>
            {
                SelectFields(query, row);

                if (!criteria.IsEmpty)
                    query.Where(criteria);

                query.OrderBy(GetUsernameField(row));

                // the users of other queries may come before them, so it is applied on the merged users then
                if (options.Take > 0 && queries.Count == 1)
                    query.Take(options.Take);
            }));

        var users = rows.Select(ToListItem).ToList();
        if (queries.Count > 1)
        {
            users = [.. users.DistinctBy(x => x.Id, StringComparer.Ordinal)
                .OrderBy(x => x.Username, StringComparer.CurrentCultureIgnoreCase)];

            if (options.Take > 0)
                users = [.. users.Take(options.Take)];
        }

        return users;
    }

    /// <summary>
    /// Gets the criteria of the users to list when they are not asked for by who they are, from the options.
    /// </summary>
    /// <param name="row">A user row, which has the fields.</param>
    /// <param name="options">The options.</param>
    protected virtual BaseCriteria GetCriteria(TRow row, UserListOptions options)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(options);

        var criteria = Criteria.Empty;

        // 1 is active, 0 is disabled and -1 is deleted
        if (row is IIsActiveRow isActive)
            criteria &= options.IncludeInactive ?
                new Criteria(isActive.IsActiveField) >= 0 : new Criteria(isActive.IsActiveField) == 1;

        return criteria & GetTextCriteria(row, options);
    }

    /// <summary>
    /// Gets the criteria for the text that the username or the display name should contain, if there is one.
    /// </summary>
    /// <param name="row">A user row, which has the fields.</param>
    /// <param name="options">The options.</param>
    protected virtual BaseCriteria GetTextCriteria(TRow row, UserListOptions options)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.ContainsText))
            return Criteria.Empty;

        var text = options.ContainsText.Trim();

        // the normalized username is what the users are compared by, so a username is found whatever its case, or
        // the collation of the database, e.g. the Turkish dotted and dotless i
        var matches = row is INormalizedUsernameRow normalized ?
            new Criteria(normalized.NormalizedUsernameField).Contains(BaseUserRetrieveService.NormalizeUsername(text)) :
            new Criteria(GetUsernameField(row)).Contains(text);
        if (row is IDisplayNameRow displayName)
            matches |= new Criteria(displayName.DisplayNameField).Contains(text);

        return matches;
    }

    /// <summary>
    /// Selects the fields that a user of the list has.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="row">A user row, which has the fields.</param>
    protected virtual void SelectFields(SqlQuery query, TRow row)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(row);

        query.Select(row.GetIdField()).Select(GetUsernameField(row));

        if (row is IDisplayNameRow displayName)
            query.Select(displayName.DisplayNameField);

        if (row is IEmailRow email)
            query.Select(email.EmailField);

        if (row is IIsActiveRow isActive)
            query.Select(isActive.IsActiveField);
    }

    /// <summary>
    /// Converts a row, which has the fields selected by <see cref="SelectFields"/>, to a user of the list.
    /// </summary>
    /// <param name="row">The row.</param>
    protected virtual IUserDefinition ToListItem(TRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        var username = Convert.ToString(GetUsernameField(row).AsObject(row), CultureInfo.InvariantCulture) ?? "";

        return new UserListItem
        {
            Id = Convert.ToString(row.GetIdField().AsObject(row), CultureInfo.InvariantCulture) ?? "",
            Username = username,
            DisplayName = row is IDisplayNameRow displayName && displayName.DisplayNameField[row] is { Length: > 0 } text ?
                text : username,
            Email = row is IEmailRow email ? email.EmailField[row] : null,
            IsActive = row is IIsActiveRow isActive ? isActive.IsActiveField[row] ?? 1 : (short)1
        };
    }

    private static BaseCriteria And(BaseCriteria first, BaseCriteria second) => first & second;

    private static List<string> ToValues(IEnumerable<string>? values) => [.. (values ?? [])
        .Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct(StringComparer.Ordinal)];

    /// <summary>
    /// Converts an identifier to the type of the id field, so it can be compared with it.
    /// </summary>
    /// <returns>The value, or null if it is not a valid identifier, e.g. a text for a number.</returns>
    private static object? ToIdValue(TRow row, string id)
    {
        try
        {
            return row.GetIdField().ConvertValue(id, CultureInfo.InvariantCulture);
        }
        catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException or ArgumentException)
        {
            return null;
        }
    }

    // the same rule as the users are looked up by their usernames with
    private static object GetUsernameValue(TRow row, string username) =>
        row is INormalizedUsernameRow ? BaseUserRetrieveService.NormalizeUsername(username) : username;

    private static Field GetUsernameCriteriaField(TRow row) =>
        row is INormalizedUsernameRow normalized ? normalized.NormalizedUsernameField : GetUsernameField(row);

    private static Field GetUsernameField(TRow row) => row.NameField ??
        throw new InvalidOperationException($"The row type {typeof(TRow).FullName} does not have a name property for its username.");
}
