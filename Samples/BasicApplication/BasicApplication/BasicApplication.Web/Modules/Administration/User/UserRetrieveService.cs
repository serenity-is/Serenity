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
            var row = new MyRow();
            var query = new SqlQuery()
                .From(row)
                .Select(
                    fld.UserId,
                    fld.Username,
                    fld.DisplayName,
                    fld.Email,
                    fld.PasswordHash,
                    fld.PasswordSalt,
                    fld.Source,
                    fld.UpdateDate,
                    fld.IsActive)
                .Where(criteria);

            if (query.GetFirst(connection))
                return new UserDefinition
                {
                    UserId = row.UserId.Value,
                    Username = row.Username,
                    Email = row.Email,
                    DisplayName = row.DisplayName,
                    IsActive = row.IsActive.Value,
                    Source = row.Source,
                    PasswordHash = row.PasswordHash,
                    PasswordSalt = row.PasswordSalt,
                    UpdateDate = row.UpdateDate
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