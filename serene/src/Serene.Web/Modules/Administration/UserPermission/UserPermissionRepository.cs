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

        var userID = ArgumentChecks.NotNull(request.UserID);
        var oldList = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        foreach (var p in GetExisting(uow.Connection, userID))
            oldList[p.PermissionKey!] = p.Granted!.Value;

        var newList = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        foreach (var p in ArgumentChecks.NotNull(request.Permissions))
            newList[p.PermissionKey!] = p.Granted ?? false;

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
            if (!oldList.TryGetValue(k, out bool value))
            {
                uow.Connection.Insert(new MyRow
                {
                    UserId = userID,
                    PermissionKey = k,
                    Granted = newList[k]
                });
            }
            else if (value != newList[k])
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

        var response = new ListResponse<MyRow>
        {
            Entities = GetExisting(connection, ArgumentChecks.NotNull(request.UserID))
        };

        return response;
    }

    public ListResponse<string> ListRolePermissions(IDbConnection connection, UserPermissionListRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new ListResponse<string>
        {
            Entities = [.. connection.Query<string>(
                new SqlQuery().From(new RolePermissionRow(), (fields, query) => query
                    .Select(fields.PermissionKey)
                    .Distinct(true)
                    .OrderBy(fields.PermissionKey)
                    .Where(fields.RoleId.In(query.SubQueryFrom(UserRoleRow.Fields, (urFields, urQuery) => urQuery
                        .Select(urFields.RoleId)
                        .Where(urFields.UserId == ArgumentChecks.NotNull(request.UserID)))))))]
        };
    }
}