using System.Data;
using MyRow = Serene.Administration.UserPermissionRow;
using Microsoft.Extensions.Caching.Memory;

namespace Serene.Administration.Repositories;

public class UserPermissionRepository(IRequestContext context) : BaseRepository(context)
{
    private static MyRow.RowFields Fld { get { return MyRow.Fields; } }

    public SaveResponse Update(IUnitOfWork uow, UserPermissionUpdateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.UserID is null)
            throw new ArgumentNullException(nameof(request.UserID));
        if (request.Permissions is null)
            throw new ArgumentNullException(nameof(request.Permissions));

        var userID = request.UserID.Value;
        var oldList = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        foreach (var p in GetExisting(uow.Connection, userID))
            oldList[p.PermissionKey] = p.Granted.Value;

        var newList = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        foreach (var p in request.Permissions)
            newList[p.PermissionKey] = p.Granted ?? false;

        if (oldList.Count == newList.Count &&
            oldList.All(x => newList.ContainsKey(x.Key) && newList[x.Key] == x.Value))
            return new SaveResponse();

        foreach (var k in oldList.Keys)
        {
            if (newList.ContainsKey(k))
                continue;

            new SqlDelete(Fld.TableName)
                .Where(
                    new Criteria(Fld.UserId) == userID &
                    new Criteria(Fld.PermissionKey) == k)
                .Execute(uow.Connection);
        }

        foreach (var k in newList.Keys)
        {
            if (!oldList.ContainsKey(k))
            {
                uow.Connection.Insert(new MyRow
                {
                    UserId = userID,
                    PermissionKey = k,
                    Granted = newList[k]
                });
            }
            else if (oldList[k] != newList[k])
            {
                new SqlUpdate(Fld.TableName)
                    .Where(
                        Fld.UserId == userID &
                        Fld.PermissionKey == k)
                    .Set(Fld.Granted, newList[k])
                    .Execute(uow.Connection);
            }
        }

        Cache.InvalidateOnCommit(uow, Fld);

        return new SaveResponse();
    }

    private static List<MyRow> GetExisting(IDbConnection connection, int userId)
    {
        return connection.List<MyRow>(q =>
        {
            q.Select(Fld.UserPermissionId, Fld.PermissionKey, Fld.Granted)
                .Where(new Criteria(Fld.UserId) == userId);
        });
    }

    public ListResponse<MyRow> List(IDbConnection connection, UserPermissionListRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.UserID is null)
            throw new ArgumentNullException(nameof(request.UserID));

        var response = new ListResponse<MyRow>
        {
            Entities = GetExisting(connection, request.UserID.Value)
        };

        return response;
    }

    public ListResponse<string> ListRolePermissions(IDbConnection connection, UserPermissionListRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.UserID is null)
            throw new ArgumentNullException(nameof(request.UserID));

        var rp = RolePermissionRow.Fields.As("rp");
        var ur = UserRoleRow.Fields.As("ur");

        var query = new SqlQuery()
            .From(rp)
            .Select(rp.PermissionKey)
            .Distinct(true)
            .OrderBy(rp.PermissionKey);

        query.Where(rp.RoleId.In(
            query.SubQuery()
                .From(ur)
                .Select(ur.RoleId)
                .Where(ur.UserId == request.UserID.Value)
        ));

        return new ListResponse<string>
        {
            Entities = connection.Query<string>(query).ToList()
        };
    }
}