using Microsoft.AspNetCore.Http;

namespace Serenity.Web;

/// <summary>
/// Extension methods providing partial compatibility with legacy 
/// VirtualPathUtility.
/// </summary>
public static class VirtualPathUtility
{
    /// <summary>
    /// Converts a path to absolute
    /// </summary>
    /// <param name="accessor">HTTP context accessor</param>
    /// <param name="contentPath">Content path</param>
    public static string ToAbsolute(IHttpContextAccessor accessor, string contentPath)
    {
        return ToAbsolute(accessor?.HttpContext, contentPath);
    }

    /// <summary>
    /// Converts a path to absolute
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="contentPath">Content path</param>
    public static string ToAbsolute(HttpContext context, string contentPath)
    {
        return ToAbsolute(context?.Request?.PathBase ?? PathString.Empty, contentPath);
    }

    /// <summary>
    /// Converts a path to absolute
    /// </summary>
    /// <param name="pathBase">Path base</param>
    /// <param name="path">Content path</param>
    public static string ToAbsolute(PathString pathBase, string path)
    {
        if (!pathBase.HasValue)
            pathBase = PathString.Empty;

        if (path.StartsWith("~/", StringComparison.Ordinal))
        {
            var segment = new PathString(path[1..]);
            return pathBase.Add(segment).Value;
        }

        return path;
    }
}