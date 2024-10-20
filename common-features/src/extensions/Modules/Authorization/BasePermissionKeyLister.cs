
using Serenity.Localization;

namespace Serenity.Extensions;

public abstract class BasePermissionKeyLister(ITwoLevelCache cache, ITypeSource typeSource) : IPermissionKeyLister
{
    protected readonly ITwoLevelCache cache = cache ?? throw new ArgumentNullException(nameof(cache));
    protected readonly ITypeSource typeSource = typeSource ?? throw new ArgumentNullException(nameof(typeSource));

    protected virtual string GetCacheKey(bool includeRoles)
    {
        return "Administration:PermissionKeys:" + (includeRoles ? "IR" : "XR");
    }

    protected virtual TimeSpan GetCacheDuration()
    {
        return TimeSpan.Zero;
    }

    protected abstract string GetCacheGroupKey();


    protected virtual IEnumerable<string> GetCachedPermissionKeys(bool includeRoles)
    {
        return cache.GetLocalStoreOnly(GetCacheKey(includeRoles), GetCacheDuration(), GetCacheGroupKey(),
            () => GetPermissionKeys(includeRoles));
    }

    protected virtual IEnumerable<string> GetNestedPermissions(ITypeSource typeSource)
    {
        return NestedPermissionKeyRegistration.AddNestedPermissions(registry: null, typeSource);
    }

    protected virtual IEnumerable<string> GetAssemblyPermissions(ITypeSource typeSource)
    {
        foreach (var attr in typeSource.GetAssemblyAttributes<PermissionAttributeBase>())
            if (!string.IsNullOrEmpty(attr.Permission))
                yield return attr.Permission;
    }

    protected virtual IEnumerable<string> GetPermissionsFromType(Type type)
    {
        return GetPermissionsFromTypeAttributes(type)
            .Concat(GetPermissionsFromMethods(type))
            .Concat(GetPermissionsFromProperties(type));
    }

    protected virtual IEnumerable<string> GetPermissionsFromTypeAttributes(Type type)
    {
        return GetAttributePermissions<PermissionAttributeBase>(type, x => x.Permission)
            .Concat(GetAttributePermissions<PageAuthorizeAttribute>(type, x => x.Permission))
            .Concat(GetAttributePermissions<ServiceAuthorizeAttribute>(type, x => x.Permission));
    }

    protected virtual IEnumerable<string> GetPermissionsFromMethods(Type type)
    {
        return type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .SelectMany(GetPermissionsFromMethod);
    }

    protected virtual IEnumerable<string> GetPermissionsFromMethod(MethodInfo method)
    {
        return GetAttributePermissions<PermissionAttributeBase>(method, x => x.Permission)
            .Concat(GetAttributePermissions<PageAuthorizeAttribute>(method, x => x.Permission))
            .Concat(GetAttributePermissions<ServiceAuthorizeAttribute>(method, x => x.Permission));
    }

    protected virtual IEnumerable<string> GetPermissionsFromProperties(Type type)
    {
        return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .SelectMany(GetPermissionsFromProperty);
    }

    protected virtual IEnumerable<string> GetPermissionsFromProperty(PropertyInfo property)
    {
        return GetAttributePermissions<PermissionAttributeBase>(property, x => x.Permission);
    }

    protected virtual IEnumerable<string> GetMarkerPermissions()
    {
        return ["*", "?"];
    }

    protected virtual IEnumerable<string> GetHiddenPermissions()
    {
        return ["ImpersonateAs"];
    }

    protected abstract IEnumerable<string> GetRoleKeys();

    protected virtual bool IsRolePermission(string key)
    {
        return !string.IsNullOrEmpty(key) && key.StartsWith("Role:", StringComparison.OrdinalIgnoreCase);
    }

    protected virtual IEnumerable<string> GetPermissionKeys(bool includeRoles)
    {
        var target = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        void add(IEnumerable<string> items)
        {
            foreach (var item in items)
                foreach (var perm in SplitPermissions(item))
                    target.Add(perm);
        }

        add(GetNestedPermissions(typeSource));
        add(GetAssemblyPermissions(typeSource));

        foreach (var type in typeSource.GetTypes())
        {
            add(GetPermissionsFromType(type));
        }

        foreach (var x in GetMarkerPermissions())
            target.Remove(x);

        foreach (var x in GetHiddenPermissions())
            target.Remove(x);

        if (includeRoles)
        {
            foreach (var role in GetRoleKeys())
                if (!string.IsNullOrWhiteSpace(role))
                    target.Add("Role:" + role);
        }
        else
        {
            foreach (var perm in target.Where(IsRolePermission).ToList())
            {
                target.Remove(perm);
            }
        }

        return target;
    }

    public virtual IEnumerable<string> ListPermissionKeys(bool includeRoles)
    {

        return GetCachedPermissionKeys(includeRoles);
    }


    protected static readonly char[] SplitChars = ['|', '&'];

    protected virtual IEnumerable<string> SplitPermissions(string permission)
    {
        if (string.IsNullOrEmpty(permission))
            return [];

        return permission.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
    }

    protected virtual IEnumerable<string> GetAttributePermissions<TAttr>(MemberInfo member,
        Func<TAttr, string> getPermission) where TAttr : Attribute
    {
        try
        {
            return member.GetCustomAttributes<TAttr>(false)
                .Select(getPermission)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();
        }
        catch
        {
            // GetCustomAttributes might fail
            return [];
        }
    }

    protected virtual IEnumerable<string> GetAttributePermissions<TAttr>(Type type,
        Func<TAttr, string> getPermission) where TAttr : Attribute
    {
        try
        {
            return type.GetCustomAttributes<TAttr>(false)
                .Select(getPermission)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();
        }
        catch
        {
            // GetCustomAttributes might fail
            return [];
        }
    }
}