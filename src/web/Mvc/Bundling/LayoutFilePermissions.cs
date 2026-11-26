namespace Serenity.Web;

/// <summary>
/// Helpers for the permissions of <see cref="LayoutScriptFileAttribute"/> and
/// <see cref="LayoutCssFileAttribute"/>. As elsewhere, a null or empty permission is
/// invalid and denies access, it does not mean public.
/// </summary>
public static class LayoutFilePermissions
{
    /// <summary>
    /// Returns true if the file permission is public ("*"), e.g. the file is
    /// included for everyone, including anonymous users.
    /// </summary>
    /// <param name="permission">The file permission</param>
    public static bool IsPublic(string? permission)
    {
        return permission?.Trim() == SpecialPermissionKeys.Public;
    }

    /// <summary>
    /// Returns true if the file with the permission can be included for the current user.
    /// Only public files are included when there is no permission service, and a file with
    /// a null or empty permission is never included.
    /// </summary>
    /// <param name="permissions">The permission service</param>
    /// <param name="permission">The file permission</param>
    public static bool IsGranted(IPermissionService? permissions, string? permission)
    {
        if (IsPublic(permission))
            return true;

        if (string.IsNullOrWhiteSpace(permission))
            return false;

        return permissions != null && permissions.HasPermission(permission.Trim());
    }

    /// <summary>
    /// Builds a dictionary of the permission of each file path. When a path is declared
    /// more than once, it is public if any of the declarations is public, otherwise the
    /// permissions are combined with "|". Null or empty permissions are kept as "DENY".
    /// </summary>
    /// <param name="files">The file paths and their permissions</param>
    public static IReadOnlyDictionary<string, string> ByPath(IEnumerable<(string? Path, string? Permission)> files)
    {
        ArgumentNullException.ThrowIfNull(files);

        var byPath = new Dictionary<string, List<string>?>(StringComparer.OrdinalIgnoreCase);
        foreach (var (rawPath, permission) in files)
        {
            var path = (rawPath ?? "").Trim();
            if (path.Length == 0)
                continue;

            if (IsPublic(permission))
            {
                byPath[path] = null;
                continue;
            }

            if (byPath.TryGetValue(path, out var list))
            {
                if (list == null) // public by another declaration
                    continue;
            }
            else
                byPath[path] = list = [];

            var normalized = Normalize(permission);
            if (!list.Contains(normalized, StringComparer.Ordinal))
                list.Add(normalized);
        }

        return byPath.ToDictionary(x => x.Key,
            x => x.Value == null ? SpecialPermissionKeys.Public : Join(x.Value, "|"),
            StringComparer.OrdinalIgnoreCase);
    }

    private static string Normalize(string? permission)
    {
        return string.IsNullOrWhiteSpace(permission) ? SpecialPermissionKeys.Deny : permission.Trim();
    }

    private static string Join(List<string> permissions, string op)
    {
        if (permissions.Count == 1)
            return permissions[0];

        return string.Join(op, permissions.Select(x => PermissionExpressionParser.IsExpression(x) ? "(" + x + ")" : x));
    }
}
