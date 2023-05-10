using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Services;

/// <summary>
/// Authorizes access to a service action. Optionally checks the permission provided as the first argument.
/// Use special permission key "?" to check for logged-in users, and "*" to allow anyone including anonymous access.
/// This returns a service error instead of raising an exception like PageAuthorize attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class ServiceAuthorizeAttribute : Attribute, IResourceFilter
{
    /// <inheritdoc/>
    public void OnResourceExecuted(ResourceExecutedContext context)
    {
    }

    /// <inheritdoc/>
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        if (string.IsNullOrEmpty(Permission))
        {
            if (context.HttpContext.User.IsLoggedIn())
                return;
        }
        else if (context.HttpContext.RequestServices.GetRequiredService<IPermissionService>().HasPermission(Permission))
            return;

        if (!string.IsNullOrEmpty(OrPermission) &&
            context.HttpContext.RequestServices.GetRequiredService<IPermissionService>().HasPermission(OrPermission))
            return;

        var myIndex = context.Filters.IndexOf(this);
        if (myIndex >= 0 && context.Filters.Skip(myIndex + 1)
            .Any(x => x is ServiceAuthorizeAttribute a && a.Override == true))
            return;

        var localizer = context.HttpContext.RequestServices.GetRequiredService<ITextLocalizer>();

        if (context.HttpContext.User.IsLoggedIn())
        {
            context.Result = new Result<ServiceResponse>(new ServiceResponse
            {
                Error = new ServiceError
                {
                    Code = "AccessDenied",
                    Message = localizer.Get("Authorization.AccessDenied")
                }
            });
            context.HttpContext.Response.StatusCode = 400;
        }
        else
        {
            context.Result = new Result<ServiceResponse>(new ServiceResponse
            {
                Error = new ServiceError
                {
                    Code = "NotLoggedIn",
                    Message = localizer.Get("Authorization.NotLoggedIn")
                }
            });
            context.HttpContext.Response.StatusCode = 400;
        }
    }
 
    /// <summary>
    /// Creates an instance of the ServiceAuthorizeAttribute.
    /// </summary>
    public ServiceAuthorizeAttribute()
    {
    }

    /// <summary>
    /// Creates an instance of the service authorize attribute while
    /// trying to determine the permission key from one of the permission attribute
    /// types the sourceType has.
    /// </summary>
    /// <param name="sourceType">Source type</param>
    /// <param name="attributeTypes">Attribute types to check in order</param>
    /// <exception cref="ArgumentNullException">Source type or attributeTypes is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">One fo the types is not a subclass of the PermissionAttributeBase.</exception>
    protected ServiceAuthorizeAttribute(Type sourceType, params Type[] attributeTypes)
    {
        if (sourceType == null)
            throw new ArgumentNullException(nameof(sourceType));

        if (attributeTypes.IsEmptyOrNull())
            throw new ArgumentNullException(nameof(attributeTypes));

        PermissionAttributeBase attr = null;
        foreach (var attributeType in attributeTypes)
        {
            var lst = sourceType.GetCustomAttributes(attributeType, true);
            if (lst.Length > 0)
            {
                attr = lst[0] as PermissionAttributeBase;
                if (attr == null)
                    throw new ArgumentOutOfRangeException(attributeType.Name + 
                        " is not a subclass of PermissionAttributeBase!");

                break;
            }
        }

        if (attr == null)
        {
            throw new ArgumentOutOfRangeException(nameof(sourceType),
                "ServiceAuthorize attribute is created with source type of " +
                sourceType.Name + ", but it has no " +
                string.Join(" OR ", attributeTypes.Select(x => x.Name)) + " attribute(s)");
        }

        Permission = attr.Permission;
    }

    /// <summary>
    /// Creates an instance of the ServiceAuthorizeAttribute, while deriving
    /// the permission key from the passed type's ReadPermissionAttribute
    /// </summary>
    /// <param name="sourceType"></param>
    public ServiceAuthorizeAttribute(Type sourceType)
        : this(sourceType, typeof(ReadPermissionAttribute))
    {
    }

    /// <summary>
    /// Creates an instance of the ServiceAuthorizeAttribute with the passed permission.
    /// </summary>
    /// <param name="permission">Permission key</param>
    public ServiceAuthorizeAttribute(object permission)
        : this()
    {
        Permission = permission?.ToString();
    }

    /// <summary>
    /// Creates an instance of the ServiceAuthorizeAttribute with a permission key
    /// generated by joining the passed permissions with a colon (:), e.g. "module:permission".
    /// </summary>
    /// <param name="module">Module</param>
    /// <param name="permission">Permission key</param>
    public ServiceAuthorizeAttribute(object module, object permission)
        : this(module.ToString() + ":" + permission)
    {
    }

    /// <summary>
    /// Creates an instance of the ServiceAuthorizeAttribute with a permission key
    /// generated by joining the passed permissions with a colon (:), e.g. "module:submodule:permission".
    /// </summary>
    /// <param name="module">Module</param>
    /// <param name="submodule">Submodule</param>
    /// <param name="permission">Permission key</param>
    public ServiceAuthorizeAttribute(object module, object submodule, object permission)
        : this(module.ToString() + ":" + submodule + ":" + permission)
    {
    }

    /// <summary>
    /// The permission key
    /// </summary>
    public string Permission { get; private set; }

    /// <summary>
    /// And optional secondary permission to check by OR,
    /// e.g. if this is specified an user does not have the Permission,
    /// the user will still be allowed access if he has the OrPermission.
    /// </summary>
    protected string OrPermission { get; set; }

    /// <summary>
    /// Should this attribute override the controller level attribute if any.
    /// The default is true.
    /// </summary>
    public bool Override { get; set; } = true;
}