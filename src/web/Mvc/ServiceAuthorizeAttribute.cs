using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Services;

/// <summary>
/// Authorizes access to a service action. Optionally checks the permission provided as the first argument.
/// Use special permission key <c>?</c> to check for logged-in users, and <c>*</c> to allow anyone including anonymous access.
/// This returns a service error instead of raising an exception like <see cref="Serenity.Web.PageAuthorizeAttribute"/>.
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
    /// Initializes a new instance of the <see cref="ServiceAuthorizeAttribute"/> class.
    /// </summary>
    public ServiceAuthorizeAttribute()
    {
    }

    /// <summary>
    /// Initializes an instance of the service authorize attribute while
    /// trying to determine the permission key from one of the permission attribute
    /// types the source type has.
    /// </summary>
    /// <param name="sourceType">The source type.</param>
    /// <param name="attributeTypes">The attribute types to check in order.</param>
    /// <exception cref="ArgumentNullException">Source type or attribute types is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">One of the types is not a subclass of <see cref="PermissionAttributeBase"/>.</exception>
    protected ServiceAuthorizeAttribute(Type sourceType, params Type[] attributeTypes)
    {
        ArgumentNullException.ThrowIfNull(sourceType);

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
    /// Initializes an instance of the <see cref="ServiceAuthorizeAttribute"/> class, while deriving
    /// the permission key from the passed type's <see cref="ReadPermissionAttribute"/>.
    /// </summary>
    /// <param name="sourceType">The source type.</param>
    public ServiceAuthorizeAttribute(Type sourceType)
        : this(sourceType, typeof(ReadPermissionAttribute))
    {
    }

    /// <summary>
    /// Initializes an instance of the <see cref="ServiceAuthorizeAttribute"/> class with the passed permission.
    /// </summary>
    /// <param name="permission">The permission key.</param>
    public ServiceAuthorizeAttribute(object permission)
        : this()
    {
        Permission = permission?.ToString();
    }

    /// <summary>
    /// Initializes an instance of the <see cref="ServiceAuthorizeAttribute"/> class with a permission key
    /// generated by joining the passed permissions with a colon (<c>:</c>), e.g. <c>module:permission</c>.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="permission">The permission key.</param>
    public ServiceAuthorizeAttribute(object module, object permission)
        : this(module.ToString() + ":" + permission)
    {
    }

    /// <summary>
    /// Initializes an instance of the <see cref="ServiceAuthorizeAttribute"/> class with a permission key
    /// generated by joining the passed permissions with a colon (<c>:</c>), e.g. <c>module:submodule:permission</c>.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="submodule">The submodule.</param>
    /// <param name="permission">The permission key.</param>
    public ServiceAuthorizeAttribute(object module, object submodule, object permission)
        : this(module.ToString() + ":" + submodule + ":" + permission)
    {
    }

    /// <summary>
    /// Gets the permission key.
    /// </summary>
    public string Permission { get; private set; }

    /// <summary>
    /// Gets or sets an optional secondary permission to check by OR,
    /// e.g. if this is specified and the user does not have the <see cref="Permission"/>,
    /// the user will still be allowed access if they have the <see cref="OrPermission"/>.
    /// </summary>
    protected string OrPermission { get; set; }

    /// <summary>
    /// Gets or sets whether this attribute should override the controller level attribute if any.
    /// The default is <c>true</c>.
    /// </summary>
    public bool Override { get; set; } = true;
}