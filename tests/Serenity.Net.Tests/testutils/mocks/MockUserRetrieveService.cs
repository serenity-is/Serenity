namespace Serenity.TestUtils;

/// <summary>
/// A simple in-memory implementation of <see cref="IUserRetrieveService"/> for tests,
/// looking up users by id or username.
/// </summary>
public class MockUserRetrieveService : IUserRetrieveService
{
    private readonly Dictionary<string, IUserDefinition> byId;
    private readonly Dictionary<string, IUserDefinition> byUsername;

    public MockUserRetrieveService()
        : this([])
    {
    }

    public MockUserRetrieveService(params IUserDefinition[] users)
    {
        byId = new Dictionary<string, IUserDefinition>(StringComparer.Ordinal);
        byUsername = new Dictionary<string, IUserDefinition>(StringComparer.OrdinalIgnoreCase);

        if (users != null)
        {
            foreach (var user in users)
                Add(user);
        }
    }

    /// <summary>
    /// Adds or updates a user so it can be retrieved.
    /// </summary>
    public void Add(IUserDefinition user)
    {
        if (user?.Id == null)
            return;

        byId[user.Id] = user;
        if (user.Username != null)
            byUsername[user.Username] = user;
    }

    /// <inheritdoc/>
    public IUserDefinition? ById(string id)
    {
        if (id == null || !byId.TryGetValue(id, out var user))
            return null;

        return user;
    }

    /// <inheritdoc/>
    public IUserDefinition? ByUsername(string username)
    {
        if (username == null || !byUsername.TryGetValue(username, out var user))
            return null;

        return user;
    }
}
