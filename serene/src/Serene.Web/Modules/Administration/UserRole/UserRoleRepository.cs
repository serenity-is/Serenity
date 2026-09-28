using System.Data;
using MyRow = Serene.Administration.UserRoleRow;

namespace Serene.Administration.Repositories;

public class UserRoleRepository(IRequestContext context) : BaseRepository(context)
{
    public SaveResponse Update(IUnitOfWork uow, UserRoleUpdateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.UserID is null)
            throw new ArgumentNullException(nameof(request.UserID));
        if (request.Roles is null)
            throw new ArgumentNullException(nameof(request.Roles));

        var userID = request.UserID.Value;
        var oldList = new HashSet<int>(
            GetExisting(uow.Connection, userID)
            .Select(x => x.RoleId!.Value));

        var newList = new HashSet<int>([.. request.Roles]);

        var fld = MyRow.Fields;

        if (oldList.SetEquals(newList))
            return new SaveResponse();

        foreach (var k in oldList)
        {
            if (newList.Contains(k))
                continue;

            new SqlDelete(fld.TableName)
                .Where(
                    fld.UserId == userID &
                    fld.RoleId == k)
                .Execute(uow.Connection);
        }

        foreach (var k in newList)
        {
            if (oldList.Contains(k))
                continue;

            uow.Connection.Insert(new MyRow
            {
                UserId = userID,
                RoleId = k
            });
        }

        Cache.InvalidateOnCommit(uow, fld);
        Cache.InvalidateOnCommit(uow, UserPermissionRow.Fields);

        return new SaveResponse();
    }

    private List<MyRow> GetExisting(IDbConnection connection, int userId)
    {
        var fld = MyRow.Fields;
        return connection.List<MyRow>(q =>
        {
            q.Select(fld.UserRoleId, fld.RoleId)
                .Where(fld.UserId == userId);
        });
    }

    public UserRoleListResponse List(IDbConnection connection, UserRoleListRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = new UserRoleListResponse
        {
            Entities = [.. GetExisting(connection, ArgumentChecks.NotNull(request.UserID))
                .Select(x => x.RoleId!.Value)]
        };

        return response;
    }
}