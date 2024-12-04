
using Serenity.Localization;
using System.Reflection;

namespace Serenity.Extensions;

public abstract class BasePermissionKeyLister(ITwoLevelCache cache, ITypeSource typeSource) : IPermissionKeyLister
{
    protected readonly ITwoLevelCache cache = cache ?? throw new ArgumentNullException(nameof(cache));
    protected readonly ITypeSource typeSource = typeSource ?? throw new ArgumentNullException(nameof(typeSource));

    /// <summary>
    /// Gets cache key for permission keys.
    /// </summary>
    /// <param name="includeRoles">True to include roles</param>
    protected virtual string GetCacheKey(bool includeRoles)
    {
        return "Administration:PermissionKeys:" + (includeRoles ? "IR" : "XR");
    }

    /// <summary>
    /// Gets cache duration for permission keys. Default is zero, e.g. never expires
    /// unless the cache is cleared by using the group key.
    /// </summary>
    protected virtual TimeSpan GetCacheDuration()
    {
        return TimeSpan.Zero;
    }

    /// <summary>
    /// Gets cache group key used to invalidate items.
    /// </summary>
    protected abstract string GetCacheGroupKey();


    /// <summary>
    /// Gets permission keys from cache or source.
    /// </summary>
    /// <param name="includeRoles">True to include permissions starting with Role:</param>
    protected virtual IEnumerable<string> GetCachedPermissionKeys(bool includeRoles)
    {
        return cache.GetLocalStoreOnly(GetCacheKey(includeRoles), GetCacheDuration(), GetCacheGroupKey(),
            () => GetPermissionKeys(includeRoles));
    }

    /// <summary>
    /// Gets nested permissions from a type source.
    /// </summary>
    /// <param name="typeSource">Type source</param>
    protected virtual IEnumerable<string> GetNestedPermissions(ITypeSource typeSource)
    {
        return NestedPermissionKeyRegistration.AddNestedPermissions(registry: null, typeSource);
    }

    /// <summary>
    /// Gets permissions from assemblies in a type source.
    /// </summary>
    /// <param name="typeSource">Type source</param>
    protected virtual IEnumerable<string> GetAssemblyPermissions(ITypeSource typeSource)
    {
        foreach (var attr in typeSource.GetAssemblyAttributes<PermissionAttributeBase>())
            if (!string.IsNullOrEmpty(attr.Permission))
                yield return attr.Permission;
    }

    /// <summary>
    /// Gets permissions from a type.
    /// </summary>
    /// <param name="type">Type</param>
    protected virtual IEnumerable<string> GetPermissionsFromType(Type type)
    {
        return GetPermissionsFromTypeAttributes(type)
            .Concat(GetPermissionsFromMethods(type))
            .Concat(GetPermissionsFromProperties(type));
    }

    /// <summary>
    /// Gets permissions from type attributes.
    /// </summary>
    /// <param name="type">Type</param>
    protected virtual IEnumerable<string> GetPermissionsFromTypeAttributes(Type type)
    {
        return GetAttributePermissions<PermissionAttributeBase>(type, x => x.Permission)
            .Concat(GetAttributePermissions<PageAuthorizeAttribute>(type, x => x.Permission))
            .Concat(GetAttributePermissions<ServiceAuthorizeAttribute>(type, x => x.Permission));
    }

    /// <summary>
    /// Gets permissions from methods of a type.
    /// </summary>
    /// <param name="type">Type</param>
    protected virtual IEnumerable<string> GetPermissionsFromMethods(Type type)
    {
        return type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .SelectMany(GetPermissionsFromMethod);
    }

    /// <summary>
    /// Gets permissions from a method. Default implementation returns permissions 
    /// from PermissionAttributeBase, PageAuthorizeAttribute and ServiceAuthorizeAttribute.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    protected virtual IEnumerable<string> GetPermissionsFromMethod(MethodInfo method)
    {
        return GetAttributePermissions<PermissionAttributeBase>(method, x => x.Permission)
            .Concat(GetAttributePermissions<PageAuthorizeAttribute>(method, x => x.Permission))
            .Concat(GetAttributePermissions<ServiceAuthorizeAttribute>(method, x => x.Permission));
    }

    /// <summary>
    /// Gets permissions from properties of a type.
    /// </summary>
    /// <param name="type">Type</param>
    protected virtual IEnumerable<string> GetPermissionsFromProperties(Type type)
    {
        return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .SelectMany(GetPermissionsFromProperty);
    }

    /// <summary>
    /// Gets permissions from a property attributes.
    /// Default implementation returns permissions from PermissionAttributeBase.
    /// </summary>
    /// <param name="property">Property</param>
    protected virtual IEnumerable<string> GetPermissionsFromProperty(PropertyInfo property)
    {
        return GetAttributePermissions<PermissionAttributeBase>(property, x => x.Permission);
    }

    /// <summary>
    /// Gets a list of permissions that are used as markers and should not be listed.
    /// Default implementation returns ["*", "?"]
    /// </summary>
    protected virtual IEnumerable<string> GetMarkerPermissions()
    {
        return ["*", "?"];
    }

    /// <summary>
    /// Gets a list of permissions that are private and should not be listed.
    /// Default implementation returns ["ImpersonateAs"]
    /// </summary>
    protected virtual IEnumerable<string> GetPrivatePermissions()
    {
        return ["ImpersonateAs"];
    }

    /// <summary>
    /// Gets a list of permissions that are external and should be listed.
    /// Common use case is to list permissions from database tables.
    /// </summary>
    protected virtual IEnumerable<string> GetExternalPermissions()
    {
        return [];
    }

    /// <summary>
    /// Gets a list of role keys.
    /// </summary>
    protected virtual IEnumerable<string> GetRoleKeys() 
    { 
        return [];
    }

    /// <summary>
    /// Checks if a permission key is a role permission key, e.g. starts with Role:
    /// </summary>
    /// <param name="key">Key</param>
    protected virtual bool IsRolePermission(string key)
    {
        return !string.IsNullOrEmpty(key) && key.StartsWith("Role:", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets all permission keys.
    /// </summary>
    /// <param name="includeRoles">True to include roles</param>
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

        add(GetExternalPermissions());

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

        foreach (var x in GetPrivatePermissions())
            target.Remove(x);

        foreach (var x in GetMarkerPermissions())
            target.Remove(x);

        return target;
    }

    /// <inheritdoc/>
    public virtual IEnumerable<string> ListPermissionKeys(bool includeRoles)
    {

        return GetCachedPermissionKeys(includeRoles);
    }

    /// <summary>
    /// Set of characters used to split permissions.
    /// </summary>

    protected static readonly char[] SplitChars = ['|', '&'];

    /// <summary>
    /// Splits permission string into multiple permissions.
    /// </summary>
    /// <param name="permission">Permission</param>
    /// <returns></returns>
    protected virtual IEnumerable<string> SplitPermissions(string permission)
    {
        if (string.IsNullOrEmpty(permission))
            return [];

        return permission.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// Gets permissions from a member attribute.
    /// </summary>
    /// <typeparam name="TAttr">Attribute type</typeparam>
    /// <param name="member">Member</param>
    /// <param name="getPermission">Permission callback</param>
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

    /// <summary>
    /// Gets permissions from a type attribute.
    /// </summary>
    /// <typeparam name="TAttr">Attribute type</typeparam>
    /// <param name="type">Type</param>
    /// <param name="getPermission">Permission callback</param>
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