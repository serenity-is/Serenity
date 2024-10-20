using System.Globalization;
using MyRow = Serene.Administration.UserRow;

namespace Serene.AppServices;

public class UserRetrieveService(ITwoLevelCache cache, ISqlConnections sqlConnections) : IUserRetrieveService, IRemoveAll, IRemoveCachedUser
{
    protected readonly ITwoLevelCache cache = cache ?? throw new ArgumentNullException(nameof(cache));
    protected readonly ISqlConnections sqlConnections = sqlConnections ?? throw new ArgumentNullException(nameof(cache));

    private static UserDefinition GetFirst(IDbConnection connection, BaseCriteria criteria)
    {
        var user = connection.TrySingle<MyRow>(criteria);
        if (user != null)
            return new UserDefinition
            {
                UserId = user.UserId.Value,
                Username = user.Username,
                Email = user.Email,
                UserImage = user.UserImage,
                DisplayName = user.DisplayName,
                IsActive = user.IsActive.Value,
                Source = user.Source,
                PasswordHash = user.PasswordHash,
                PasswordSalt = user.PasswordSalt,
                UpdateDate = user.UpdateDate,
                LastDirectoryUpdate = user.LastDirectoryUpdate
            };

        return null;
    }

    public IUserDefinition ById(string id)
    {
        return cache.Get("UserByID_" + id, TimeSpan.Zero, TimeSpan.FromDays(1), MyRow.Fields.GenerationKey, () =>
        {
            using var connection = sqlConnections.NewByKey("Default");
            return GetFirst(connection, new Criteria(MyRow.Fields.UserId) == int.Parse(id, CultureInfo.InvariantCulture));
        });
    }

    public IUserDefinition ByUsername(string username)
    {
        if (string.IsNullOrEmpty(username))
            return null;

        return cache.Get("UserByName_" + username.ToLowerInvariant(),
            TimeSpan.Zero, TimeSpan.FromDays(1), MyRow.Fields.GenerationKey, () =>
        {
            using var connection = sqlConnections.NewByKey("Default");
            return GetFirst(connection, new Criteria(MyRow.Fields.Username) == username);
        });
    }

    public void RemoveAll()
    {
        cache.ExpireGroupItems(MyRow.Fields.GenerationKey);
    }

    public void RemoveCachedUser(string userId, string username)
    {
        if (userId != null && int.TryParse(userId, CultureInfo.InvariantCulture, out int id))
            cache.Remove("UserByID_" + id.ToInvariant());
        if (username != null)
            cache.Remove("UserByName_" + username.ToLowerInvariant());
    }
}