using Serenity.Extensions.Entities;
using MyRow = Serene.Administration.UserRow;

namespace Serene.Administration;

public interface IUserDeleteHandler : IDeleteHandlerAsync<MyRow> { }

public class UserDeleteHandler(IRequestContext context,
    IOptions<EnvironmentSettings> environmentOptions)
    : DeleteRequestHandlerAsync<MyRow>(context), IUserDeleteHandler
{
    private readonly IOptions<EnvironmentSettings> environmentOptions = environmentOptions ??
        throw new ArgumentNullException(nameof(environmentOptions));

    protected override async Task ValidateRequestAsync(CancellationToken cancellationToken = default)
    {
        await base.ValidateRequestAsync(cancellationToken).ConfigureAwait(false);

        environmentOptions.CheckPublicDemo(Row.UserId);
    }

    protected override async Task OnBeforeDeleteAsync(CancellationToken cancellationToken = default)
    {
        await base.OnBeforeDeleteAsync(cancellationToken).ConfigureAwait(false);

        await new SqlDelete(UserPreferenceRow.Fields.TableName)
            .Where(UserPreferenceRow.Fields.UserId == Row.UserId.Value)
            .ExecuteAsync(Connection, ExpectedRows.Ignore, cancellationToken: cancellationToken).ConfigureAwait(false);

        await new SqlDelete(UserRoleRow.Fields.TableName)
            .Where(UserRoleRow.Fields.UserId == Row.UserId.Value)
            .ExecuteAsync(Connection, ExpectedRows.Ignore, cancellationToken: cancellationToken).ConfigureAwait(false);

        await new SqlDelete(UserPermissionRow.Fields.TableName)
            .Where(UserPermissionRow.Fields.UserId == Row.UserId.Value)
            .ExecuteAsync(Connection, ExpectedRows.Ignore, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
