
using Serenity.Localization;
using Serene.Administration;
using System.Reflection;

namespace Serene.AppServices;

public class PermissionKeyLister(ISqlConnections sqlConnections, ITwoLevelCache cache, ITypeSource typeSource) : IPermissionKeyLister
{
    private readonly ISqlConnections sqlConnections = sqlConnections ?? throw new ArgumentNullException(nameof(sqlConnections));
    private readonly ITwoLevelCache cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ITypeSource typeSource = typeSource ?? throw new ArgumentNullException(nameof(typeSource));

    public IEnumerable<string> ListPermissionKeys(bool includeRoles)
    {

        return cache.GetLocalStoreOnly("Administration:PermissionKeys:" +
            (includeRoles ? "IR" : "XR"), TimeSpan.Zero, RoleRow.Fields.GenerationKey, () =>
            {
                var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                result.AddRange(NestedPermissionKeyRegistration.AddNestedPermissions(registry: null, typeSource));

                foreach (var attr in typeSource.GetAssemblyAttributes<PermissionAttributeBase>())
                    if (!string.IsNullOrEmpty(attr.Permission))
                        result.AddRange(SplitPermissions(attr.Permission));

                foreach (var type in typeSource.GetTypes())
                {
                    ProcessAttributes<PageAuthorizeAttribute>(result, type, x => x.Permission);
                    ProcessAttributes<PermissionAttributeBase>(result, type, x => x.Permission);
                    ProcessAttributes<ServiceAuthorizeAttribute>(result, type, x => x.Permission);

                    foreach (var member in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                    {
                        ProcessAttributes<PageAuthorizeAttribute>(result, member, x => x.Permission);
                        ProcessAttributes<PermissionAttributeBase>(result, member, x => x.Permission);
                        ProcessAttributes<ServiceAuthorizeAttribute>(result, member, x => x.Permission);
                    }

                    foreach (var member in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                        if (member.GetIndexParameters().Length == 0)
                            ProcessAttributes<PermissionAttributeBase>(result, member, x => x.Permission);
                }

                result.Remove("ImpersonateAs");
                result.Remove("*");
                result.Remove("?");

                foreach (var perm in result.Where(x =>
                    x.StartsWith("Role:", StringComparison.OrdinalIgnoreCase)).ToList())
                {
                    result.Remove(perm);
                }

                return result;
            });
    }

    private static readonly string[] emptyPermissions = [];
    private static readonly char[] splitChar = ['|', '&'];

    private static string[] SplitPermissions(string permission)
    {
        if (string.IsNullOrEmpty(permission))
            return emptyPermissions;

        return permission.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
    }

    private static void ProcessAttributes<TAttr>(HashSet<string> hash,
        MemberInfo member, Func<TAttr, string> getPermission)
        where TAttr : Attribute
    {
        try
        {
            foreach (var attr in member.GetCustomAttributes<TAttr>(false))
            {
                var permission = getPermission(attr);
                hash.AddRange(SplitPermissions(permission));
            }
        }
        catch
        {
            // GetCustomAttributes might fail before .NET 4.6
        }
    }

    private static void ProcessAttributes<TAttr>(HashSet<string> hash,
            Type member, Func<TAttr, string> getPermission)
        where TAttr : Attribute
    {
        try
        {
            foreach (var attr in member.GetCustomAttributes<TAttr>(false))
            {
                var permission = getPermission(attr);
                hash.AddRange(SplitPermissions(permission));
            }
        }
        catch
        {
            // GetCustomAttributes might fail before .NET 4.6
        }
    }
}