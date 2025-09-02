namespace Serenity.Data;

/// <summary>
///   Contains static extension methods for DbField and Meta objects.</summary>
public static class ServiceAuthorizationExtensions
{
    private static string GetPermissionFor(Type sourceType, params Type[] attributeTypes)
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
                "ServiceAuthorize method is created with source type of " +
                sourceType.Name + ", but it has no " +
                string.Join(" OR ", attributeTypes.Select(x => x.Name)) + " attribute(s)");
        }

        return attr.Permission;
    }

    private static void AuthorizeOperation(IPermissionService permissions, ITextLocalizer localizer, Type sourceType, params Type[] attributeTypes)
    {
        var permission = GetPermissionFor(sourceType, attributeTypes);
        permissions.ValidatePermission(permission, localizer);
    }

    /// <summary>
    /// Authorizes list request access similar to [AuthorizeList] attribute.
    /// </summary>
    /// <typeparam name="TRow">Type of row to get ReadPermissionAttribute from</typeparam>
    /// <param name="permissions">Permission service</param>
    /// <param name="localizer">Text localizer</param>
    public static void AuthorizeList<TRow>(this IPermissionService permissions, ITextLocalizer localizer)
    {
        AuthorizeOperation(permissions, localizer, typeof(TRow), typeof(ReadPermissionAttribute));
    }

    /// <summary>
    /// Authorizes list request access similar to [AuthorizeList] attribute.
    /// </summary>
    /// <typeparam name="TRow">Type of row to get ReadPermissionAttribute from</typeparam>
    /// <param name="context">Request context</param>
    public static void AuthorizeList<TRow>(this IRequestContext context)
    {
        AuthorizeOperation(context.Permissions, context.Localizer, typeof(TRow), typeof(ReadPermissionAttribute));
    }

    /// <summary>
    /// Authorizes create request access similar to [AuthorizeCreate] attribute.
    /// </summary>
    /// <typeparam name="TRow">Type of row to get Insert/Modify/Read permission attribute from</typeparam>
    /// <param name="permissions">Permission service</param>
    /// <param name="localizer">Text localizer</param>
    public static void AuthorizeCreate<TRow>(this IPermissionService permissions, ITextLocalizer localizer)
    {
        AuthorizeOperation(permissions, localizer, typeof(TRow), typeof(InsertPermissionAttribute), typeof(ModifyPermissionAttribute), typeof(ReadPermissionAttribute));
    }

    /// <summary>
    /// Authorizes create request access similar to [AuthorizeCreate] attribute.
    /// </summary>
    /// <typeparam name="TRow">Type of row to get Insert/Modify/Read permission attribute from</typeparam>
    /// <param name="context">Request context</param>
    public static void AuthorizeCreate<TRow>(this IRequestContext context)
    {
        AuthorizeOperation(context.Permissions, context.Localizer, typeof(TRow), typeof(InsertPermissionAttribute), typeof(ModifyPermissionAttribute), typeof(ReadPermissionAttribute));
    }

    /// <summary>
    /// Authorizes delete request access similar to [AuthorizeUpdate] attribute.
    /// </summary>
    /// <typeparam name="TRow">Type of row to get Insert/Modify/Read permission attribute from</typeparam>
    /// <param name="permissions">Permission service</param>
    /// <param name="localizer">Text localizer</param>
    public static void AuthorizeUpdate<TRow>(this IPermissionService permissions, ITextLocalizer localizer)
    {
        AuthorizeOperation(permissions, localizer, typeof(TRow), typeof(UpdatePermissionAttribute), typeof(ModifyPermissionAttribute), typeof(ReadPermissionAttribute));
    }

    /// <summary>
    /// Authorizes delete request access similar to [AuthorizeUpdate] attribute.
    /// </summary>
    /// <typeparam name="TRow">Type of row to get Insert/Modify/Read permission attribute from</typeparam>
    /// <param name="context">Request context</param>
    public static void AuthorizeUpdate<TRow>(this IRequestContext context)
    {
        AuthorizeOperation(context.Permissions, context.Localizer, typeof(TRow), typeof(UpdatePermissionAttribute), typeof(ModifyPermissionAttribute), typeof(ReadPermissionAttribute));
    }

    /// <summary>
    /// Authorizes update request access similar to [AuthorizeDelete] attribute.
    /// </summary>
    /// <typeparam name="TRow">Type of row to get Delete/Modify/Read permission attribute from</typeparam>
    /// <param name="permissions">Permission service</param>
    /// <param name="localizer">Text localizer</param>
    public static void AuthorizeDelete<TRow>(this IPermissionService permissions, ITextLocalizer localizer)
    {
        AuthorizeOperation(permissions, localizer, typeof(TRow), typeof(DeletePermissionAttribute), typeof(ModifyPermissionAttribute), typeof(ReadPermissionAttribute));
    }

    /// <summary>
    /// Authorizes update request access similar to [AuthorizeDelete] attribute.
    /// </summary>
    /// <typeparam name="TRow">Type of row to get Delete/Modify/Read permission attribute from</typeparam>
    /// <param name="context">Request context</param>
    public static void AuthorizeDelete<TRow>(this IRequestContext context)
    {
        AuthorizeOperation(context.Permissions, context.Localizer, typeof(TRow), typeof(DeletePermissionAttribute), typeof(ModifyPermissionAttribute), typeof(ReadPermissionAttribute));
    }
}