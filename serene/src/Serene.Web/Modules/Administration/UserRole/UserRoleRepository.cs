using System.Data;
using MyRow = Serene.Administration.UserRoleRow;

namespace Serene.Administration.Repositories;

public class UserRoleRepository(IRequestContext context) : BaseRepository(context)
{
    private static MyRow.RowFields Fld { get { return MyRow.Fields; } }

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
            .Select(x => x.RoleId.Value));

        var newList = new HashSet<int>([.. request.Roles]);

        if (oldList.SetEquals(newList))
            return new SaveResponse();

        foreach (var k in oldList)
        {
            if (newList.Contains(k))
                continue;

            new SqlDelete(Fld.TableName)
                .Where(
                    new Criteria(Fld.UserId) == userID &
                    new Criteria(Fld.RoleId) == k)
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

        Cache.InvalidateOnCommit(uow, Fld);
        Cache.InvalidateOnCommit(uow, UserPermissionRow.Fields);

        return new SaveResponse();
    }

    private List<MyRow> GetExisting(IDbConnection connection, int userId)
    {
        return connection.List<MyRow>(q =>
        {
            q.Select(Fld.UserRoleId, Fld.RoleId)
                .Where(new Criteria(Fld.UserId) == userId);
        });
    }

    public UserRoleListResponse List(IDbConnection connection, UserRoleListRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.UserID is null)
            throw new ArgumentNullException(nameof(request.UserID));

        var response = new UserRoleListResponse
        {
            Entities = GetExisting(connection, request.UserID.Value)
            .Select(x => x.RoleId.Value).ToList()
        };

        return response;
    }
}