using Serenity.Extensions.Entities;
using MyRow = Serene.Administration.UserRow;

namespace Serene.Administration;

public interface IUserDeleteHandler : IDeleteHandler<MyRow> { }

public class UserDeleteHandler(IRequestContext context,
    IOptions<EnvironmentSettings> environmentOptions)
    : DeleteRequestHandler<MyRow>(context), IUserDeleteHandler
{
    private readonly IOptions<EnvironmentSettings> environmentOptions = environmentOptions ??
        throw new ArgumentNullException(nameof(environmentOptions));

    protected override void ValidateRequest()
    {
        base.ValidateRequest();

        environmentOptions.CheckPublicDemo(Row.UserId);
    }

    protected override void OnBeforeDelete()
    {
        base.OnBeforeDelete();

        new SqlDelete(UserPreferenceRow.Fields.TableName)
            .Where(UserPreferenceRow.Fields.UserId == Row.UserId.Value)
            .Execute(Connection, ExpectedRows.Ignore);

        new SqlDelete(UserRoleRow.Fields.TableName)
            .Where(UserRoleRow.Fields.UserId == Row.UserId.Value)
            .Execute(Connection, ExpectedRows.Ignore);

        new SqlDelete(UserPermissionRow.Fields.TableName)
            .Where(UserPermissionRow.Fields.UserId == Row.UserId.Value)
            .Execute(Connection, ExpectedRows.Ignore);
    }
}