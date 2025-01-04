using System.Data;
using MyRow = Serene.Administration.RolePermissionRow;

namespace Serene.Administration.Repositories;

public class RolePermissionRepository(IRequestContext context) : BaseRepository(context)
{
    private static MyRow.RowFields Fld { get { return MyRow.Fields; } }

    public SaveResponse Update(IUnitOfWork uow, RolePermissionUpdateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.RoleID);
        ArgumentNullException.ThrowIfNull(request.Permissions);

        var roleID = request.RoleID.Value;
        var oldList = new HashSet<string>(
            GetExisting(uow.Connection, roleID)
            .Select(x => x.PermissionKey), StringComparer.OrdinalIgnoreCase);

        var newList = new HashSet<string>([.. request.Permissions],
            StringComparer.OrdinalIgnoreCase);

        if (oldList.SetEquals(newList))
            return new SaveResponse();

        foreach (var k in oldList)
        {
            if (newList.Contains(k))
                continue;

            new SqlDelete(Fld.TableName)
                .Where(
                    new Criteria(Fld.RoleId) == roleID &
                    new Criteria(Fld.PermissionKey) == k)
                .Execute(uow.Connection);
        }

        foreach (var k in newList)
        {
            if (oldList.Contains(k))
                continue;

            uow.Connection.Insert(new MyRow
            {
                RoleId = roleID,
                PermissionKey = k
            });
        }

        Cache.InvalidateOnCommit(uow, Fld);
        Cache.InvalidateOnCommit(uow, UserPermissionRow.Fields);

        return new SaveResponse();
    }

    private List<MyRow> GetExisting(IDbConnection connection, int roleId)
    {
        return connection.List<MyRow>(q =>
        {
            q.Select(Fld.RolePermissionId, Fld.PermissionKey)
                .Where(new Criteria(Fld.RoleId) == roleId);
        });
    }

    public RolePermissionListResponse List(IDbConnection connection, RolePermissionListRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.RoleID is null)
            throw new ArgumentNullException(nameof(request.RoleID));

        var response = new RolePermissionListResponse
        {
            Entities = GetExisting(connection, request.RoleID.Value)
                .Select(x => x.PermissionKey).ToList()
        };

        return response;
    }
}