using System.Globalization;
using MyRow = Serene.Administration.UserRow;

namespace Serene.AppServices;

public class UserRetrieveService(ITwoLevelCache cache, ISqlConnections sqlConnections) : IUserRetrieveService
{
    private static MyRow.RowFields Fld { get { return MyRow.Fields; } }

    protected ITwoLevelCache Cache { get; } = cache;
    protected ISqlConnections SqlConnections { get; } = sqlConnections;

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
        return Cache.Get("UserByID_" + id, TimeSpan.Zero, TimeSpan.FromDays(1), Fld.GenerationKey, () =>
        {
            using var connection = SqlConnections.NewByKey("Default");
            return GetFirst(connection, new Criteria(Fld.UserId) == int.Parse(id, CultureInfo.InvariantCulture));
        });
    }

    public IUserDefinition ByUsername(string username)
    {
        if (string.IsNullOrEmpty(username))
            return null;

        return Cache.Get("UserByName_" + username.ToLowerInvariant(),
            TimeSpan.Zero, TimeSpan.FromDays(1), Fld.GenerationKey, () =>
        {
            using var connection = SqlConnections.NewByKey("Default");
            return GetFirst(connection, new Criteria(Fld.Username) == username);
        });
    }

    public static void RemoveCachedUser(ITwoLevelCache cache, int? userId, string username)
    {
        if (userId != null)
            cache.Remove("UserByID_" + userId);

        if (username != null)
            cache.Remove("UserByName_" + username.ToLowerInvariant());
    }
}