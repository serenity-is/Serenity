using MyRow = Serene.Administration.UserRow;

namespace Serene.AppServices;

public class UserRetrieveService(ITwoLevelCache cache, ISqlConnections sqlConnections)
    : BaseUserRetrieveService<MyRow>(cache, sqlConnections)
{
    protected override UserDefinition ToUserDefinition(MyRow user)
    {
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
    }
}