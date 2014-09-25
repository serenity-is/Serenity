namespace BasicApplication.Administration
{
    using Serenity;
    using Serenity.Abstractions;
    using Serenity.Data;
    using System.Data;
    using MyRow = Entities.UserRow;

    public class UserRetrieveService : IUserRetrieveService
    {
        private static MyRow.RowFields fld { get { return MyRow.Fields; } }

        private UserDefinition GetFirst(IDbConnection connection, BaseCriteria criteria)
        {
            var user = connection.TrySingle<Entities.UserRow>(criteria);
            if (user != null)
                return new UserDefinition
                {
                    UserId = user.UserId.Value,
                    Username = user.Username,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    IsActive = user.IsActive.Value,
                    Source = user.Source,
                    PasswordHash = user.PasswordHash,
                    PasswordSalt = user.PasswordSalt,
                    UpdateDate = user.UpdateDate
                };

            return null;
        }

        public IUserDefinition ById(long id)
        {
            return TwoLevelCache.Get<UserDefinition>("UserByID_" + id.ToInvariant(), CacheExpiration.Never, CacheExpiration.OneDay, fld.GenerationKey, () =>
            {
                using (var connection = SqlConnections.NewByKey("Default"))
                    return GetFirst(connection, new Criteria(fld.UserId) == id);
            });
        }

        public IUserDefinition ByUsername(string username)
        {
            if (username.IsEmptyOrNull())
                return null;

            if (username.IndexOf(' ') > 0)
                return null;

            return TwoLevelCache.Get<UserDefinition>("UserByName_" + username, CacheExpiration.Never, CacheExpiration.OneDay, fld.GenerationKey, () =>
            {
                using (var connection = SqlConnections.NewByKey("Default"))
                    return GetFirst(connection, new Criteria(fld.Username) == username);
            });
        }
    }
}