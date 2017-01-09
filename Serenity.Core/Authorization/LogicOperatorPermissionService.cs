using Serenity.Abstractions;
using System;
using System.Collections.Concurrent;

namespace Serenity.Web
{
    /// <summary>
    /// Adds AND OR operator support to any IPermissionService implementation
    /// </summary>
    /// <remarks>
    /// Register this class in your application start, to allow | &amp; operators
    /// in your permission services, e.g.
    /// <code>
    /// registrar.RegisterInstance&lt;IPermissionService&gt;(new LogicOperatorPermissionService(new MyPermissionService()))
    /// </code>
    /// </remarks>
    public class LogicOperatorPermissionService : IPermissionService
    {
        private static readonly char[] and = new char[] { '&' };
        private static readonly char[] or = new char[] { '|' };
        private static readonly char[] chars = new Char[] { '|', '&' };
        private IPermissionService permissionService;
        private ConcurrentDictionary<string, string[][]> cache = new ConcurrentDictionary<string, string[][]>();

        public LogicOperatorPermissionService(IPermissionService permissionService)
        {
            Check.NotNull(permissionService, "permissionService");

            this.permissionService = permissionService;
        }

        public bool HasPermission(string permission)
        {
            if (string.IsNullOrEmpty(permission) ||
                permission.IndexOfAny(chars) < 0)
                return permissionService.HasPermission(permission);

            string[][] expr;
            if (!cache.TryGetValue(permission, out expr))
            {
                var parts = permission.Split(or, StringSplitOptions.RemoveEmptyEntries);
                expr = new string[parts.Length][];
                for (var i = 0; i < parts.Length; i++)
                {
                    expr[i] = parts[i].Split(and, StringSplitOptions.RemoveEmptyEntries);
                }

                cache[permission] = expr;
            }

            for (var r = 0; r < expr.Length; r++)
            {
                var p = expr[r];

                if (p.Length == 0)
                    continue;

                bool fail = false;
                for (var n = 0; n < p.Length; n++)
                {
                    if (!permissionService.HasPermission(p[n]))
                    {
                        fail = true;
                        break;
                    }
                }

                if (!fail)
                    return true;
            }

            return false;
        }
    }
}